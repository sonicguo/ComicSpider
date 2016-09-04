using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbotX;
using AbotX.Crawler;
using AbotX.Core;
using AbotX.Poco;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var crawler = new CrawlerX();


            var config = new CrawlConfigurationX();
            {
                MaxConcurrentSiteCrawls = 10,
                SitesToCrawlBatchSizePerRequest = 25,
                MinSiteToCrawlRequestDelayInSecs = 15,
                IsJavascriptRenderingEnabled = false,
                JavascriptRenderingWaitTimeInMilliseconds = 3500
                //etc...
            };

            //Now you must pass it into the constructor of CrawlerX or ParallelCrawlerEngine var crawler = new CrawlerX(config);
            var crawlerEngine = new ParallelCrawlerEngine(config);

        }
    }
}
