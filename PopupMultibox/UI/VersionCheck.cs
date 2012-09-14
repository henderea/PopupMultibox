using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace Multibox.Core.UI
{
// ReSharper disable InconsistentNaming
    public partial class VersionCheck : Form, IVersionCheck
    {
        public VersionCheck()
        {
            InitializeComponent();
            checkTimer.Start();
            checkForUpdate();
        }

        private List<string> updates;
        //private const string websiteURL = "http://localhost/MyWebsite/multibox/"; 
        private const string websiteURL = "http://multibox.everydayprogramminggenius.com/";
        
        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(websiteURL + "download");
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
            CheckForUpdateForce();
        }

        public void CheckForUpdateForce()
        {
            try
            {
                string cv = Application.ProductVersion;
                cv = cv.Remove(cv.LastIndexOf("."));
                string nv = getData().Trim();
                if (!nv.Equals(cv))
                {
                    versionLabel.Text = "Current version: " + cv + "\n\nNew version: " + nv;
                    installButton.Enabled = false;
                    progressBar.Value = 0;
                    launchButton.Visible = false;
                    okButton.Visible = true;
                    downloadButton.Visible = true;
                    progressBar.Visible = false;
                    installButton.Visible = false;
                    Show();
                }
                else if(CheckForUpdates2())
                {
                    versionLabel.Text = "There are updates to plugins";
                    installButton.Enabled = false;
                    progressBar.Value = 0;
                    launchButton.Visible = true;
                    okButton.Visible = false;
                    downloadButton.Visible = false;
                    progressBar.Visible = false;
                    installButton.Visible = false;
                    Show();
                }
                Properties.Settings.Default.LastVersionCheck = DateTime.Now;
                Properties.Settings.Default.Save();
            }
            catch {}
        }

        private static string getData()
        {
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(websiteURL + "version_check");
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

        private bool CheckForUpdates2()
        {
            Dictionary<string, string> d = GetDLLInfo();
            Dictionary<string, string> d2 = GetData2();
            updates = new List<string>(0);
            updates.AddRange(from kv in d2 where !d.ContainsKey(kv.Key) || !d[kv.Key].Equals(kv.Value) select kv.Key);
            return updates.Count > 0;
        }

        private static Dictionary<string, string> GetDLLInfo()
        {
            Dictionary<string, string> rval = new Dictionary<string, string>(0);
            foreach (string f in Directory.EnumerateFiles(Application.StartupPath + "\\plugins\\"))
            {
                if (!f.EndsWith(".dll")) continue;
                Assembly assembly = Assembly.LoadFrom(f);
                AssemblyName n = assembly.GetName();
                try
                {
                    rval[n.Name] = n.Version.ToString(3);
                }
                catch { }
            }
            return rval;
        }

        private static Dictionary<string, string> GetData2()
        {
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(websiteURL + "plugin_check");
                myReq.Credentials = CredentialCache.DefaultCredentials;
                myReq.MaximumAutomaticRedirections = 4;
                myReq.MaximumResponseHeadersLength = 4;
                myReq.Method = "GET";
                HttpWebResponse resp = (HttpWebResponse)myReq.GetResponse();
                Stream strm = resp.GetResponseStream();
                StreamReader rdr = new StreamReader(strm);
                string rval2 = rdr.ReadToEnd();
                rdr.Close();
                string[] parts = rval2.Split(new[] { ";;;" }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, string> rval = new Dictionary<string, string>(0);
                foreach (string p in parts)
                {
                    try
                    {
                        string[] parts2 = p.Split(new[] { "=>" }, StringSplitOptions.None);
                        rval[parts2[0]] = parts2[1];
                    }
                    catch { }
                }
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
            webClient.DownloadFileAsync(new Uri(websiteURL + "MultiboxInstaller.msi"), fileChooserS.FileName);
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

        private void diffLogLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string cv = Application.ProductVersion;
            cv = cv.Remove(cv.LastIndexOf("."));
            Process.Start(websiteURL + "difflog?v=" + cv);
        }

        private void launchButton_Click(object sender, EventArgs e)
        {
            MainClass.CloseAndInstall(Application.StartupPath + "\\PluginUpdater.exe");
        }
    }
// ReSharper restore InconsistentNaming
}