using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private static AutoResetEvent mEvent = new AutoResetEvent(false);
        string html = string.Empty;

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(Navigate);
            t.Start();
            
            mEvent.WaitOne();

            string temp = html;
        }

        void Navigate()
        {
            WebBrowser webBrowser1 = new WebBrowser();
            webBrowser1.Visible = false;
            webBrowser1.DocumentCompleted += WebBrowser1_DocumentCompleted;
            webBrowser1.Navigated += WebBrowser1_Navigated;
            this.webBrowser1.Navigate("http://localhost/eval.html");

        }

        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            html = webBrowser1.Document.Body.InnerText;
            mEvent.Set();
        }

        private void WebBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            
             html = this.webBrowser1.DocumentText;
        }
    }
}
