using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Extensions;
using SolarTimeProvider.Models;
using SolarTimeProvider.Services;
using SolarTimeProvider.Tests.Infrastructure;
using Xunit;

namespace SolarTimeProvider.Tests
{
    public class SolarTimeSpecs : IClassFixture<RedisFixture>, IAsyncLifetime
    {
        private readonly RedisFixture _redisFixture;

        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public SolarTimeSpecs(RedisFixture redisFixture)
        {
            _redisFixture = redisFixture;
        }

        public async Task InitializeAsync() => await _redisFixture.ResetAsync();

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task User_can_get_solar_times_for_their_location_by_ip()
        {
            // Arrange
            using var app = new FakeApp
            {
                ClientIp = IPAddress.Parse("20.112.101.1")
            };

            var date = new DateTimeOffset(2020, 07, 03, 0, 0, 0, TimeSpan.FromHours(-5));
            var expectedSunrise = new DateTimeOffset(2020, 07, 03, 05, 20, 37, TimeSpan.FromHours(-5));
            var expectedSunset = new DateTimeOffset(2020, 07, 03, 20, 28, 54, TimeSpan.FromHours(-5));

            // Act
            var query = new QueryBuilder
            {
                {"date", date.ToString("O", CultureInfo.InvariantCulture)}
            };

            var response = await app.Client.GetStringAsync($"/solartimes/by_ip{query}");
            var solarTimes = JsonSerializer.Deserialize<SolarTimes>(response, _serializerOptions);

            // Assert
            solarTimes.Sunrise.Should().BeCloseTo(expectedSunrise, TimeSpan.FromSeconds(1));
            solarTimes.Sunset.Should().BeCloseTo(expectedSunset, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task User_can_get_solar_times_for_their_location_by_ip_multiple_times_with_the_same_result()
        {
            // Arrange
            using var app = new FakeApp();

            // Act
            var collectedSolarTimes = new List<SolarTimes>();

            for (var i = 0; i < 3; i++)
            {
                var response = await app.Client.GetStringAsync("/solartimes/by_ip");
                var solarTimes = JsonSerializer.Deserialize<SolarTimes>(response, _serializerOptions);

                collectedSolarTimes.Add(solarTimes);
            }

            // Assert
            collectedSolarTimes.Select(t => t.Sunrise).Distinct().Should().ContainSingle();
            collectedSolarTimes.Select(t => t.Sunset).Distinct().Should().ContainSingle();
        }

        [Fact]
        public async Task User_can_get_solar_times_for_a_specific_location_and_date()
        {
            // Arrange
            using var app = new FakeApp();

            var date = new DateTimeOffset(2020, 07, 03, 0, 0, 0, TimeSpan.FromHours(+3));
            var expectedSunrise = new DateTimeOffset(2020, 07, 03, 04, 52, 23, TimeSpan.FromHours(+3));
            var expectedSunset = new DateTimeOffset(2020, 07, 03, 21, 11, 45, TimeSpan.FromHours(+3));

            // Act
            var query = new QueryBuilder
            {
                {"lat", "50.45"},
                {"lon", "30.52"},
                {"date", date.ToString("O", CultureInfo.InvariantCulture)}
            };

            var response = await app.Client.GetStringAsync($"/solartimes/by_location{query}");
            var solarTimes = JsonSerializer.Deserialize<SolarTimes>(response, _serializerOptions);

            // Assert
            solarTimes.Sunrise.Should().BeCloseTo(expectedSunrise, TimeSpan.FromSeconds(1));
            solarTimes.Sunset.Should().BeCloseTo(expectedSunset, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void User_can_get_solar_times_for_New_York_in_November()
        {
            // Arrange
            var location = new Location
            {
                Latitude = 40.71,
                Longitude = -74.00
            };

            var date = new DateTimeOffset(2019, 11, 04, 00, 00, 00, TimeSpan.FromHours(-5));
            var expectedSunrise = new DateTimeOffset(2019, 11, 04, 06, 29, 34, TimeSpan.FromHours(-5));
            var expectedSunset = new DateTimeOffset(2019, 11, 04, 16, 49, 04, TimeSpan.FromHours(-5));

            // Act
            var solarTimes = new SolarCalculator().GetSolarTimes(location, date);

            // Assert
            solarTimes.Sunrise.Should().BeCloseTo(expectedSunrise, TimeSpan.FromSeconds(1));
            solarTimes.Sunset.Should().BeCloseTo(expectedSunset, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void User_can_get_solar_times_for_Tromso_in_January()
        {
            // Arrange
            var location = new Location
            {
                Latitude = 69.65,
                Longitude = 18.96
            };

            var date = new DateTimeOffset(2020, 01, 03, 00, 00, 00, TimeSpan.FromHours(+1));
            var expectedSunrise = new DateTimeOffset(2020, 01, 03, 11, 48, 31, TimeSpan.FromHours(+1));
            var expectedSunset = new DateTimeOffset(2020, 01, 03, 11, 48, 45, TimeSpan.FromHours(+1));

            // Act
            var solarTimes = new SolarCalculator().GetSolarTimes(location, date);

            // Assert
            solarTimes.Sunrise.Should().BeCloseTo(expectedSunrise, TimeSpan.FromSeconds(1));
            solarTimes.Sunset.Should().BeCloseTo(expectedSunset, TimeSpan.FromSeconds(1));
        }
    }
}