using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abot.Crawler;
using Abot.Poco;

using HtmlAgilityPack;

using ComicBCL;



namespace ComicCrawler
{
    public class CrawlerDmzj : BasicCrawler
    {
        private DmzjBCLOperator m_BCL;


        public CrawlerDmzj()
        {
            m_BCL = new DmzjBCLOperator();
        }

        public override void CrawlCategory()
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();
            string URL = m_BCL.SiteInfo.URL + "/category";
            HtmlAgilityPack.HtmlDocument doc = web.Load(URL);

            string xPath = "/html[1]/body[1]/div[2]/div[2]/div[1]/div[2]/div[3]/ul[1]/span/li";

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes(xPath))
            {
                if (link.Name != "li" || link.ChildNodes["a"] == null || link.ChildNodes["a"].Attributes["href"] == null)
                {
                    continue;
                }

                string cateName = link.InnerText;
                string cateURL = link.ChildNodes["a"].Attributes["href"].Value;

                if (!string.IsNullOrEmpty(cateName) && ! string.IsNullOrEmpty(cateURL))
                {
                    cateURL = m_BCL.SiteInfo.URL.Trim() + (cateURL.Trim().StartsWith("/") ? cateURL.Trim() : ("/" + cateURL.Trim()));
                    m_BCL.UpdateCategoary(cateName, cateURL);
                }

            }
        }


        public override void CrawlComicChapterPage()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Crawling from http://www.dmzj.com/category/0-0-0-0-0-0-1.html
        /// </summary>
        public override void CrawlComicChapter()
        {
            PrepareCrawlForChapter();


        }

        #region Private Method -- Web Crawl

        

        private void PrepareCrawlForChapter()
        {
            Uri uriToCrawl = new Uri("http://www.dmzj.com/category/0-0-0-0-0-0-1.html");
            IWebCrawler crawler;

            //Uncomment only one of the following to see that instance in action
            //crawler = GetDefaultWebCrawler();
            crawler = GetManuallyConfiguredWebCrawler();

            //Start the crawl
            //This is a synchronous call
            CrawlResult result = crawler.Crawl(uriToCrawl);

            //System.Threading.Thread.join
        }

        private IWebCrawler GetManuallyConfiguredWebCrawler()
        {
            // Create a config object manually
            CrawlConfiguration config = new CrawlConfiguration();
            config.CrawlTimeoutSeconds = 0;
            config.DownloadableContentTypes = "text/html";
            config.IsExternalPageCrawlingEnabled = false;
            config.IsExternalPageLinksCrawlingEnabled = false;
            config.IsRespectRobotsDotTextEnabled = false;
            config.IsUriRecrawlingEnabled = false;
            config.MaxConcurrentThreads = 1;
            config.MaxPagesToCrawl = 10000;
            config.MaxPagesToCrawlPerDomain = 10000;
            config.MinCrawlDelayPerDomainMilliSeconds = 10000;

            config.MaxCrawlDepth = 1;

            IWebCrawler crawler = new PoliteWebCrawler(config, null, null, null, null, null, null, null, null);

            //Initialize the crawler with custom configuration created above.
            //This override the app.config file values

            // Register a lambda expression that will make Abot not crawl any url that has the word "ghost" in it.
            //For example http://a.com/ghost, would not get crawled if the link were found during the crawl.
            //If you set the log4net log level to "DEBUG" you will see a log message when any page is not allowed to be crawled.
            //NOTE: This is lambda is run after the regular ICrawlDecsionMaker.ShouldCrawlPage method is run.
            //crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            //{
            //    if (pageToCrawl.Uri.AbsoluteUri.Contains("ghost"))
            //        return new CrawlDecision { Allow = false, Reason = "Scared of ghosts" };

            //    return new CrawlDecision { Allow = true };
            //});

            //Register a lambda expression that will tell Abot to not download the page content for any page after 5th.
            //Abot will still make the http request but will not read the raw content from the stream
            //NOTE: This lambda is run after the regular ICrawlDecsionMaker.ShouldDownloadPageContent method is run
            //crawler.ShouldDownloadPageContent((crawledPage, crawlContext) =>
            //{
            //    if (crawlContext.CrawledCount >= 5)
            //        return new CrawlDecision { Allow = false, Reason = "We already downloaded the raw page content for 5 pages" };

            //    return new CrawlDecision { Allow = true };
            //});

            //Register a lambda expression that will tell Abot to not crawl links on any page that is not internal to the root uri.
            //NOTE: This lambda is run after the regular ICrawlDecsionMaker.ShouldCrawlPageLinks method is run
            crawler.ShouldCrawlPageLinks((crawledPage, crawlContext) =>
            {
                if (!crawledPage.IsInternal)
                    return new CrawlDecision { Allow = false, Reason = "We dont crawl links of external pages" };

                return new CrawlDecision { Allow = true };
            });

            crawler.PageCrawlStartingAsync += Crawler_PageCrawlStartingAsync;
            crawler.PageCrawlCompletedAsync += Crawler_PageCrawlCompletedAsync;

            return crawler;

        }


        private void Crawler_PageCrawlStartingAsync(object sender, PageCrawlStartingArgs e)
        {
            //throw new NotImplementedException();
        }
        private void Crawler_PageCrawlCompletedAsync(object sender, PageCrawlCompletedArgs e)
        {
            object o = e.CrawledPage;
        }


        #endregion




    }
}
