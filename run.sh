#!/bin/bash

dotnet run --no-launch-profile --no-build -c Altcoins-Release --project "BTCPayServer/BTCPayServer.csproj" -- $@
