using System;
using System.Reflection;
using System.Threading.Tasks;
using Abot2.Core;
using Abot2.Crawler;
using Abot2.Poco;
using log4net;

namespace Abot2.Demo
{
    public class Program
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static async Task Main(string[] args)
        {
            Log.Info("Demo starting up!");

            //await DemoPageRequester();
            await DemoSimpleCrawler();

            Log.Info("Demo done!");
            Console.ReadKey();
        }

        private static async Task DemoSimpleCrawler()
        {
            var config = new CrawlConfiguration
            {
                MaxPagesToCrawl = 25,
                MinCrawlDelayPerDomainMilliSeconds = 3000
            };
            var crawler = new PoliteWebCrawler(config);

            crawler.PageCrawlCompleted += Crawler_PageCrawlCompleted;

            var crawlResult = await crawler.CrawlAsync(new Uri("http://wvtesting2.com"));
        }

        private static void Crawler_PageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            
        }

        private static async Task DemoPageRequester()
        {
            var pageRequester =
                new PageRequester(new CrawlConfiguration(), new WebContentExtractor());

            //var result = await pageRequester.MakeRequestAsync(new Uri("http://google.com"));
            var result = await pageRequester.MakeRequestAsync(new Uri("http://wvtesting2.com"));
            Log.DebugFormat("{0}", new { url = result.Uri, status = Convert.ToInt32(result.HttpResponseMessage.StatusCode) });

        }
    }
}
