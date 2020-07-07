using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace jaytwo.AspNetCore.DataProtection.Tests
{
    public class SampleAppServerContext : IDisposable
    {
        private readonly string _url;
        private readonly IHost _webHost;

        private SampleAppServerContext(string url, IHost webHost)
        {
            _url = url;
            _webHost = webHost;
        }

        public string Url => _url;

        public static SampleAppServerContext Create(string staticKey)
        {
            var webHostBuilder = SampleApp.Program.CreateHostBuilder(new string[] { "--staticKey", staticKey });

            var port = GetRandomFreePort();
            var url = $"http://localhost:{port}";
            var webHost = webHostBuilder
                .ConfigureWebHost(x => x.UseUrls(url))
                .Build();

            try
            {
                webHost.Start();
                return new SampleAppServerContext(url, webHost);
            }
            catch
            {
                webHost.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            _webHost.Dispose();
        }

        private static int GetRandomFreePort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }
    }
}
