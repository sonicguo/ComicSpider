using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abot.Crawler;
using Abot.Poco;

namespace ComicCrawler
{

    public abstract class BasicCrawler
    {
        protected IWebCrawler m_Crawler;
        public abstract void CrawlCategory();
        public abstract void CrawlComicChapter();
        public abstract void CrawlComicChapterPage();

    }
}
