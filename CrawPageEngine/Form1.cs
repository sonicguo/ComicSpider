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
        DmzjBCLOperator bclOperator = new DmzjBCLOperator();
        List<Chapter> chapters = new List<Chapter>();
        private const string mFilePath = @"c:\temp";
        int index = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void webBwr_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string url = string.Empty;
            string context = webBwr.Document.Body.InnerText;
            while (index < chapters.Count -1)
            {
                Chapter chapter = chapters[index++];
                string script = ExtractScriptFromChapterUrl(chapter.URL.ToString());

                url = CreateScriptWebPageLocally(mFilePath, chapter.ChapterGUID + ".html", script);

                if (string.IsNullOrEmpty(url))
                {
                    continue;
                }
                else break;
                
            }
            if (!string.IsNullOrEmpty(url))
            {
                this.webBwr.Navigate(url);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.chapters = bclOperator.GetChapterList();
            Chapter chapter = chapters[index++];
            string script = ExtractScriptFromChapterUrl(chapter.URL.ToString());

            string url = CreateScriptWebPageLocally(mFilePath, chapter.ChapterGUID+".html", script);

            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            this.webBwr.Navigate(url);
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

            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);

            var scriptNode = doc.DocumentNode.SelectNodes(xpath).SingleOrDefault();
            if (scriptNode != null && scriptNode.InnerHtml != null)
            {
                script = scriptNode.InnerHtml.Trim();
                script = script.Substring(script.IndexOf("eval")).Trim().Replace("return p", "document.write(p);return p;");
                script = "<script>" + script + "</script>";
            }
            

            return script;
            
        }
    }
}
