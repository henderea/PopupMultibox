using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Henderson.Util.MyDictionary;
using System.Text.RegularExpressions;
using System.Security.Permissions;

namespace PopupMultibox.UI
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
// ReSharper disable InconsistentNaming
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
            helpViewer.ObjectForScripting = this;
            indexMatcher1 = new Regex(@"_\((\d+)\+(.+?)\)");
            indexMatcher2 = new Regex(@"_\[(\d+)\+(.+?)\]");
            indexMatcher3 = new Regex(@"^\{(.+?)\}.mbh$");
            linkMatcher1 = new Regex(@"\[a\#([.\d]+)\]");
            linkMatcher2 = new Regex(@"\[/a\]");
            IndexHelpFiles();
            string pcont = GeneratePage("");
            helpViewer.DocumentText = pcont;
        }

        private MyDictionary fileIndex;
        private readonly Regex indexMatcher1;
        private readonly Regex indexMatcher2;
        private readonly Regex indexMatcher3;
        private readonly Regex linkMatcher1;
        private readonly Regex linkMatcher2;

        private void Help_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
            Hide();
        }

        private void IndexHelpFiles()
        {
            string[] files = Directory.GetFiles(Application.StartupPath + "\\help files\\");
            fileIndex = new MyDictionary();
            foreach (string f in files)
            {
                string f2 = f.Substring(f.LastIndexOf("\\") + 1);
                MatchCollection mc1 = indexMatcher1.Matches(f2);
                Match m2 = indexMatcher2.Match(f2);
                Match m3 = indexMatcher3.Match(f2);
                if (m3.Success)
                {
                    fileIndex["hname"] = m3.Groups[1].Value;
                    fileIndex["fname"] = f;
                }
                List<int> inds = new List<int>(0);
                foreach (Match m in mc1)
                {
                    try
                    {
                        int mi = int.Parse(m.Groups[1].Value);
                        string mn = m.Groups[2].Value;
                        inds.Add(mi);
                        MyDictionary tmpd = inds.Aggregate(fileIndex, (current, ti) => current[ti]);
                        tmpd["name"] = mn;
                    }
                    catch { }
                }
                try
                {
                    if (m2.Success)
                    {
                        int mi = int.Parse(m2.Groups[1].Value);
                        string mn = m2.Groups[2].Value;
                        inds.Add(mi);
                        MyDictionary tmpd = inds.Aggregate(fileIndex, (current, ti) => current[ti]);
                        tmpd["sname"] = mn;
                    }
                }
                catch { }
                try
                {
                    MyDictionary tmpd = inds.Aggregate(fileIndex, (current, ti) => current[ti]);
                    tmpd["fname"] = f;
                }
                catch { }
            }
        }

        public void ShowPage(string page)
        {
            string pcont = GeneratePage(page);
            helpViewer.Document.OpenNew(true);
            helpViewer.Document.Write(pcont);
        }

        private string GeneratePage(string page)
        {
            string rval = "<html><body>";
            if (!string.IsNullOrEmpty(page))
            {
                rval += "<div style=\"paddding:20px;\"><a href=\"#\" onclick=\"window.external.ShowPage('');\" style=\"color:#0000FF;cursor:pointer;\">Home</a></div>";
                string[] parts = page.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                List<int> inds = new List<int>(0);
                foreach (string p in parts)
                {
                    try
                    {
                        inds.Add(int.Parse(p));
                    }
                    catch { }
                }
                string tmpinds = "";
                for (int i = 0; i < inds.Count - 1; i++)
                {
                    tmpinds += inds[i] + "";
                    if (i < inds.Count - 2)
                        tmpinds += ".";
                }
                rval += "<div style=\"paddding:20px;\"><a href=\"#\" onclick=\"window.external.ShowPage('" + tmpinds + "');\" style=\"color:#0000FF;cursor:pointer;\">Up one level</a></div>";
                MyDictionary tmpd = fileIndex;
                for (int i = 0; i < inds.Count; i++)
                {
                    int ti = inds[i];
                    tmpd = tmpd[ti];
                    if (tmpd.Keys.Length > 0 && tmpd["name"].Value != null)
                        rval += "<h" + (i + 1) + ">" + tmpd["name"] + "</h" + (i + 1) + ">";
                }
                try
                {
                    if (tmpd.Keys.Length > 0)
                    {
                        string fcont = File.ReadAllText(tmpd["fname"]);
                        rval += "<div>" + EvalLinks(fcont) + "</div>";
                    }
                }
                catch { }
                if (tmpd.IKeys.Length > 0 && tmpd[tmpd.IKeys[0]].Keys.Length > 0 && tmpd[tmpd.IKeys[0]]["sname"] != null)
                {
                    foreach (MyDictionary d in tmpd.Values)
                    {
                        if (d.Keys.Length > 0 && d["sname"].Value != null)
                        {
                            rval += "<div style=\"font-size:larger;font-weight:bold;padding-top:20px;\">" + d["sname"] + "</div>";
                            try
                            {
                                if (d.Keys.Length > 0)
                                {
                                    string fcont = File.ReadAllText(d["fname"]);
                                    rval += "<div>" + EvalLinks(fcont) + "</div>";
                                }
                            }
                            catch { }
                        }
                    }
                }
                try
                {
                    rval += GenerateLinks(tmpd, page, "");
                }
                catch { }
            }
            else
            {
                if (fileIndex.Keys.Length > 0 && fileIndex["hname"].Value != null)
                    rval += "<h1>" + fileIndex["hname"] + "</h1>";
                else
                    rval += "<h1>Multibox Help</h1>";
                try
                {
                    if (fileIndex.Keys.Length > 0)
                    {
                        string fcont = File.ReadAllText(fileIndex["fname"]);
                        rval += "<div>" + EvalLinks(fcont) + "</div>";
                    }
                }
                catch { }
                try
                {
                    rval += GenerateLinks(fileIndex, "", "");
                }
                catch { }
            }
            rval += "</body></html>";
            return rval;
        }

        private static string GenerateLinks(MyDictionary d, string baseindstr, string indstr)
        {
            string rval = "";
            if (indstr == null || indstr.Length <= 0)
                rval = "<br><div style=\"font-size:larger;font-weight:bold;\">Links</div>";
            if (!string.IsNullOrEmpty(indstr))
            {
                if (d.Keys.Length <= 0 || d["name"].Value == null || d["name"].Value.ToString().Length <= 0)
                    return "";
                rval += "<div>" + baseindstr + indstr + "<a href=\"#\" onclick=\"window.external.ShowPage('" + baseindstr + indstr + "');\" style=\"color:#0000FF;cursor:pointer;\">" + d["name"] + "</a></div>";
            }
            int[] ik = d.IKeys;
            if (ik.Length <= 0)
                return rval;
            rval += "<div style=\"padding:0px;padding-left:20px;\">";
            rval = ik.Aggregate(rval, (current, i) => current + GenerateLinks(d[i], baseindstr, (((indstr == null || indstr.Length <= 0) && (baseindstr == null || baseindstr.Length <= 0)) ? "" : indstr + ".") + i));
            rval += "</div>";
            return rval;
        }

        private string EvalLinks(string content)
        {
            string rval = linkMatcher1.Replace(content, "<a href=\"#\" onclick=\"window.external.ShowPage('$1');\" style=\"color:#0000FF;cursor:pointer;\">");
            rval = linkMatcher2.Replace(rval, "</a>");
            return rval;
        }

        public void LaunchPage(string page)
        {
            Show();
            ShowPage(page);
        }

        public List<ResultItem> GetAutocompleteOptions(string path)
        {
            List<ResultItem> lst = new List<ResultItem>(0);
            GetAutocompleteOptions(fileIndex, "", path, "", lst, true);
            return lst;
        }

        private static void GetAutocompleteOptions(MyDictionary d, string path, string rempath, string numpath, List<ResultItem> lst, bool fv)
        {
            if (!string.IsNullOrEmpty(path) || !fv)
            {
                if (d.Keys.Length <= 0 || d["name"].Value == null || d["name"].Value.ToString().Length <= 0)
                    return;
                string newpath = path + d["name"] + ">";
                if (rempath.Length <= 0)
                {
                    lst.Add(new ResultItem(d["name"], newpath, numpath));
                    return;
                }
                int ind = rempath.IndexOf(">");
                if (ind >= 0)
                {
                    if ((d["name"] + "").Equals(rempath.Remove(ind)))
                    {
                        string newrempath = "";
                        if (ind < rempath.Length - 1)
                            newrempath = rempath.Substring(ind + 1);
                        else
                            lst.Add(new ResultItem("<Current Item>", newpath, numpath));
                        int[] ik = d.IKeys;
                        if (ik.Length <= 0)
                        {
                            return;
                        }
                        foreach (int i in ik)
                        {
                            GetAutocompleteOptions(d[i], newpath, newrempath, ((numpath == null || numpath.Length <= 0) ? "" : numpath + ".") + i, lst, false);
                        }
                    }
                }
            }
            else
            {
                if (rempath == null || rempath.Length <= 0)
                    lst.Add(new ResultItem("<Current Item>", "", ""));
                int[] ik = d.IKeys;
                if (ik.Length <= 0)
                    return;
                foreach (int i in ik)
                {
                    GetAutocompleteOptions(d[i], path, rempath, ((numpath == null || numpath.Length <= 0) ? "" : numpath + ".") + i, lst, false);
                }
            }
        }
    }
// ReSharper restore InconsistentNaming
}