using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace PopupMultibox
{
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
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Hide();
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
            string cv = Application.ProductVersion;
            cv = cv.Remove(cv.LastIndexOf("."));
            string nv = getData().Trim();
            if (!nv.Equals(cv))
            {
                versionLabel.Text = "Current version: " + cv + "\n\nNew version: " + nv;
                this.Show();
            }
            Properties.Settings.Default.LastVersionCheck = DateTime.Now;
            Properties.Settings.Default.Save();
        }

        private string getData()
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
    }
}
