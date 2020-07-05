using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace SolarTimeProvider.Tests.Infrastructure
{
    public class FakeIpStartupFilter : IStartupFilter
    {
        public IPAddress Ip { get; set; } = IPAddress.Parse("::1");

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> nextFilter)
        {
            return app =>
            {
                app.Use(async (ctx, next) =>
                {
                    ctx.Connection.RemoteIpAddress = Ip;
                    await next();
                });

                nextFilter(app);
            };
        }
    }
}