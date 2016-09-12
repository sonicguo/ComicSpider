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

using JSExecutionEngine;

namespace ComicSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            //IWebCrawler crawler;

            //ComicBCL.DmzjBCLOperator control = new ComicBCL.DmzjBCLOperator();

            CrawlerDmzj crawler = new CrawlerDmzj();
            //crawler.CrawlComic();
            //crawler.CrawlComicChapter();

            //crawler.CrawPageTest();

            FakeJSExecution jsContext = new FakeJSExecution();
            jsContext.docCompleted += JsContext_docCompleted;

            jsContext.Execute("<script>eval(function(p,a,c,k,e,d){e=function(c){return(c<a?'':e(parseInt(c/a)))+((c=c%a)>35?String.fromCharCode(c+29):c.toString(36))};if(!''.replace(/^/,String)){while(c--){d[e(c)]=k[c]||e(c)}k=[function(e){return d[e]}];e=function(){return'\\w+'};c=1};while(c--){if(k[c]){p=p.replace(new RegExp('\\b'+e(c)+'\\b','g'),k[c])}}document.write(p);return p;}('I b=b=\'{\"J\":\"K\",\"L\":\"H\",\"G\":\"\\C\\9\",\"B\":\"4\",\"D\":\"8\",\"E\":F,\"M\":\"N\\/3\\/2\\/6\\/U.5\\r\\7\\/3\\/2\\/6\\/V.5\\r\\7\\/3\\/2\\/6\\/W.5\\r\\7\\/3\\/2\\/6\\/X.5\\r\\7\\/3\\/2\\/6\\/T.5\\r\\7\\/3\\/2\\/6\\/S.5\\r\\7\\/3\\/2\\/6\\/O.5\\r\\7\\/3\\/2\\/6\\/P.5\\r\\7\\/3\\/2\\/6\\/Q.5\\r\\7\\/3\\/2\\/6\\/A.5\\r\\7\\/3\\/2\\/6\\/Y.5\\r\\7\\/3\\/2\\/6\\/u.5\\r\\7\\/3\\/2\\/6\\/h.5\\r\\7\\/3\\/2\\/6\\/i.5\\r\\7\\/3\\/2\\/6\\/j.5\\r\\7\\/3\\/2\\/6\\/g.5\\r\\7\\/3\\/2\\/6\\/k.5\\r\\7\\/3\\/2\\/6\\/l.5\\r\\7\\/3\\/2\\/6\\/e.5\\r\\7\\/3\\/2\\/6\\/c.5\\r\\7\\/3\\/2\\/6\\/f.5\\r\\7\\/3\\/2\\/6\\/d.5\\r\\7\\/3\\/2\\/6\\/z.5\\r\\7\\/3\\/2\\/6\\/m.5\\r\\7\\/3\\/2\\/6\\/v.5\\r\\7\\/3\\/2\\/6\\/x.5\\r\\7\\/3\\/2\\/6\\/y.5\\r\\7\\/3\\/2\\/6\\/t.5\\r\\7\\/3\\/2\\/6\\/s.5\\r\\7\\/3\\/2\\/6\\/n.5\\r\\7\\/3\\/2\\/6\\/o.5\\r\\7\\/3\\/2\\/6\\/p.5\\r\\7\\/3\\/2\\/6\\/q.5\\r\\7\\/3\\/2\\/6\\/R.5\\r\\7\\/3\\/2\\/6\\/1v.5\",\"1r\":\"0\",\"1n\":\"0\",\"1o\":\"1\",\"1p\":\"4\",\"1q\":\"8\",\"1w\":\"a\",\"1x\":\"1\",\"1E\":\"1F\",\"1D\":\"\\1C\\1y\\1z\",\"1A\":\"\",\"1B\":\"\",\"1l\":\"\",\"1m\":\"\",\"16\":\"\",\"17\":\"0\",\"18\":\"0\",\"19\":\"15\",\"14\":\"0\",\"10\":\"a\",\"11\":\"\",\"12\":\"\",\"13\":\"\",\"1a\":\"\\1b\\1i\\1j\\1k\\1h\",\"1g\":\"w\",\"1c\":\"1d\",\"1e\":\"\\1f\\9\",\"1s\":\"\\1t\\Z\\1u\"}\';',62,104,'||1172|chapterpic||jpg|7436|nimg|1432896163|u8bdd|35|pages|14328961525919|14328961533867|14328961523478|14328961528349|14328961510749|14328961499298|14328961503453|14328961505644|14328961512745|14328961517876|14328961541765|14328961562629|14328961568076|14328961570286|14328961575481||14328961559509|14328961554138|14328961496742|14328961544036||14328961549173|14328961551657|14328961536114|14328961492168|chapter_order|u7b2c04|createtime|folder|null|chapter_name|18621|var|id|39363|comic_id|page_url|img|14328961477651|14328961480045|14328961485425|14328961577952|14328961475612|14328961470263|14328961461724|14328961464005|14328961466042|14328961468159|1432896149428|u8f7d|picnum|keywords|comic_alias|comic_real|high_file_size|3317257|download|hidden|direction|filesize|comic_name|u821e|comic_py|wudongdelinghun|last_update_chapter_name|u7b2c19|first_letter|u9b42|u52a8|u7684|u7075|link|message|chaptertype|chapter_true_type|chapter_num|updatetime|chapter_type|status|u8fde|u4e2d|14328961580519|sum_pages|sns_tag|u6f2b|u753b|translatorid|translator|u6dd8|username|uid|100141526'.split('|'),0,{}))</script>");

        }

        private static void JsContext_docCompleted(object sender, string innerHtml)
        {
            string mystring = innerHtml;
        }
    }
}
