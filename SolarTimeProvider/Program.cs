using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SolarTimeProvider
{
    public class Program
    {
        private static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(o => o.UseStartup<Startup>());

        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();
    }
}