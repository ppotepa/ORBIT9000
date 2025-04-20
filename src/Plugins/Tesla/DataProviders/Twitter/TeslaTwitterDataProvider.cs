using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Authentication;
using ORBIT9000.Core.Abstractions.Providers.Data;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;

namespace ORBIT9000.Plugins.Tesla.DataProviders.Twitter
{
    [DataProvider]
    [DefaultProject("Tesla")]
    internal class TeslaTwitterDataProvider : IDataProvider<TeslaTwitterResult>, IAuthenticate
    {
        private ILogger<TeslaTwitterDataProvider> _logger;

        public TeslaTwitterDataProvider(ILogger<TeslaTwitterDataProvider> logger)
        {
            this._logger = logger;
        }

        public bool AllowAnonymous => true;

        public bool IsAuthenticated => false;

        public IAuthResult Authenticate()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TeslaTwitterResult> GetData()
        {
            throw new NotImplementedException();
        }
    }
}