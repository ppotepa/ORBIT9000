namespace ORBIT9000.Plugins.Example.Common
{
    public class HourlyData
    {
        public float[]? Temperature2M { get; set; }
        public string[]? Time { get; set; }
    }

    public class HourlyUnitsData
    {
        public string? Temperature2M { get; set; }
        public string? Time { get; set; }
    }

    public class WeatherResponse
    {
        public float Elevation { get; set; }
        public float GenerationTimeMs { get; set; }
        public HourlyData? Hourly { get; set; }
        public HourlyUnitsData? HourlyUnits { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string? Timezone { get; set; }
        public string? TimezoneAbbreviation { get; set; }
        public int UtcOffsetSeconds { get; set; }

        public override string ToString()
        {
            return $"Location: [Latitude: {Latitude}, Longitude: {Longitude}], " +
                   $"Generated in: {GenerationTimeMs} ms, " +
                   $"UTC Offset: {UtcOffsetSeconds} seconds, " +
                   $"Timezone: {Timezone} ({TimezoneAbbreviation}), " +
                   $"Elevation: {Elevation} meters, " +
                   $"Hourly Units: [Time Unit: {HourlyUnits?.Time}, " +
                   $"Temperature Unit: {HourlyUnits?.Temperature2M}], " +
                   $"Hourly Data: [Times: {string.Join(", ", Hourly?.Time ?? [])}, " +
                   $"Temperatures: {string.Join(", ", Hourly?.Temperature2M ?? [])}]";
        }
    }
}