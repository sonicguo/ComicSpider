using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abot.Crawler;
using Abot.Poco;

using ComicData;
using ComicBCL;
using ComicCrawler;

namespace ComicSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            //IWebCrawler crawler;

            //ComicBCL.DmzjBCLOperator control = new ComicBCL.DmzjBCLOperator();

            CrawlerDmzj crawler = new CrawlerDmzj();
            crawler.CrawlCategory();

        }

    }
}
