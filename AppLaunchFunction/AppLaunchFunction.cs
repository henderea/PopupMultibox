﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using Multibox.Core.Functions;
using Multibox.Core.UI;
using Multibox.Plugin.Util;

namespace Multibox.Plugin.AppLaunchFunction
{
    public class AppLaunchFunction : AbstractFunction
    {
        public override int SuggestedIndex()
        {
            return 1;
        }

        private static void GetApps(string sDir, List<string> itms)
        {
            try
            {
                Regex tmp = new Regex(@".*\.lnk", RegexOptions.IgnoreCase);
                itms.AddRange(Filesystem.GetFiles(sDir).Where(f => tmp.IsMatch(f.Substring(f.LastIndexOf("\\") + 1))));
                foreach (string d in Filesystem.GetDirectories(sDir))
                {
                    itms.Add(d + "\\");
                    GetApps(d, itms);
                }
            }
            catch { }
        }

        private void ReloadCache()
        {
            List<string> tmp1 = new List<string>(0);
            List<string> tmp2 = new List<string>(0);
            appCache = new List<ResultItem>(0);
            string p1 = Filesystem.GetFolderPath(Environment.SpecialFolder.StartMenu);
            string p2 = Filesystem.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            GetApps(p1, tmp1);
            try
            {
                appCache.Sort();
            }
            catch { }
            GetApps(p2, tmp2);
            List<string> fnd = new List<string>(0);
            foreach (string t in tmp1)
            {
                string fnd2 = t.Substring(p1.Length + 1);
                string dtxt;
                if (!fnd2.EndsWith("\\"))
                {
                    fnd2 = fnd2.Remove(fnd2.Length - 4);
                    dtxt = fnd2.Substring(fnd2.LastIndexOf("\\") + 1);
                }
                else
                    dtxt = fnd2.Substring(fnd2.Remove(fnd2.Length - 1).LastIndexOf("\\") + 1);
                appCache.Add(new ResultItem(dtxt, t, fnd2));
                fnd.Add(fnd2);
            }
            foreach (string t in tmp2)
            {
                string fnd2 = t.Substring(p2.Length + 1);
                string dtxt;
                if (!fnd2.EndsWith("\\"))
                {
                    fnd2 = fnd2.Remove(fnd2.Length - 4);
                    dtxt = fnd2.Substring(fnd2.LastIndexOf("\\") + 1);
                }
                else
                    dtxt = fnd2.Substring(fnd2.Remove(fnd2.Length - 1).LastIndexOf("\\") + 1);
                if (fnd.Contains(fnd2)) continue;
                int ind = 0;
                foreach (string t1 in tmp1)
                {
                    string fnd3 = t1.Substring(p2.Length + 1);
                    if (!fnd3.EndsWith("\\")) 
                        fnd3 = fnd3.Remove(fnd3.Length - 4);
                    if (fnd2.CompareTo(fnd3) < 0)
                        ind++;
                    else
                        break;
                }
                tmp1.Insert(ind, t);
                appCache.Insert(ind, new ResultItem(dtxt, t, fnd2));
            }
        }

        private void CacheReloader()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(3600000);
                }
                catch { }
                try
                {
                    ReloadCache();
                }
                catch { }
            }
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns

        private List<ResultItem> DirList(string fnd)
        {
            List<ResultItem> tmp = new List<ResultItem>(0);
            foreach (ResultItem r in appCache)
            {
                string ss;
                int ind;
                try
                {
                    ss = r.EvalText.Substring(fnd.Length);
                    ind = ss.IndexOf("\\");
                }
                catch
                {
                    ss = "";
                    ind = -1;
                }
                if (r.EvalText.StartsWith(fnd) && (ind < 0 || ind == ss.Length - 1) && (!r.EvalText.EndsWith("\\") || !r.EvalText.Equals(fnd)))
                    tmp.Add(r);
            }
            return tmp;
        }

        private volatile List<ResultItem> appCache;
        private delegate void CacheReloaderDel();

        public AppLaunchFunction()
        {
            ReloadCache();
            new CacheReloaderDel(CacheReloader).BeginInvoke(null, null);
        }

        public override bool Triggers(MultiboxFunctionParam args)
        {
            return args.MultiboxText.StartsWith(">");
        }

        public override bool IsMulti(MultiboxFunctionParam args)
        {
            return true;
        }

        public override bool IsBackgroundStream(MultiboxFunctionParam args)
        {
            return true;
        }

        public override void RunMultiBackgroundStream(MultiboxFunctionParam args)
        {
            if (args.Key == Keys.Tab)
            {
                ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
                if (tmp2 != null)
                    args.MC.InputFieldText = ">" + tmp2.EvalText;
            }
            try
            {
                args.MC.LabelManager.ResultItems = DirList(args.MultiboxText.Substring(1));
                args.MC.UpdateSize();
                return;
            }
            catch { }
            args.MC.LabelManager.ResultItems = null;
            args.MC.UpdateSize();
        }

        public override bool HasDetails(MultiboxFunctionParam args)
        {
            return true;
        }

        public override string GetDetails(MultiboxFunctionParam args)
        {
            try
            {
                ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
                if (tmp2 != null)
                    return "Name: " + tmp2.DisplayText + "\nStart Menu Item: " + tmp2.FullText;
            }
            catch { }
            return "";
        }

        public override bool SupressKeyPress(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Tab);
        }

        public override bool HasActionKeyEvent(MultiboxFunctionParam args)
        {
            return true;
        }

        public override void RunActionKeyEvent(MultiboxFunctionParam args)
        {
            try
            {
                ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
                if (tmp2 == null) return;
                string tmpt = tmp2.FullText;
                Process.Start(tmpt);
            }
            catch { }
        }
    }
}