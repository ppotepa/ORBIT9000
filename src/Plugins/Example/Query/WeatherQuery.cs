namespace ORBIT9000.Plugins.Example.DataProviders.Query
{
    internal class WeatherQuery
    {
        public double latitude { get; set; }
        public double Longitude { get; set; }
        public string Hourly { get; set; }
        public string Timezone { get; set; }

        public WeatherQuery(double latitude, double longitude, string hourly, string timezone)
        {
            this.latitude = latitude;
            this.Longitude = longitude;
            this.Hourly = hourly;
            this.Timezone = timezone;
        }

        public WeatherQuery() { }

        public override bool Equals(object? obj)
        {
            return obj is WeatherQuery other &&
                   this.latitude == other.latitude &&
                   this.Longitude == other.Longitude &&
                   this.Hourly == other.Hourly &&
                   this.Timezone == other.Timezone;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.latitude, this.Longitude, this.Hourly, this.Timezone);
        }
    }
}