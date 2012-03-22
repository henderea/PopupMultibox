namespace PopupMultibox.UI
{
    partial class Help
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Help));
            this.helpViewer = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // helpViewer
            // 
            this.helpViewer.AllowWebBrowserDrop = false;
            this.helpViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpViewer.Location = new System.Drawing.Point(0, 0);
            this.helpViewer.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpViewer.Name = "helpViewer";
            this.helpViewer.ScriptErrorsSuppressed = true;
            this.helpViewer.Size = new System.Drawing.Size(584, 662);
            this.helpViewer.TabIndex = 1;
            this.helpViewer.Url = new System.Uri("http://localhost/", System.UriKind.Absolute);
            this.helpViewer.WebBrowserShortcutsEnabled = false;
            // 
            // Help
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 662);
            this.Controls.Add(this.helpViewer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Help";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Help";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Help_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser helpViewer;
    }
}