using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace Multibox.PluginUpdater
{
// ReSharper disable InconsistentNaming
    public partial class MainClass : Form
    {
        public MainClass()
        {
            InitializeComponent();
            UpdateDataView();
        }

        private List<string> updates;
        //private const string websiteURL = "http://localhost/MyWebsite/multibox/"; 
        private const string websiteURL = "http://multibox.everydayprogramminggenius.com/";

        private void UpdateDataView()
        {
            installButton.Enabled = false;
            Dictionary<string, string> d = GetDLLInfo();
            Dictionary<string, string> d2 = GetData();
            updates = new List<string>(0);
            updates.AddRange(from kv in d2 where !d.ContainsKey(kv.Key) || !d[kv.Key].Equals(kv.Value) select kv.Key);
            updateList.Rows.Clear();
            if (updates.Count <= 0) return;
            foreach (string u in updates)
            {
                updateList.Rows.Add(new object[] { true, u, d.ContainsKey(u) ? d[u] : "none", d2[u] });
            }
            installButton.Enabled = true;
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
                catch {}
            }
            return rval;
        }

        private static Dictionary<string, string> GetData()
        {
            try
            {
                HttpWebRequest myReq = (HttpWebRequest) WebRequest.Create(websiteURL + "plugin_check");
                myReq.Credentials = CredentialCache.DefaultCredentials;
                myReq.MaximumAutomaticRedirections = 4;
                myReq.MaximumResponseHeadersLength = 4;
                myReq.Method = "GET";
                HttpWebResponse resp = (HttpWebResponse) myReq.GetResponse();
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
                    catch {}
                }
                return rval;
            }
            catch {}
            return null;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            UpdateDataView();
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in updateList.Rows)
            {
                r.Cells[updateCheckCol.Name].Value = true;
            }
        }

        private void uncheckButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in updateList.Rows)
            {
                r.Cells[updateCheckCol.Name].Value = false;
            }
        }

        private WebClient webClient;

        private void installButton_Click(object sender, EventArgs e)
        {
            installButton.Enabled = false;
            checkButton.Enabled = false;
            uncheckButton.Enabled = false;
            closeButton.Enabled = false;
            foreach (DataGridViewRow r in updateList.Rows)
            {
                if (!((bool)r.Cells[updateCheckCol.Name].Value))
                    updates.Remove((string)r.Cells[pluginNameCol.Name].Value);
            }
            webClient = new WebClient();
            webClient.DownloadFileCompleted += Completed;
            webClient.DownloadProgressChanged += ProgressChanged;
            if (updates.Count <= 0)
            {
                checkButton.Enabled = true;
                uncheckButton.Enabled = true;
                closeButton.Enabled = true;
                UpdateDataView();
                return;
            }
            try
            {
                webClient.DownloadFileAsync(new Uri(websiteURL + "plugins/" + updates[0] + ".dll"), Application.StartupPath + "\\plugins\\" + updates[0] + ".dll");
            }
            catch
            {
                UpdateDataView();
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (updates.Count > 0)
                updates.RemoveAt(0);
            else
            {
                checkButton.Enabled = true;
                uncheckButton.Enabled = true;
                closeButton.Enabled = true;
                UpdateDataView();
                return;
            }
            if (updates.Count > 0)
            {
                progressBar.Value = 0;
                try
                {
                    webClient.DownloadFileAsync(new Uri(websiteURL + "plugins/" + updates[0] + ".dll"), Application.StartupPath + "\\plugins\\" + updates[0] + ".dll");
                }
                catch
                {
                    UpdateDataView();
                }
            }
            else
            {
                checkButton.Enabled = true;
                uncheckButton.Enabled = true;
                closeButton.Enabled = true;
                UpdateDataView();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "\\PopupMultibox.exe");
            Application.Exit();
        }
    }

// ReSharper restore InconsistentNaming
}