using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SolarTimeProvider.Models;
using SolarTimeProvider.Services;

namespace SolarTimeProvider.Controllers
{
    [ApiController]
    [Route("solartimes")]
    public class SolarTimeController : ControllerBase
    {
        private readonly SolarCalculator _solarCalculator;
        private readonly LocationProvider _locationProvider;
        private readonly CachingLayer _cachingLayer;

        public SolarTimeController(
            SolarCalculator solarCalculator,
            LocationProvider locationProvider,
            CachingLayer cachingLayer)
        {
            _solarCalculator = solarCalculator;
            _locationProvider = locationProvider;
            _cachingLayer = cachingLayer;
        }

        [HttpGet("by_ip")]
        public async Task<IActionResult> GetByIp(DateTimeOffset? date)
        {
            var ip = HttpContext.Connection.RemoteIpAddress;
            var cacheKey = $"{ip},{date}";

            var cachedSolarTimes = await _cachingLayer.TryGetAsync<SolarTimes>(cacheKey);
            if (cachedSolarTimes != null)
                return Ok(cachedSolarTimes);

            var location = await _locationProvider.GetLocationAsync(ip);
            var solarTimes = _solarCalculator.GetSolarTimes(location, date ?? DateTimeOffset.Now);
            await _cachingLayer.SetAsync(cacheKey, solarTimes);

            return Ok(solarTimes);
        }

        [HttpGet("by_location")]
        public async Task<IActionResult> GetByLocation(double lat, double lon, DateTimeOffset? date)
        {
            var location = new Location
            {
                Latitude = lat,
                Longitude = lon
            };

            var cacheKey = $"{lat},{lon},{date}";

            var cachedSolarTimes = await _cachingLayer.TryGetAsync<SolarTimes>(cacheKey);
            if (cachedSolarTimes != null)
                return Ok(cachedSolarTimes);

            var solarTimes = _solarCalculator.GetSolarTimes(location, date ?? DateTimeOffset.Now);
            await _cachingLayer.SetAsync(cacheKey, solarTimes);

            return Ok(solarTimes);
        }
    }
}