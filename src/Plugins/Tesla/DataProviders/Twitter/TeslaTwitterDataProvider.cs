using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Authentication;
using ORBIT9000.Core.Abstractions.Data;
using ORBIT9000.Core.Attributes;

namespace ORBIT9000.Plugins.Tesla.Scrapers.Twitter
{
    [DefaultProject("Tesla")]
    internal class TeslaTwitterDataProvider : IDataProvider<TeslaTwitterResult>, IAuthenticate
    {
        private ILogger _logger;

        public TeslaTwitterDataProvider(ILogger logger)
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