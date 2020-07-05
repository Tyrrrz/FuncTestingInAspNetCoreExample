using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace SolarTimeProvider.Services
{
    public class CachingLayer
    {
        private readonly IConnectionMultiplexer _redis;

        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public CachingLayer(IConnectionMultiplexer connectionMultiplexer) =>
            _redis = connectionMultiplexer;

        public async Task<T> TryGetAsync<T>(string key) where T : class
        {
            var result = await _redis.GetDatabase().StringGetAsync(key);

            if (result.HasValue)
                return JsonSerializer.Deserialize<T>(result.ToString(), _serializerOptions);

            return null;
        }

        public async Task SetAsync<T>(string key, T obj) where T : class
        {
            await _redis.GetDatabase().StringSetAsync(key, JsonSerializer.Serialize(obj, _serializerOptions));
        }
    }
}