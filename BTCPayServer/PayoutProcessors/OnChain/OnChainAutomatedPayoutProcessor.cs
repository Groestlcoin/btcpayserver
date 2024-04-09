using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Client.Models;
using BTCPayServer.Data;
using BTCPayServer.Events;
using BTCPayServer.HostedServices;
using BTCPayServer.Payments;
using BTCPayServer.Payments.Bitcoin;
using BTCPayServer.Services;
using BTCPayServer.Services.Invoices;
using BTCPayServer.Services.Stores;
using BTCPayServer.Services.Wallets;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBXplorer;
using NBXplorer.DerivationStrategy;
using PayoutData = BTCPayServer.Data.PayoutData;
using PayoutProcessorData = BTCPayServer.Data.PayoutProcessorData;

namespace BTCPayServer.PayoutProcessors.OnChain
{
	public class OnChainAutomatedPayoutProcessor : BaseAutomatedPayoutProcessor<OnChainAutomatedPayoutBlob>
	{
		private readonly ExplorerClientProvider _explorerClientProvider;
		private readonly BTCPayWalletProvider _btcPayWalletProvider;
		private readonly BTCPayNetworkJsonSerializerSettings _btcPayNetworkJsonSerializerSettings;
		private readonly BitcoinLikePayoutHandler _bitcoinLikePayoutHandler;
		private readonly PaymentMethodHandlerDictionary _handlers;
		private readonly IFeeProviderFactory _feeProviderFactory;

		public OnChainAutomatedPayoutProcessor(
			ApplicationDbContextFactory applicationDbContextFactory,
			ExplorerClientProvider explorerClientProvider,
			BTCPayWalletProvider btcPayWalletProvider,
			BTCPayNetworkJsonSerializerSettings btcPayNetworkJsonSerializerSettings,
			ILoggerFactory logger,
			BitcoinLikePayoutHandler bitcoinLikePayoutHandler,
			EventAggregator eventAggregator,
			WalletRepository walletRepository,
			StoreRepository storeRepository,
			PayoutProcessorData payoutProcesserSettings,
			PullPaymentHostedService pullPaymentHostedService,
			PaymentMethodHandlerDictionary handlers,
			IPluginHookService pluginHookService,
			IFeeProviderFactory feeProviderFactory) :
			base(logger, storeRepository, payoutProcesserSettings, applicationDbContextFactory,
				handlers, pluginHookService, eventAggregator)
		{
			_explorerClientProvider = explorerClientProvider;
			_btcPayWalletProvider = btcPayWalletProvider;
			_btcPayNetworkJsonSerializerSettings = btcPayNetworkJsonSerializerSettings;
			_bitcoinLikePayoutHandler = bitcoinLikePayoutHandler;
			WalletRepository = walletRepository;
			_handlers = handlers;
			_feeProviderFactory = feeProviderFactory;
		}

		public WalletRepository WalletRepository { get; }

