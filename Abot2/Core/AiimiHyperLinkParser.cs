using Abot2.Core;
using Abot2.Poco;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InsightMaker.Source.Web
{
    /// <summary>
    /// An extension of the Abot AngleSharpHyperLinkParser. 
    /// Built for IM-4302, an issue where href values that included a '////' between the scheme and
    /// the url would not be cralwed properly. Overrides the GetUris function to add a check for instances
    /// of this issue and resolve before attempting to crawl.
    /// </summary>
    public class AiimiHyperLinkParser : AngleSharpHyperlinkParser
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Constructors

        public AiimiHyperLinkParser(CrawlConfiguration config, Func<string, string> cleanURLFunc)
        {
            Config = config;
            CleanUrlFunc = cleanURLFunc;
        }

        #endregion

        #region Abstract
        /// <summary>
        /// The name of the parser
        /// </summary>
        protected override string ParserType
        {
            get { return "AiimiHyperLinkParser"; }
        }
        #endregion

        /// <summary>
        /// Gets the Uris from the page inside certain href tags
        /// </summary>
        /// <param name="crawledPage"></param>
        /// <param name="hrefValues"></param>
        /// <returns>A collection of uris on the page</returns>
        protected override List<Uri> GetUris(CrawledPage crawledPage, IEnumerable<string> hrefValues)
        {
            var uris = new List<Uri>();
            if (hrefValues == null || hrefValues.Count() < 1)
                return uris;

            //Use the uri of the page that actually responded to the request instead of crawledPage.Uri (Issue 82).
            var uriToUse = crawledPage.HttpResponseMessage.RequestMessage.RequestUri ?? crawledPage.Uri;

            //If html base tag exists use it instead of page uri for relative links
            var baseHref = GetBaseHrefValue(crawledPage);
            if (!string.IsNullOrEmpty(baseHref))
            {
                if (baseHref.StartsWith("//"))
                    baseHref = crawledPage.Uri.Scheme + ":" + baseHref;

                try
                {
                    uriToUse = new Uri(baseHref);
                }
                catch
                {
                    _logger.Error($"Failed to parse {crawledPage.Uri} as {baseHref}");
                }
            }

            var href = "";
            foreach (string hrefValue in hrefValues)
            {
                try
                {                    
                    // Remove the url fragment part of the url if needed.
                    // This is the part after the # and is often not useful.
                    href = Config.IsRespectUrlNamedAnchorOrHashbangEnabled
                        ? hrefValue
                        : hrefValue.Split('#')[0];

                    // IM-4302
                    if (href.StartsWith("http:////") || href.StartsWith("https:////") || href.StartsWith("////"))
                        href = crawledPage.Uri.Scheme + "://" + href.Split(new string[] { "////" }, StringSplitOptions.None)[1];

                    var newUri = new Uri(uriToUse, href);

                    if (CleanUrlFunc != null)
                        newUri = new Uri(CleanUrlFunc(newUri.AbsoluteUri));

                    if (!uris.Exists(u => u.AbsoluteUri == newUri.AbsoluteUri))
                        uris.Add(newUri);
                }
                catch (Exception e)
                {
                    _logger.DebugFormat("Could not parse link [{0}] on page [{1}].", hrefValue, crawledPage.Uri);
                    _logger.Debug(e);
                }
            }

            return uris;
        }
    }

}
