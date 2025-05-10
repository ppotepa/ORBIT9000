using ORBIT9000.Core.Abstractions.Providers.Data;

namespace ORBIT9000.Plugins.ScheduleExample2.Response
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

    public class WeatherResponse : IResult
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
            return $"Location: [Latitude: {this.Latitude}, Longitude: {this.Longitude}], " +
                   $"Generated in: {this.GenerationTimeMs} ms, " +
                   $"UTC Offset: {this.UtcOffsetSeconds} seconds, " +
                   $"Timezone: {this.Timezone} ({this.TimezoneAbbreviation}), " +
                   $"Elevation: {this.Elevation} meters, " +
                   $"Hourly Units: [Time Unit: {this.HourlyUnits?.Time}, " +
                   $"Temperature Unit: {this.HourlyUnits?.Temperature2M}], " +
                   $"Hourly Data: [Times: {string.Join(", ", this.Hourly?.Time ?? [])}, " +
                   $"Temperatures: {string.Join(", ", this.Hourly?.Temperature2M ?? [])}]";
        }
    }
}