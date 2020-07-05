using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SolarTimeProvider.Services
{
    internal static class Extensions
    {
        public static DateTimeOffset ResetTimeOfDay(this DateTimeOffset dateTime) =>
            new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTime.Offset);

        public static bool IsLocal(this IPAddress ip)
        {
            var components = ip.MapToIPv4().GetAddressBytes();

            if (components.Length >= 4)
            {
                if (components[0] == 0)
                    return true;

                if (components[0] == 10)
                    return true;

                if (components[0] == 172)
                    return components[1] >= 16 && components[1] <= 31;

                if (components[0] == 192)
                    return components[1] == 168;
            }

            return false;
        }

        public static async Task<JsonElement> ReadAsJsonAsync(this HttpContent content)
        {
            await using var stream = await content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            return doc.RootElement.Clone();
        }

        public static async Task<JsonElement> GetJsonAsync(this HttpClient httpClient, string uri)
        {
            using var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            return await response.Content.ReadAsJsonAsync();
        }
    }
}