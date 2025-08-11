#nullable disable
namespace ORBIT9000.Plugins.Example.Common
{
    internal class WeatherQuery
    {
        public WeatherQuery(double latitude, double longitude, string hourly, string timezone)
        {
            Latitude = latitude;
            Longitude = longitude;
            Hourly = hourly;
            Timezone = timezone;
        }

        public WeatherQuery() { }

        public string Hourly { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timezone { get; set; }

        public override bool Equals(object obj)
        {
            return obj is WeatherQuery other &&
                   Equals((float)Latitude, (float)other.Latitude) &&
                   Equals((float)Longitude, (float)other.Longitude) &&
                   Hourly == other.Hourly &&
                   Timezone == other.Timezone;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Latitude, Longitude, Hourly, Timezone);
        }
    }
}