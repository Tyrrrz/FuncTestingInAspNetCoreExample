using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SolarTimeProvider.Models;

namespace SolarTimeProvider.Services
{
    public class LocationProvider
    {
        private readonly HttpClient _httpClient;

        public LocationProvider(HttpClient httpClient) =>
            _httpClient = httpClient;

        public async Task<Location> GetLocationAsync(IPAddress ip)
        {
            var ipFormatted = !ip.IsLocal() ? ip.MapToIPv4().ToString() : "";
            var json = await _httpClient.GetJsonAsync($"http://ip-api.com/json/{ipFormatted}");

            var latitude = json.GetProperty("lat").GetDouble();
            var longitude = json.GetProperty("lon").GetDouble();

            return new Location
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }
    }
}