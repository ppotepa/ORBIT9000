using ORBIT9000.Core.Abstractions.Result;

namespace ORBIT9000.Plugins.Twitter.DataProviders
{
    public class TwitterResult : IResult
    {
        public Double Temperature { get; set; }
        public Double WindSpeed { get; set; }
    }
}