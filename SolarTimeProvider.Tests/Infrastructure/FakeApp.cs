using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace SolarTimeProvider.Tests.Infrastructure
{
    public class FakeApp : IDisposable
    {
        private readonly WebApplicationFactory<Startup> _appFactory;
        private readonly FakeIpStartupFilter _fakeIpStartupFilter = new FakeIpStartupFilter();

        public HttpClient Client { get; }

        public IPAddress ClientIp
        {
            get => _fakeIpStartupFilter.Ip;
            set => _fakeIpStartupFilter.Ip = value;
        }

        public FakeApp()
        {
            _appFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(o =>
            {
                o.ConfigureServices(s =>
                {
                    s.AddSingleton<IStartupFilter>(_fakeIpStartupFilter);
                });
            });

            Client = _appFactory.CreateClient();
        }

        public void Dispose()
        {
            Client.Dispose();
            _appFactory.Dispose();
        }
    }
}