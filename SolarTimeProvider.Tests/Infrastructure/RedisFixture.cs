using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Xunit;

namespace SolarTimeProvider.Tests.Infrastructure
{
    public class RedisFixture : IAsyncLifetime
    {
        private string _containerId;

        public async Task InitializeAsync()
        {
            var result = await Cli.Wrap("docker")
                .WithArguments("run -d -p 6379:6379 redis")
                .ExecuteBufferedAsync();

            _containerId = result.StandardOutput.Trim();
        }

        public async Task ResetAsync() =>
            await Cli.Wrap("docker")
                .WithArguments($"exec {_containerId} redis-cli FLUSHALL")
                .ExecuteAsync();

        public async Task DisposeAsync() =>
            await Cli.Wrap("docker")
                .WithArguments($"container kill {_containerId}")
                .ExecuteAsync();
    }
}