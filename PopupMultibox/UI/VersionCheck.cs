using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace PopupMultibox.UI
{
// ReSharper disable InconsistentNaming
    public partial class VersionCheck : Form
    {
        public VersionCheck()
        {
            InitializeComponent();
            checkTimer.Start();
            checkForUpdate();
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://multibox.everydayprogramminggenius.com/download");
        }

        private void VersionCheck_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            e.Cancel = true;
            Hide();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void checkTimer_Tick(object sender, EventArgs e)
        {
            checkForUpdate();
        }

        private void checkForUpdate()
        {
            try
            {
                if (!PrefsManager.AutoCheckUpdate || (DateTime.Now - Properties.Settings.Default.LastVersionCheck).TotalDays < PrefsManager.AutoCheckFrequency)
                    return;
            }
            catch { }
            checkForUpdateForce();
        }

        public void checkForUpdateForce()
        {
            string cv = Application.ProductVersion;
            cv = cv.Remove(cv.LastIndexOf("."));
            string nv = getData().Trim();
            if (!nv.Equals(cv))
            {
                versionLabel.Text = "Current version: " + cv + "\n\nNew version: " + nv;
                installButton.Enabled = false;
                progressBar.Value = 0;
                okButton.Visible = true;
                downloadButton.Visible = true;
                progressBar.Visible = false;
                installButton.Visible = false;
                Show();
            }
            Properties.Settings.Default.LastVersionCheck = DateTime.Now;
            Properties.Settings.Default.Save();
        }

        private static string getData()
        {
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create("http://multibox.everydayprogramminggenius.com/version_check");
                myReq.Credentials = CredentialCache.DefaultCredentials;
                myReq.MaximumAutomaticRedirections = 4;
                myReq.MaximumResponseHeadersLength = 4;
                myReq.Method = "GET";
                HttpWebResponse resp = (HttpWebResponse)myReq.GetResponse();
                Stream strm = resp.GetResponseStream();
                StreamReader rdr = new StreamReader(strm);
                string rval = rdr.ReadToEnd();
                rdr.Close();
                return rval;
            }
            catch { }
            return null;
        }

        private void fileChooserS_FileOk(object sender, CancelEventArgs e)
        {
            installButton.Enabled = false;
            progressBar.Value = 0;
            okButton.Visible = false;
            downloadButton.Visible = false;
            progressBar.Visible = true;
            installButton.Visible = true;
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += Completed;
            webClient.DownloadProgressChanged += ProgressChanged;
            webClient.DownloadFileAsync(new Uri("http://multibox.everydayprogramminggenius.com/MultiboxInstaller.msi"), fileChooserS.FileName);
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            installButton.Enabled = true;
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            fileChooserS.ShowDialog();
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            MainClass.CloseAndInstall(fileChooserS.FileName);
        }
    }
// ReSharper restore InconsistentNaming
}