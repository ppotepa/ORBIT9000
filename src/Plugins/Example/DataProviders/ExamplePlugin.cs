using ORBIT9000.Core.Abstractions.Providers.Data;

namespace ORBIT9000.Plugins.Example.DataProviders
{
    public class ExamplePlugin : IResult
    {
        public Double Temperature { get; set; }
        public Double WindSpeed { get; set; }
    }
}