namespace ORBIT9000.Plugins.Example.DataProviders.Query
{
    internal class WeatherQuery
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public string Hourly { get; }
        public string Timezone { get; }

        public WeatherQuery(double latitude, double longitude, string hourly, string timezone)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Hourly = hourly;
            this.Timezone = timezone;
        }

        public override bool Equals(object? obj)
        {
            return obj is WeatherQuery other &&
                   this.Latitude == other.Latitude &&
                   this.Longitude == other.Longitude &&
                   this.Hourly == other.Hourly &&
                   this.Timezone == other.Timezone;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Latitude, this.Longitude, this.Hourly, this.Timezone);
        }
    }
}