using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComicBCL;
using ComicData;
using HtmlAgilityPack;


namespace CrawPageEngine
{
    public partial class Form1 : Form
    {
        private DmzjBCLOperator bclOperator;
        private List<Chapter> chapters = new List<Chapter>();
        private const string mFilePath = @"c:\temp";
        private int index = 0;
        private Chapter currChapter = null;
        public Form1()
        {
            InitializeComponent();
            bclOperator = new DmzjBCLOperator();
            this.chapters = bclOperator.GetChapterList();
        }

        private void webBwr_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {

            
            string url = string.Empty;
            string context = webBwr.Document.Body.InnerText;

            ExtractPageInforAndUpdateToPageTable(context);

            url = GetLocalWebPageUrl();
            if (!string.IsNullOrEmpty(url))
            {
                this.webBwr.Navigate(url);
                this.btnStart.Enabled = false;
            }
            else
            {
                MessageBox.Show("Completed!");
                this.btnStart.Enabled = true;

            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string url = GetLocalWebPageUrl();
            if (!string.IsNullOrEmpty(url))
            {
                this.webBwr.Navigate(url);
                this.btnStart.Enabled = false;
            }
        }

        private string CreateScriptWebPageLocally(string path, string filename, string script)
        {
            string url = string.Empty;

            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(System.IO.Path.Combine(path, filename), true))
                {
                    file.WriteLine(script);
                }
                url = "http://localhost/temp/" + filename;
            }
            catch (Exception)
            {

                url = string.Empty;
            }

            return url;


        }

        private string ExtractScriptFromChapterUrl(string url)
        {
            string script = string.Empty;
            string xpath = "/html[1]/head[1]/script[1]";

            try
            {
                HtmlAgilityPack.HtmlWeb web = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = web.Load(url);
                System.Threading.Thread.Sleep(3000);

                var scriptNode = doc.DocumentNode.SelectNodes(xpath).SingleOrDefault();
                if (scriptNode != null && scriptNode.InnerHtml != null)
                {
                    script = scriptNode.InnerHtml.Trim();
                    script = script.Substring(script.IndexOf("eval")).Trim().Replace("return p", "document.write(p);return p;");
                    script = "<script>" + script + "</script>";
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(url + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }

            return script;
            
        }

        private string GetLocalWebPageUrl()
        {
            string url = string.Empty;
            while (index < chapters.Count -1 )
            {
                Chapter chapter = chapters[index++];
                string script = ExtractScriptFromChapterUrl(chapter.URL.ToString());
                if (string.IsNullOrEmpty(script))
                {
                    continue;
                }
                url = CreateScriptWebPageLocally(mFilePath, chapter.ChapterGUID + ".html", script);

                currChapter = chapter;

                if (!string.IsNullOrEmpty(url))
                {
                    break;
                }
            }
            return url;
        }

        private void ExtractPageInforAndUpdateToPageTable(string content)
        {

            Dictionary<string, string> elems = new Dictionary<string, string>();  // Key/Value paire 
            List<string> pagesURL = new List<string>(); // pages' URL
            int totalPage = 0;

            #region convert script execution result to a Key/Value paire 

            // var pages=pages='{"id":"50947","comic_id":"35369","chapter_name":"\u7b2c01\u8bdd","chapter_order":"1","createtime":"1465188960","folder":null,"page_url":"img\/chapterpic\/16602\/43750\/14651889556316.jpg\r\nimg\/chapterpic\/16602\/43750\/14651889557231.jpg\r\nimg\/chapterpic\/16602\/43750\/14651889558044.jpg\r\nimg\/chapterpic\/16602\/43750\/14651889558913.jpg","chapter_type":"0","chaptertype":"0","chapter_true_type":"1","chapter_num":"1","updatetime":"1465188960","sum_pages":"4","sns_tag":"1","uid":"100537698","username":"\u5468\u667a\u5ef6\u661f\u6f2b","translatorid":"","translator":"","link":"","message":"","download":"","hidden":"0","direction":"0","filesize":"343784","high_file_size":"0","picnum":"4","hit":"0","keywords":"","comic_alias":"","comic_real":"","comic_name":"\u6708\u5f71\u4ec1\u9b5446","first_letter":"y","comic_py":"yueyingrenmo46","last_update_chapter_name":"\u7b2c01\u8bdd","status":"\u5df2\u5b8c\u7ed3"}'; 
            if (string.IsNullOrEmpty(content))
            {
                return;
            }
            content = content.Replace("var pages=pages='{", ""); // remove prefix 
            content = content.Replace("}'; ", ""); // remove postfix

            string[] segements = content.Split(new char[] { ',' });
            

            foreach (var item in segements)
            {
                string[] items = item.Split(new char[] { ':' });
                if (items.Length != 2 || string.IsNullOrEmpty(items[0]) || string.IsNullOrEmpty(items[1]))
                {
                    continue;
                }

                // "id":"50947"
                string key = items[0].Trim().Replace("\"", ""); // remote \"
                string value = items[1].Trim().Replace("\"", ""); // remote \"
                if (!elems.ContainsKey(key))
                {
                    elems.Add(key, value);
                }
                else
                {
                    elems[key] = value;
                }
            }

            if (elems.Count == 0)
            {
                return;
            }

            #endregion

            #region Convert Pages' URL

            if (!elems.ContainsKey("page_url") || string.IsNullOrEmpty(elems["page_url"]))
            {
                return;
            }
            string strPagesURL = elems["page_url"];
            foreach (var item in strPagesURL.Split(new string[] { "\\r\\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                pagesURL.Add(item.Trim().Replace("\\",""));
            }
            #endregion

            #region Update Pages' information to Table
            for (int i = 0; i < pagesURL.Count; i++)
            {
                bclOperator.UpdatePage(Guid.Empty, currChapter.ChapterGUID, i, @"http://images.dmzj.com/" + pagesURL[i], "", i);
            }
            #endregion


        }


    }
}