		protected override async Task Process(object paymentMethodConfig, List<PayoutData> payouts)
		{
			if (paymentMethodConfig is not DerivationSchemeSettings { IsHotWallet: true } config)
			{
				return;
			}
			var network = _handlers.TryGetNetwork(this.PaymentMethodId);
			if (network is null || !_explorerClientProvider.IsAvailable(network.CryptoCode))
            {
                return;
            }
            
            var explorerClient = _explorerClientProvider.GetExplorerClient(network.CryptoCode);

            var extKeyStr = await explorerClient.GetMetadataAsync<string>(
                config.AccountDerivation,
                WellknownMetadataKeys.AccountHDKey);
            if (extKeyStr == null)
            {
                return;
            }

            var wallet = _btcPayWalletProvider.GetWallet(PaymentMethodId.CryptoCode);

            var reccoins = (await wallet.GetUnspentCoins(config.AccountDerivation)).ToArray();
            var coins = reccoins.Select(coin => coin.Coin).ToArray();

            var accountKey = ExtKey.Parse(extKeyStr, network.NBitcoinNetwork);
            var keys = reccoins.Select(coin => accountKey.Derive(coin.KeyPath).PrivateKey).ToArray();
            Transaction workingTx = null;
            decimal? failedAmount = null;
            var changeAddress = await explorerClient.GetUnusedAsync(
                config.AccountDerivation, DerivationFeature.Change, 0, true);

            var processorBlob = GetBlob(PayoutProcessorSettings);
            var payoutToBlobs = payouts.ToDictionary(data => data, data => data.GetBlob(_btcPayNetworkJsonSerializerSettings));
            if (payoutToBlobs.Sum(pair => pair.Value.CryptoAmount) < processorBlob.Threshold)
            {
                return;
            }
            
            var feeRate = await this._feeProviderFactory.CreateFeeProvider(network).GetFeeRateAsync(Math.Max(processorBlob.FeeTargetBlock, 1));

            var transfersProcessing = new List<KeyValuePair<PayoutData, PayoutBlob>>();
            foreach (var transferRequest in payoutToBlobs)
            {
                var blob = transferRequest.Value;
                if (failedAmount.HasValue && blob.CryptoAmount >= failedAmount)
                {
                    continue;
                }

                var claimDestination =
                    await _bitcoinLikePayoutHandler.ParseClaimDestination(PaymentMethodId, blob.Destination, CancellationToken);
                if (!string.IsNullOrEmpty(claimDestination.error))
                {
                    continue;
                }

                var bitcoinClaimDestination = (IBitcoinLikeClaimDestination)claimDestination.destination;
                var txBuilder = network.NBitcoinNetwork.CreateTransactionBuilder()
                    .AddCoins(coins)
                    .AddKeys(keys);

                if (workingTx is not null)
                {
                    foreach (var txout in workingTx.Outputs.Where(txout =>
                                 !txout.IsTo(changeAddress.Address)))
                    {
                        txBuilder.Send(txout.ScriptPubKey, txout.Value);
                    }
                }

                txBuilder.Send(bitcoinClaimDestination.Address,
                    new Money(blob.CryptoAmount.Value, MoneyUnit.BTC));

                try
                {
                    txBuilder.SetChange(changeAddress.Address);
                    txBuilder.SendEstimatedFees(feeRate);
                    workingTx = txBuilder.BuildTransaction(true);
                    transfersProcessing.Add(transferRequest);
                }
                catch (NotEnoughFundsException)
                {

                    failedAmount = blob.CryptoAmount;
                    //keep going, we prioritize withdraws by time but if there is some other we can fit, we should
                }
            }

            if (workingTx is not null)
            {
                try
                {
                    var txHash = workingTx.GetHash();
                    foreach (var payoutData in transfersProcessing)
                    {
                        payoutData.Key.State = PayoutState.InProgress;
                        _bitcoinLikePayoutHandler.SetProofBlob(payoutData.Key,
                            new PayoutTransactionOnChainBlob()
                            {
                                Accounted = true,
                                TransactionId = txHash,
                                Candidates = new HashSet<uint256>() { txHash }
                            });
                    }
                    TaskCompletionSource<bool> tcs = new();
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(TimeSpan.FromSeconds(20));
                    var task = _eventAggregator.WaitNext<NewOnChainTransactionEvent>(
                        e => e.NewTransactionEvent.TransactionData.TransactionHash == txHash,
                        cts.Token);
                    var broadcastResult = await explorerClient.BroadcastAsync(workingTx, cts.Token);
                    if (!broadcastResult.Success)
                    {
                        tcs.SetResult(false);
                    }
                    var walletId = new WalletId(PayoutProcessorSettings.StoreId, network.CryptoCode);
                    foreach (var payoutData in transfersProcessing)
                    {
                        await WalletRepository.AddWalletTransactionAttachment(walletId,
                            txHash,
                            Attachment.Payout(payoutData.Key.PullPaymentDataId, payoutData.Key.Id));
                    }
                    await Task.WhenAny(tcs.Task, task);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    Logs.PayServer.LogError(e, "Could not finalize and broadcast");
                }
            }
        }
    }
}
