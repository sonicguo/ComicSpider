namespace CrawPageEngine
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlWebBrowser = new System.Windows.Forms.Panel();
            this.webBwr = new System.Windows.Forms.WebBrowser();
            this.btnStart = new System.Windows.Forms.Button();
            this.pnlWebBrowser.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWebBrowser
            // 
            this.pnlWebBrowser.Controls.Add(this.webBwr);
            this.pnlWebBrowser.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlWebBrowser.Location = new System.Drawing.Point(0, 82);
            this.pnlWebBrowser.Name = "pnlWebBrowser";
            this.pnlWebBrowser.Size = new System.Drawing.Size(1231, 708);
            this.pnlWebBrowser.TabIndex = 0;
            // 
            // webBwr
            // 
            this.webBwr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBwr.Location = new System.Drawing.Point(0, 0);
            this.webBwr.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBwr.Name = "webBwr";
            this.webBwr.Size = new System.Drawing.Size(1231, 708);
            this.webBwr.TabIndex = 0;
            this.webBwr.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBwr_DocumentCompleted);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(13, 13);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(117, 46);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1231, 790);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.pnlWebBrowser);
            this.Name = "Form1";
            this.Text = "Form1";
            this.pnlWebBrowser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlWebBrowser;
        private System.Windows.Forms.WebBrowser webBwr;
        private System.Windows.Forms.Button btnStart;
    }
}

