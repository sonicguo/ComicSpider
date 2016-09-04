using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abot.Crawler;
using Abot.Poco;

using AbotX;
using AbotX.Crawler;
using AbotX.Core;
using AbotX.Poco;


using HtmlAgilityPack;

using ComicBCL;



namespace ComicCrawler
{
    public class CrawlerDmzj : BasicCrawler
    {
        private DmzjBCLOperator m_BCL = new DmzjBCLOperator();

        public int SiteID
        {
            get
            {
                return m_BCL.SiteInfo.SiteID;
            }
        }

        public string SiteName
        {
            get
            {
                return m_BCL.SiteInfo.SiteName;
            }
        }


        public CrawlerDmzj()
        {

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
                if (pageToCrawl.IsRoot || pageToCrawl.Uri.AbsoluteUri.Trim().StartsWith("http://www.dmzj.com/info/"))   // ignore web page if it's not a Comic
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
        private void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (string.IsNullOrEmpty(crawledPage.Content.Text) || crawledPage.IsRoot)
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
            else
            {
                CrawlerComicInfo(crawledPage);
            }
        }

        private void CrawlerComicInfo(CrawledPage page)
        {
            if (page == null || string.IsNullOrEmpty(page.Content.Text))
            {
                return;
            }
            string uri = page.Uri.ToString();
            int siteID = this.SiteID;
            string description = "";

            Guid comicID = m_BCL.UpdateComic(siteID,uri,description);

            HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page.Content.Text);

            string xPathRoot = "/html[1]/body[1]/div[2]/div[1]/div[1]/div[2]";
            string xPathTitle = "h1[1]/a[1]";
            //string xPathRanking = "div[1]/span[2]/em[1]";
            //string xPathRankingCount = "div[1]/span[2]/em[2]";
            string xPathAuthor = "ul[1]/li[1]";
            string xPathStatus = "ul[1]/li[2]";
            string xPathCategory = "ul[1]/li[3]";
            

            var rootNode = doc.DocumentNode.SelectNodes(xPathRoot).FirstOrDefault();
            if (rootNode != null)
            {
                string title = rootNode.SelectNodes(xPathTitle).FirstOrDefault()?.InnerText;

                string author = rootNode.SelectNodes(xPathAuthor).FirstOrDefault()?.InnerText;
                string status = rootNode.SelectNodes(xPathStatus).FirstOrDefault()?.InnerText;
                string category = rootNode.SelectNodes(xPathCategory).FirstOrDefault()?.InnerText;

                author = string.IsNullOrEmpty(author) ? string.Empty : author.Trim().Substring(3, author.Length - 1 - 3); // 作者：肆叶动漫-涂秋 

                m_BCL.UpdateComicComicDetail(comicID, title, author, !status.Contains("已完结"));

            }

            string xPathChapterRoot = "/html[1]/body[1]/div['wrap autoHeight']/div[1]/div[4]/div[2]/ul[1]";

            var chapterRootNode = doc.DocumentNode.SelectNodes(xPathChapterRoot).FirstOrDefault();

            foreach (var chapterNode in chapterRootNode.ChildNodes)
            {

            }


        }


        #endregion




    }
}
