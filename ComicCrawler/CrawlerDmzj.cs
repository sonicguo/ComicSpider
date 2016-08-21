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

        public override void CrawlComicEpisode()
        {
            throw new NotImplementedException();
        }

        public override void CrawlComicEpisodePage()
        {
            throw new NotImplementedException();
        }
    }
}
