using ORBIT9000.Core.Abstractions.Data.Entities;

namespace ORBIT9000.ExampleDomain.Entities
{
    public class WeatherData : ExtendedEntity<Guid>
    {
        public decimal? Temperature { get; set; }
        public string? City { get; set; }
        public float? Long { get; set; }
        public float? Lat { get; set; }
    }
}