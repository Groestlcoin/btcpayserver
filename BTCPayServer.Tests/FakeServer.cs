﻿using System.Threading.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BTCPayServer.Tests
{
    public class FakeServer : IDisposable
    {
        IWebHost webHost;
        SemaphoreSlim semaphore;
        CancellationTokenSource cts = new CancellationTokenSource();
        public FakeServer()
        {
            _channel = Channel.CreateUnbounded<HttpContext>();
            semaphore = new SemaphoreSlim(0);
        }

        Channel<HttpContext> _channel;
        public async Task Start()
        {
            webHost = new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls("http://127.0.0.1:0")
                    .Configure(appBuilder =>
                    {
                        appBuilder.Run(async ctx =>
                        {
                            await _channel.Writer.WriteAsync(ctx);
                            await semaphore.WaitAsync(cts.Token);
                        });
                    })
                    .Build();
            await webHost.StartAsync();
            var port = new Uri(webHost.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First(), UriKind.Absolute)
                .Port;
            ServerUri = new Uri($"http://127.0.0.1:{port}/");
        }

        public Uri ServerUri { get; set; }

        public void Done()
        {
            semaphore.Release();
        }

        public async Task Stop()
        {
            await webHost.StopAsync();
        }
        public void Dispose()
        {
            cts.Dispose();
            webHost?.Dispose();
            semaphore.Dispose();
        }

        public async Task<HttpContext> GetNextRequest()
        {
            return await _channel.Reader.ReadAsync();
        }
    }
}
