using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Authentication;
using ORBIT9000.Core.Scrapers;

namespace ORBIT9000.Plugins.Tesla.Scrapers.Twitter
{
    [DefaultProject("Tesla")]
    internal class TeslaTwitterScraper : 
        IScraper<TeslaTwitterResult>,
        IAuthenticate
    {
        private ILogger _logger;

        public TeslaTwitterScraper(ILogger logger)
        {
            this._logger = logger;
        }

        public bool AllowAnonymous => true;

        public bool IsAuthenticated => false;

        public IAuthResult Authenticate()
        {
            throw new NotImplementedException();
        }

        public TeslaTwitterResult Scrape()
        {
            throw new NotImplementedException();
        }
    }
}