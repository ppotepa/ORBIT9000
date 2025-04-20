using Microsoft.Extensions.Logging;
using ORBIT9000.Plugins.Twitter;

namespace ORBIT9000.PoCDemo
{
    internal class TwitterPlugin2 : TwitterPlugin
    {
        public TwitterPlugin2(IServiceProvider provider, ILogger<TwitterPlugin> logger) : base(provider, logger)
        {
        }
    }
}