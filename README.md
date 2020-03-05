![GRSPay Server](BTCPayServer/wwwroot/img/btc_pay_BG_twitter.png)

[![Docker Automated build](https://img.shields.io/docker/automated/jrottenberg/ffmpeg.svg)](https://hub.docker.com/r/btcpayserver/btcpayserver/)
[![Deploy to Azure](https://azuredeploy.net/deploybutton.svg)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fbtcpayserver%2Fbtcpayserver-azure%2Fmaster%2Fazuredeploy.json)
[![CircleCI](https://circleci.com/gh/btcpayserver/btcpayserver.svg?style=svg)](https://circleci.com/gh/btcpayserver/btcpayserver)

# GRSPay Server

## Introduction

GRSPay is a free and open-source cryptocurrency payment processor which allows you to receive payments in Groestlcoin, with no fees, transaction cost or a middleman.

GRSPay Server is a non-custodial invoicing system which eliminates the involvement of a third-party. Payments with GRSPay Server go directly to your wallet, which increases the privacy and security. Your private keys are never uploaded to the server. There is no address re-use, since each invoice generates a new address deriving from your xpubkey.

The software is built in C#. It allows for your website to be easily configured as a self-hosted payment processor.

You can run GRSPay Server as a self-hosted solution on your own server, or use a [third-party host](https://github.com/btcpayserver/btcpayserver-doc/blob/master/ThirdPartyHosting.md).

The self-hosted solution allows you not only to attach an unlimited number of stores and use the Lightning Network but also become the payment processor for others.

Thanks to the [apps](https://github.com/btcpayserver/btcpayserver-doc/blob/master/Apps.md) built on top of it, you can use GRSPay to receive donations, start a crowdfunding campaign or have an in-store Point of Sale.

[![What is BTCPay](https://img.youtube.com/vi/q7xJMno_B3U/sddefault.jpg)](https://www.youtube.com/watch?v=q7xJMno_B3U "What is BTCPay")

## Table of Contents

* [Features](#features)
* [Getting Started](#getting-started)
* [Documentation](#documentation)
* [Contributing](#Contributing)
* [How to build](#how-to-build)
* [How to run](#how-to-run)
* [How to debug](#how-to-debug)
* [Dependencies](#other-dependencies)

## Features

* Direct, peer-to-peer Groestlcoin payments
* No transaction fees (other than those for the groestlcoin networks)
* No processing fees
* No middleman
* No KYC
* Non-custodial (complete control over the private key)
* Enhanced privacy
* Enhanced security
* Self-hosted
* SegWit support
* Lightning Network support (LND, c-lightning, Eclair and Ptarmigan)
* Tor support
* Process payments for others
* Easy-embeddable Payment buttons
* Point of sale app
* Crowdfunding app
* Payment Requests
* Internal, full-node reliant wallet with [hardware wallet integration](https://github.com/btcpayserver/btcpayserver-doc/blob/master/Vault.md)

## How to build

While the documentation advises to use docker-compose, you may want to build GRSPay yourself.

First install .NET Core SDK v3.1 as specified by [Microsoft website](https://dotnet.microsoft.com/download/dotnet-core/3.1).

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

If you want to debug, use Visual Studio Code or Visual Studio 2019.

You need to run the development time docker-compose as described [in the test guide](BTCPayServer.Tests/README.md).

You can then run the debugger by using the Launch Profile `Docker-Regtest` on either Visual Studio Code or Visual Studio 2017.

If you need to debug ledger wallet interaction, install the development time certificate with:

```bash
# Install development time certificate in the trust store
dotnet dev-certs https --trust
```

Then use the `Docker-Regtest-https` debug profile.

## Other dependencies

For more information see the documentation [How to deploy a GRSPay server instance](https://github.com/groestlcoin/btcpayserver-doc/#deployment).
