using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abot.Crawler;
using Abot.Poco;

//using AbotX;
//using AbotX.Crawler;
//using AbotX.Core;
//using AbotX.Poco;

using HtmlAgilityPack;

using ComicBCL;
using log4net;

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
            try
            {


                foreach (HtmlNode link in doc.DocumentNode.SelectNodes(xPath))
                {
                    if (link.Name != "li" || link.ChildNodes["a"] == null || link.ChildNodes["a"].Attributes["href"] == null)
                    {
                        continue;
                    }

                    string cateName = link.InnerText;
                    string cateURL = link.ChildNodes["a"].Attributes["href"].Value;

                    if (!string.IsNullOrEmpty(cateName) && !string.IsNullOrEmpty(cateURL))
                    {
                        cateURL = m_BCL.SiteInfo.URL.Trim() + (cateURL.Trim().StartsWith("/") ? cateURL.Trim() : ("/" + cateURL.Trim()));
                        m_BCL.UpdateCategoary(cateName, cateURL);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public override void CrawlComicChapterPage()
        {
            throw new NotImplementedException();
        }







    

        #region Private Method -- Web Crawl




        private static void PrintAttentionText(string text)
        {
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(text);
            System.Console.ForegroundColor = originalColor;
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
                CrawlerComic(crawledPage);
            }
        }

        private void CrawlerComic(CrawledPage page)
        {
            if (page == null || string.IsNullOrEmpty(page.Content.Text))
            {
                return;
            }
            string uri = page.Uri.ToString();
            int siteID = this.SiteID;
            string description = "";

            try
            {

                Guid comicID = m_BCL.UpdateComic(siteID, uri, description);

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

                    author = string.IsNullOrEmpty(author) ? string.Empty : author.Trim().Replace("作者：", ""); // 作者：肆叶动漫-涂秋 

                    m_BCL.UpdateComicComicDetail(comicID, title, author, !status.Contains("已完结"));

                }

                string xPathChapterRoot = "/html[1]/body[1]/div['wrap autoHeight']/div[1]/div[4]/div[2]/ul[1]";

                var chapterRootNode = doc.DocumentNode.SelectNodes(xPathChapterRoot).FirstOrDefault();
                if (chapterRootNode != null && chapterRootNode.ChildNodes != null)
                {
                    var chapterNodes = chapterRootNode.SelectNodes("li");
                    if (chapterNodes != null)
                    {
                        foreach (var chapterNode in chapterRootNode.SelectNodes("li"))
                        {
                            if (chapterNode == null) continue;
                            if (chapterNode.SelectNodes("a") == null || chapterNode.SelectNodes("a")[0] == null) continue;


                            string chapterName = chapterNode.SelectNodes("a")[0].ChildNodes[1].InnerText;    // "第76话"
                            string chapterURL = chapterNode
                                ?.SelectNodes("a")[0]
                                ?.Attributes["href"]
                                ?.Value;    // "http://www.dmzj.com/view/xianxiashijie/56676.html" 
                            string chapterDesc = chapterNode
                                ?.SelectNodes("a")[0]
                                ?.Attributes["title"]
                                ?.Value; // "仙侠世界第76话 2016-09-02" 
                            int totalPage = -1;

                            if (string.IsNullOrEmpty(chapterName) || string.IsNullOrEmpty(chapterURL)) { continue; }

                            m_BCL.UpdateChapter(comicID, chapterName, chapterURL, chapterDesc, totalPage);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        private static void PrintDisclaimer()
        {
            PrintAttentionText("The demo is configured to only crawl a total of 10 pages and will wait 1 second in between http requests. This is to avoid getting you blocked by your isp or the sites you are trying to crawl. You can change these values in the app.config or Abot.Console.exe.config file.");
        }

        #endregion


        #region "Crawling Comic"

        public override void CrawlComic()
        {
            //PrepareCrawlForChapter();

            //CrawComicWithHtmlAgilityPack();

            PrepareCrawlForComic();
        }

        private void PrepareCrawlForComic()
        {
            PrintDisclaimer();
            foreach (var uriToCrawl in m_BCL.GetCategoryIndexerUri())
            {
                IWebCrawler crawler;
                crawler = GetManuallyConfiguredWebCrawlerForComic();
                //This is a synchronous call
                CrawlResult result = crawler.Crawl(uriToCrawl);
            }
            PrintDisclaimer();
        }
        private IWebCrawler GetManuallyConfiguredWebCrawlerForComic()
        {

            // Create a config object manually
            CrawlConfiguration config = new CrawlConfiguration();
            config.CrawlTimeoutSeconds = 0;
            config.DownloadableContentTypes = "text/html";
            config.IsExternalPageCrawlingEnabled = false;
            config.IsExternalPageLinksCrawlingEnabled = false;
            config.IsRespectRobotsDotTextEnabled = false;
            config.IsUriRecrawlingEnabled = false;
            config.MaxConcurrentThreads = 5;
            config.MaxPagesToCrawl = 0;
            config.MaxPagesToCrawlPerDomain = 0;
            config.MinCrawlDelayPerDomainMilliSeconds = 30000;


            config.MaxCrawlDepth = 0;

            IWebCrawler crawler = new PoliteWebCrawler(config, null, null, null, null, null, null, null, null);

            crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            {
                if (pageToCrawl.IsRoot)   // ignore web page if it's not a root
                    return new CrawlDecision { Allow = true, Reason = "crawling category links only" };

                return new CrawlDecision { Allow = false, Reason = "Pass through any other LINKs" };
            });

            crawler.PageCrawlCompletedAsync += crawlerComic_ProcessComicCrawlCompleted;


            return crawler;
        }

        private void crawlerComic_ProcessComicCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
            else if (crawledPage.IsRoot)
            {
                Console.WriteLine("Page is root URL {0}, start crawling", crawledPage.Uri.AbsoluteUri);
                CrawlerComicFromCategoryPage(crawledPage);
            }
            else
            {
                Console.WriteLine("Page is not root URL {0}, skipping crawling", crawledPage.Uri.AbsoluteUri);
            }
        }

        private void CrawlerComicFromCategoryPage(CrawledPage page)
        {
            if (page == null || string.IsNullOrEmpty(page.Content.Text) || !page.IsRoot)
            {
                return;
            }
            //string uri = page.Uri.ToString();
            int siteID = this.SiteID;

            try
            {

                HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(page.Content.Text);

                string xPathRoot = "/html[1]/body[1]/div[2]/div[1]/div[2]/div[1]/ul";


                var rootNode = doc.DocumentNode.SelectSingleNode(xPathRoot);
                if (rootNode != null)
                {
                    var liNodes = rootNode.SelectNodes("li");
                    if (liNodes != null && liNodes.Count > 0)
                    {
                        foreach (var node in liNodes)
                        {
                            string title = node.SelectSingleNode("span[1]/h3[1]/a[1]")
                                ?.InnerText;
                            string author = node.SelectSingleNode("span[1]/p[1]")?.InnerText;
                            string category = node.SelectSingleNode("span[1]/p[2]")?.InnerText;
                            string status = node.SelectSingleNode("span[1]/p[3]")?.InnerText;
                            string lastModifiedChapter = node.SelectSingleNode("span[1]/p[4]")?.InnerText;

                            string comicUrl = node.SelectSingleNode("a")
                                ?.Attributes["href"]
                                ?.Value;    // http://www.dmzj.com/info/wheretogo.html
                            string icon = node.SelectSingleNode("a[1]/img")
                                ?.Attributes["src"]
                                ?.Value;    // http://images.dmzj.com/img/webpic/15/1473157857.jpg


                            author = string.IsNullOrEmpty(author) ? string.Empty : author.Trim().Replace("作者：", "").Trim(); // 作者：漫画岛
                            category = string.IsNullOrEmpty(author) ? string.Empty : author.Trim().Replace("类型：", "").Trim(); // 类型：治愈/搞笑 
                            status = string.IsNullOrEmpty(author) ? string.Empty : author.Trim().Replace("状态：", "").Trim(); // 状态：连载中
                            lastModifiedChapter = string.IsNullOrEmpty(author) ? string.Empty : author.Trim().Replace("最新：", "").Trim(); // 最新：第02话

                            Guid comicID = m_BCL.UpdateComic(siteID, comicUrl, lastModifiedChapter);
                            m_BCL.UpdateComicComicDetail(comicID, title, author, !status.Contains("已完结"));

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region “Crawing Comic Chapter”

        /// <summary>
        /// Comic Links already store on table Comic. Crawling Chapter from Comic links
        /// </summary>
        public override void CrawlComicChapter()
        {
            PrepareCrawlForChapter();


        }

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
            config.MaxPagesToCrawl = 0;
            config.MaxPagesToCrawlPerDomain = 0;
            config.MinCrawlDelayPerDomainMilliSeconds = 30000;


            config.MaxCrawlDepth = 0;

            IWebCrawler crawler = new PoliteWebCrawler(config, null, null, null, null, null, null, null, null);

            //Register a lambda expression that will make Abot not crawl any url that has the word "ghost" in it.
            //For example http://a.com/ghost, would not get crawled if the link were found during the crawl.
            //If you set the log4net log level to "DEBUG" you will see a log message when any page is not allowed to be crawled.
            //NOTE: This is lambda is run after the regular ICrawlDecsionMaker.ShouldCrawlPage method is run.
            crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            {
                if (pageToCrawl.IsRoot)   // ignore web page if it's not a Comic
                    return new CrawlDecision { Allow = true, Reason = "crawling comic link only" };

                return new CrawlDecision { Allow = false, Reason = "Pass through any other LINKs" };
            });

            crawler.PageCrawlCompletedAsync += crawler_ProcessChapterCrawlCompleted;


            return crawler;
        }

        private void crawler_ProcessChapterCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
                        CrawledPage crawledPage = e.CrawledPage;

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
            else if (crawledPage.IsRoot)
            {
                Console.WriteLine("Page is root URL {0}, start crawling", crawledPage.Uri.AbsoluteUri);
                CrawlerChapterFromComicPage(crawledPage);
            }
            else
            {
                Console.WriteLine("Page is not root URL {0}, skipping crawling", crawledPage.Uri.AbsoluteUri);
            }
        }

        private void CrawlerChapterFromComicPage(CrawledPage page)
        {
            if (page == null || string.IsNullOrEmpty(page.Content.Text) || !page.IsRoot)
            {
                return;
            }

            string uri = page.Uri.ToString();
            int siteID = this.SiteID;
            string description = "";

            try
            {


                HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(page.Content.Text);
                Guid comicID = m_BCL.GetComicID(siteID, page.Uri);

                if (comicID == null || comicID == Guid.Empty) { return; }


                string xPathChapterRoot = "/html[1]/body[1]/div['wrap autoHeight']/div[1]/div[4]/div[2]/ul[1]";

                var chapterRootNode = doc.DocumentNode.SelectNodes(xPathChapterRoot).SingleOrDefault();
                if (chapterRootNode != null && chapterRootNode.ChildNodes != null)
                {
                    var chapterNodes = chapterRootNode.SelectNodes("li");
                    if (chapterNodes != null || chapterNodes.Count > 0)
                    {

                        for (int i = chapterNodes.Count - 1; i >=0; i--)
                        {
                            var chapterNode = chapterNodes[i];

                            if (chapterNode == null) continue;
                            if (chapterNode.SelectNodes("a").SingleOrDefault() == null) continue;

                            string chapterName = chapterNode.SelectNodes("a").SingleOrDefault()?.ChildNodes[1]?.InnerText;    // "第76话"
                            string chapterURL = chapterNode
                                ?.SelectNodes("a")[0]
                                ?.Attributes["href"]
                                ?.Value;    // "http://www.dmzj.com/view/xianxiashijie/56676.html" 
                            string chapterDesc = chapterNode
                                ?.SelectNodes("a")[0]
                                ?.Attributes["title"]
                                ?.Value; // "仙侠世界第76话 2016-09-02" 
                            int totalPage = -1;

                            if (string.IsNullOrEmpty(chapterName) || string.IsNullOrEmpty(chapterURL)) { continue; }

                            m_BCL.UpdateChapter(comicID, chapterName, chapterURL, chapterDesc, totalPage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        #endregion

    }
}
