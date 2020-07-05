using System;

namespace SolarTimeProvider.Models
{
    public class SolarTimes
    {
        public DateTimeOffset Sunrise { get; set; }

        public DateTimeOffset Sunset { get; set; }
    }
}