using ORBIT9000.Core.Abstractions.Providers.Data;

namespace ORBIT9000.Plugins.Example.DataProviders
{
    public class Hourly
    {
        public required float[] temperature_2m { get; set; }
        public required string[] time { get; set; }
    }

    public class Hourly_Units
    {
        public required string temperature_2m { get; set; }
        public required string time { get; set; }
    }

    public class WeatherResponse : IResult
    {
        public float elevation { get; set; }
        public float generationtime_ms { get; set; }
        public Hourly? hourly { get; set; }
        public Hourly_Units? hourly_units { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string? timezone { get; set; }
        public string? timezone_abbreviation { get; set; }
        public int utc_offset_seconds { get; set; }
        public override string ToString()
        {
            return $"Location: [Latitude: {latitude}, Longitude: {longitude}], " +
                   $"Generated in: {generationtime_ms} ms, " +
                   $"UTC Offset: {utc_offset_seconds} seconds, " +
                   $"Timezone: {timezone} ({timezone_abbreviation}), " +
                   $"Elevation: {elevation} meters, " +
                   $"Hourly Units: [Time Unit: {hourly_units?.time}, " +
                   $"Temperature Unit: {hourly_units?.temperature_2m}], " +
                   $"Hourly Data: [Times: {string.Join(", ", hourly?.time ?? Array.Empty<string>())}, " +
                   $"Temperatures: {string.Join(", ", hourly?.temperature_2m ?? Array.Empty<float>())}]";
        }
    }
}