#nullable disable
namespace ORBIT9000.Plugins.Example.Query
{
    internal class WeatherQuery
    {
        public WeatherQuery(double latitude, double longitude, string hourly, string timezone)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Hourly = hourly;
            this.Timezone = timezone;
        }

        public WeatherQuery() { }

        public string Hourly { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timezone { get; set; }

        public override bool Equals(object obj)
        {
            return obj is WeatherQuery other &&
                   Equals((float)this.Latitude, (float)other.Latitude) &&
                   Equals((float)this.Longitude, (float)other.Longitude) &&
                   this.Hourly == other.Hourly &&
                   this.Timezone == other.Timezone;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Latitude, this.Longitude, this.Hourly, this.Timezone);
        }
    }
}