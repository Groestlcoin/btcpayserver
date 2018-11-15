![GRSPay Server](BTCPayServer/wwwroot/img/btc_pay_BG_twitter.png)

[![Docker Automated build](https://img.shields.io/docker/automated/jrottenberg/ffmpeg.svg)](https://hub.docker.com/r/nicolasdorier/btcpayserver/)
[![Deploy to Azure](https://azuredeploy.net/deploybutton.svg)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fbtcpayserver%2Fbtcpayserver-azure%2Fmaster%2Fazuredeploy.json)
[![CircleCI](https://circleci.com/gh/btcpayserver/btcpayserver.svg?style=svg)](https://circleci.com/gh/btcpayserver/btcpayserver)

# GRSPay Server

## Introduction

GRSPay is a free and open-source cryptocurrency payment processor which allows you to receive payments in Groestlcoin, with no fees, transaction cost or a middleman.

GRSPay is a non-custodial invoicing system which eliminates the involvement of a third-party. Payments with GRSPay go directly to your wallet, which increases the privacy and security. Your private keys are never uploaded to the server. There is no address re-use, since each invoice generates a new address deriving from your xpubkey.

The software is built in C#. It allows for your website to be easily configured as a self-hosted payment processor.

You can run GRSPay as a self-hosted solution on your own server, or use a [third-party host](https://github.com/btcpayserver/btcpayserver-doc/blob/master/ThirdPartyHosting.md).

The self-hosted solution allows you not only to attach an unlimited number of stores and use the Lightning Network but also become the payment processor for others.

Thanks to the apps built on top of it, you can use GRSPay to receive donations or have an in-store POS system.

## Features

* Direct, P2P Groestlcoin payments
* Lightning Network support (LND and c-lightning)
* Complete control over private keys
* Enchanced privacy
* SegWit support
* Process payments for others
* Payment buttons
* Point of sale
* No transaction fees (other than those for the groestlcoin network)
* No processing fees
* No middleman
* No KYC

## How to build

While the documentation advises to use docker-compose, you may want to build GRSPay yourself.

First install .NET Core SDK v2.1.4 (with patch version >= 403) as specified by [Microsoft website](https://www.microsoft.com/net/download/dotnet-core/2.1).

On Powershell:
```
.\build.ps1
```

On linux:
```
./build.sh
```

## How to run

Use the `run` scripts to run GRSPay, this example show how to print the available command line arguments of GRSPay.

On Powershell:
```
.\run.ps1 --help
```

On linux:
```
./run.sh --help
```

## How to debug

If you want to debug, use Visual Studio Code or Visual studio 2017.

You need to run the development time docker-compose as described [in the test guide](BTCPayServer.Tests/README.md).

You can then run the debugger by using the Launch Profile `Docker-Regtest` on either Visual Studio Code or Visual studio 2017.

If you need to debug ledger wallet interaction, install the development time certificate with:

```bash
# Install development time certificate in the trust store
dotnet dev-certs https --trust
```

Then use the `Docker-Regtest-https` debug profile.



## Other dependencies

For more information see the documentation [How to deploy a GRSPay server instance](https://github.com/groestlcoin/btcpayserver-doc/#deployment).
