using Abot2.Core;
using Abot2.Poco;
using System.Net;

namespace InsightMaker.Source.Web
{
    public class CookieLoadedPageRequester : PageRequester
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CookieLoadedPageRequester(CrawlConfiguration config) 
            : base(config,null)
        {

        }

        public CookieLoadedPageRequester(CrawlConfiguration config, IWebContentExtractor contentExtractor) 
            : base(config, contentExtractor)
        {

        }

        public CookieLoadedPageRequester(CrawlConfiguration config, IWebContentExtractor contentExtractor, CookieContainer cookieContainer)
            : base(config, contentExtractor)
        {
            this._cookieContainer = cookieContainer;
        }
    }
}
