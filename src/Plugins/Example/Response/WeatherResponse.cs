using ORBIT9000.Core.Abstractions.Providers.Data;

namespace ORBIT9000.Plugins.Example.DataProviders
{
    public class WeatherResponse : IResult
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float generationtime_ms { get; set; }
        public int utc_offset_seconds { get; set; }
        public string timezone { get; set; }
        public string timezone_abbreviation { get; set; }
        public float elevation { get; set; }
        public Hourly_Units hourly_units { get; set; }
        public Hourly hourly { get; set; }

        public override string ToString()
        {
            return $"Latitude: {latitude}, Longitude: {longitude}, Generation Time (ms): {generationtime_ms}, " +
                   $"UTC Offset (seconds): {utc_offset_seconds}, Timezone: {timezone}, " +
                   $"Timezone Abbreviation: {timezone_abbreviation}, Elevation: {elevation}, " +
                   $"Hourly Units: [Time: {hourly_units?.time}, Temperature 2m: {hourly_units?.temperature_2m}], " +
                   $"Hourly Data: [Times: {string.Join(", ", hourly?.time ?? Array.Empty<string>())}, " +
                   $"Temperatures: {string.Join(", ", hourly?.temperature_2m ?? Array.Empty<float>())}]";
        }
    }

    public class Hourly_Units
    {
        public string time { get; set; }
        public string temperature_2m { get; set; }
    }

    public class Hourly
    {
        public string[] time { get; set; }
        public float[] temperature_2m { get; set; }
    }

}