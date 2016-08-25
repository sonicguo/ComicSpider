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


        public override void CrawlComic()
        {
            PrepareCrawlForChapter();
        }
        /// <summary>
        /// Crawling from http://www.dmzj.com/category/0-0-0-0-0-0-1.html
        /// </summary>
        public override void CrawlComicChapter()
        {
            //PrepareCrawlForChapter();


        }

        #region Private Method -- Web Crawl

        

        private void PrepareCrawlForChapter()
        {
            PrintDisclaimer();
            foreach (var uriToCrawl in m_BCL.GetCategoryIndexerUri())
            {
                IWebCrawler crawler;
                crawler = GetManuallyConfiguredWebCrawler();
                //This is a synchronous call
                CrawlResult result = crawler.Crawl(uriToCrawl);
            }
            PrintDisclaimer();
        }

        private static void PrintDisclaimer()
        {
            PrintAttentionText("The demo is configured to only crawl a total of 10 pages and will wait 1 second in between http requests. This is to avoid getting you blocked by your isp or the sites you are trying to crawl. You can change these values in the app.config or Abot.Console.exe.config file.");
        }

        private static void PrintAttentionText(string text)
        {
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(text);
            System.Console.ForegroundColor = originalColor;
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
            config.MaxConcurrentThreads = 10;
            config.MaxPagesToCrawl = 10000;
            config.MaxPagesToCrawlPerDomain = 10000;
            config.MinCrawlDelayPerDomainMilliSeconds = 10000;

            config.MaxCrawlDepth = 2;

            IWebCrawler crawler = new PoliteWebCrawler(config, null, null, null, null, null, null, null, null);

            //Register a lambda expression that will make Abot not crawl any url that has the word "ghost" in it.
            //For example http://a.com/ghost, would not get crawled if the link were found during the crawl.
            //If you set the log4net log level to "DEBUG" you will see a log message when any page is not allowed to be crawled.
            //NOTE: This is lambda is run after the regular ICrawlDecsionMaker.ShouldCrawlPage method is run.
            crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            {
                if (pageToCrawl.IsRoot || pageToCrawl.Uri.AbsoluteUri.Trim().StartsWith("http://www.dmzj.com/info/"))
                    return new CrawlDecision { Allow = true, Reason = "crawling page indexer and comic link only" };

                return new CrawlDecision { Allow = false, Reason = "Pass through any other LINKs" };
            });

            crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;


            return crawler;
        }

        private void Crawler_PageCrawlStartingAsync(object sender, PageCrawlStartingArgs e)
        {
            //throw new NotImplementedException();
        }
        static void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
            else
            {
                Console.WriteLine("Crawled Page succeeded  -- ", crawledPage.Uri.AbsoluteUri);
                string content = crawledPage.Content.Text;
            }
        }


        #endregion




    }
}
