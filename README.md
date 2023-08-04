# GRSPay Server

## Introduction

GRSPay is a free and open-source cryptocurrency payment processor which allows you to receive payments in Groestlcoin, with no fees, transaction cost or a middleman.

GRSPay Server is a non-custodial invoicing system which eliminates the involvement of a third-party. Payments with GRSPay Server go directly to your wallet, which increases the privacy and security. Your private keys are never uploaded to the server. There is no address re-use, since each invoice generates a new address deriving from your xpubkey.

The software is built in C#. It allows for your website to be easily configured as a self-hosted payment processor.

You can run GRSPay Server as a self-hosted solution on your own server, or use a [third-party host](https://docs.btcpayserver.org/ThirdPartyHosting/).

The self-hosted solution allows you not only to attach an unlimited number of stores and use the Lightning Network but also become the payment processor for others.

Thanks to the [apps](https://docs.btcpayserver.org/Apps/) built on top of it, you can use GRSPay to receive donations, start a crowdfunding campaign or have an in-store Point of Sale.

[![What is BTCPay](https://img.youtube.com/vi/q7xJMno_B3U/sddefault.jpg)](https://www.youtube.com/watch?v=q7xJMno_B3U "What is BTCPay")

## Table of Contents

* [Features](#features)
* [How to build](#how-to-build)
* [How to run](#how-to-run)
* [How to debug](#how-to-debug)
* [Dependencies](#other-dependencies)

## 🎨 Features

* Direct, peer-to-peer Groestlcoin payments
* No transaction fees (other than those for the groestlcoin networks)
* No processing fees
* No middleman
* No KYC
* Non-custodial (complete control over the private key)
* Enhanced privacy & security
* Self-hosted
* SegWit support
* Lightning Network support (LND, Core Lightning (CLN), Eclair)
* Tor support
* Process payments for others
* Easy-embeddable Payment buttons
* Point of sale app
* Crowdfunding app
* Payment Requests
* Internal, full-node reliant wallet with [hardware wallet integration](https://docs.btcpayserver.org/Vault/)

## How to build

While the documentation advises to use docker-compose, you may want to build GRSPay yourself.

First, install .NET Core SDK v6.0 as specified by the [Microsoft website](https://dotnet.microsoft.com/download/dotnet-core/6.0).

On Powershell:

```powershell
.\build.ps1
```

On linux:

```sh
./build.sh
```

### How to run

Use the `run` scripts to run GRSPay Server, this example shows how to print the available command-line arguments of GRSPay Server.

On Powershell:

```powershell
.\run.ps1 --help
```

On linux:

```sh
./run.sh --help
```

### How to debug

If you want to debug, use Jetbrain's Rider or Visual Studio 2022.

You need to run the development time docker-compose as described [in the test guide](./BTCPayServer.Tests/README.md).

You can then run the debugger by using the Launch Profile `Docker-Regtest`.

If you need to debug ledger wallet interaction, install the development time certificate with:

```bash
# Install development time certificate in the trust store
dotnet dev-certs https --trust
```

Then use the `Docker-Regtest-https` debug profile.

### Other dependencies

For more information, see the documentation:
[How to deploy a GRSPay Server instance](https://github.com/groestlcoin/btcpayserver-doc/#deployment).

### 🧪 API

GRSPay Server has two APIs:

- **Greenfield API (New)**
  - [Greenfield API documentation](https://docs.btcpayserver.org/API/Greenfield/v1/)
  - [Greenfield API examples with CURL](https://docs.btcpayserver.org/GreenFieldExample/)
- **Legacy API**

The **Greenfield API** is our brand-new API which is still in development. Once complete, it will allow you to run GRSPay Server headlessly.
The **Legacy API**, is fully compatible with [BitPay's API](https://bitpay.com/api/). It has limited features, but allows instant migration from BitPay.

## 📝 License

GRSPay Server software, logo and designs are provided under [MIT License](https://github.com/Groestlcoin/btcpayserver/blob/master/LICENSE).
