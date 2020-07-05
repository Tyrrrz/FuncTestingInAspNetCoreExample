using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolarTimeProvider.Services;
using StackExchange.Redis;

namespace SolarTimeProvider
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) =>
            _configuration = configuration;

        private string GetRedisConnectionString() =>
            _configuration.GetConnectionString("Redis");

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o => o.EnableEndpointRouting = false);

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(GetRedisConnectionString()));
            services.AddSingleton<CachingLayer>();

            services.AddHttpClient<LocationProvider>();
            services.AddTransient<SolarCalculator>();
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvcWithDefaultRoute();
        }
    }
}