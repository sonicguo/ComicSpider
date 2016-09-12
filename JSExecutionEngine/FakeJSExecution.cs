using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSExecutionEngine
{
    public delegate void DocumentCompleted(object sender, string innerHtml);
    public class FakeJSExecution
    {
        WebBrowser webBrowser = new WebBrowser();

        const string filename = @"c:\temp\temp.html";
        const string url = "http://localhost/temp/temp.html";
        private static object _sycblk = new object();

        public event DocumentCompleted docCompleted;

        public FakeJSExecution()
        {
            if (webBrowser == null)
            {
                webBrowser = new WebBrowser();
            }
            webBrowser.Visible = false;
            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (docCompleted != null)
            {
                docCompleted(sender, this.webBrowser.Document?.Body?.InnerHtml);
            }
        }

        ~FakeJSExecution()
        {
            webBrowser.Dispose();
        }

        public void Execute(string script)
        {
            try
            {

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filename, false))
                {
                    file.WriteLine(script);

                    webBrowser.Navigate(url);
                }

            }
            catch (Exception)
            {

                
            }

        }
    }
}
