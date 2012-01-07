using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;

namespace PopupMultibox
{
    public class FilesystemFunction : MultiboxFunction
    {
        private void DirSearch(string sDir, string fnd, List<string> itms)
        {
            try
            {
                Regex tmp = new Regex(fnd.Replace(@"\\", @"\\\\").Replace(@".", @"\.").Replace(@"*", @".*").Replace(@"?", @".?").Replace(@"[", @"\[").Replace(@"]", @"\]".Replace(@"(", @"\(").Replace(@")", @"\)")), RegexOptions.IgnoreCase);
                foreach (string f in Directory.GetFiles(sDir))
                {
                    if (tmp.IsMatch(f.Substring(f.LastIndexOf("\\") + 1)))
                        itms.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirSearch(d, fnd, itms);
                }
            }
            catch { }
        }

        private string[] DirList(string sDir, string fnd)
        {
            try
            {
                List<string> itms = new List<string>(0);
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    if (d.StartsWith(fnd))
                        itms.Add(d.Remove(0, sDir.Length) + "\\");
                }
                foreach (string f in Directory.GetFiles(sDir))
                {
                    if (f.StartsWith(fnd))
                        itms.Add(f.Remove(0, sDir.Length));
                }
                return itms.ToArray();
            }
            catch { }
            return null;
        }

        private string[] DirList2(string sDir)
        {
            try
            {
                List<string> itms = new List<string>(0);
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    itms.Add(d + "\\");
                }
                foreach (string f in Directory.GetFiles(sDir))
                {
                    itms.Add(f);
                }
                return itms.ToArray();
            }
            catch { }
            return null;
        }

        private long GetFileSize(string path)
        {
            try
            {
                FileAttributes fa = File.GetAttributes(path);
                if ((fa & FileAttributes.Directory) == FileAttributes.Directory)
                    return -1;
                return new FileInfo(path).Length;
            }
            catch
            {
                return -1;
            }
        }

        private long GetFileSize2(string path)
        {
            try
            {
                return new FileInfo(path).Length;
            }
            catch { }
            return -1;
        }

        private bool GetFolderSize(string path, MainClass mc, ref long cs, ref long files, ref long folders, ref DateTime ld, long delay)
        {
            try
            {
                if (cancelCalc)
                    return false;
                FileAttributes fa = File.GetAttributes(path);
                if ((fa & FileAttributes.Directory) != FileAttributes.Directory)
                {
                    cs += GetFileSize2(path);
                    files++;
                    DateTime nw = DateTime.Now;
                    if ((nw - ld).TotalMilliseconds > delay)
                    {
                        ld = nw;
                        if (cancelCalc)
                            return false;
                        new SetMCOutputLabelTextDel(SetMCOutputLabelText).BeginInvoke(mc, cs, files, folders, currentEnding, null, null);
                    }
                    return true;
                }
                folders++;
                string[] itms = DirList2(path);
                if (itms == null || itms.Length <= 0 || cancelCalc)
                    return false;
                foreach (string itm in itms)
                {
                    if (cancelCalc)
                        return false;
                    GetFolderSize(itm, mc, ref cs, ref files, ref folders, ref ld, delay);
                }
                return true;
            }
            catch { }
            return false;
        }

        private void SetMCOutputLabelText(MainClass mc, long cs, long files, long folders, string ending)
        {
            int i = 0;
            double size = (double)cs;
            while (size >= 1024 && i < sizeEndings.Length - 1)
            {
                if (sizeEndings[i].ToLower().Equals(ending))
                    break;
                i++;
                size = size / 1024.0;
            }
            mc.OutputLabelText = files + " file(s); " + folders + " folder(s); " + size.ToString("#.###") + sizeEndings[i];
        }

        private static string lastSizePath = null;
        private static long lastSizeValue = -1;
        private static long lastSizeFiles = -1;
        private static long lastSizeFolders = -1;
        private static DateTime lastSizeTime;
        private static volatile string currentEnding = null;
        private static string[] sizeEndings = new string[] { "B", "KB", "MB", "GB", "TB" };
        private static volatile bool cancelCalc = false;
        private static volatile bool isCalculating = false;
        private delegate void SetMCOutputLabelTextDel(MainClass mc, long cs, long files, long folders, string ending);

        public FilesystemFunction()
        {
            BookmarkList.Load();
        }

        #region MultiboxFunction Members

        public bool Triggers(MultiboxFunctionParam args)
        {
            return (args.MultiboxText != null && args.MultiboxText.Length > 0 && args.MultiboxText[0] == ':');
        }

        public bool IsMulti(MultiboxFunctionParam args)
        {
            return ((args.MultiboxText.IndexOf(">>>") <= 1) && (args.MultiboxText.IndexOf("<<") <= 1));
        }

        public bool IsBackgroundStream(MultiboxFunctionParam args)
        {
            return (args.MultiboxText.IndexOf(">>>") > 1);
        }

        public bool ShouldRun(MultiboxFunctionParam args)
        {
            return !(args.Key == Keys.Up || args.Key == Keys.Down);
        }

        public string RunSingle(MultiboxFunctionParam args)
        {
            int ind = args.MultiboxText.IndexOf(">>>");
            int ind2 = args.MultiboxText.IndexOf("<<");
            if (ind <= 1 && ind2 > 1)
            {
                string pth = args.MultiboxText.Substring(1, ind2 - 1);
                string pth2 = pth.Substring(0, pth.LastIndexOf("\\") + 1);
                string name = "";
                try
                {
                    name = args.MultiboxText.Substring(ind2 + 2);
                }
                catch { }
                return "Bookmark " + pth2 + " as \"" + name + "\"";
            }
            throw new InvalidOperationException();
        }

        public List<ResultItem> RunMulti(MultiboxFunctionParam args)
        {
            if ((args.MultiboxText.IndexOf(">>>") > 1) || (args.MultiboxText.IndexOf("<<") > 1))
                throw new InvalidOperationException();
            if (args.Key == Keys.Tab)
            {
                ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
                if (tmp2 != null)
                    args.MC.InputFieldText = ":" + tmp2.EvalText;
            }
            int ind = args.MultiboxText.IndexOf(">> ");
            if (args.Key != Keys.Up && args.Key != Keys.Down)
            {
                if (ind <= 1)
                {
                    string pth = args.MultiboxText.Substring(1);
                    string pth2 = pth.Substring(0, pth.LastIndexOf("\\") + 1);
                    string pth3 = pth2;
                    if (pth3.Length > 0 && pth3[0] == '~')
                        pth3 = args.MC.HomeDirectory + pth3.Substring(1);
                    string pth4 = pth;
                    if (pth4.Length > 0 && pth4[0] == '~')
                        pth4 = args.MC.HomeDirectory + pth4.Substring(1);
                    string[] pths = DirList(pth3, pth4);
                    if (pths != null)
                    {
                        List<ResultItem> tmp = new List<ResultItem>(0);
                        foreach (string tpth in pths)
                        {
                            tmp.Add(new ResultItem(tpth, pth3 + tpth, pth2 + tpth));
                        }
                        return tmp;
                    }
                    return null;
                }
                else
                {
                    string pth = args.MultiboxText.Substring(1, ind - 1);
                    string pth2 = pth.Substring(0, pth.LastIndexOf("\\") + 1);
                    string pth3 = pth2;
                    bool ht = (pth3.Length > 0 && pth3[0] == '~');
                    if (ht)
                        pth3 = args.MC.HomeDirectory + pth3.Substring(1);
                    string fnd = args.MultiboxText.Substring(ind + 3);
                    if (fnd.Length > 0 && fnd[0] == '/')
                        fnd = @"^" + fnd.Substring(1) + @"$";
                    List<string> rlts = new List<string>(0);
                    DirSearch(pth3, fnd, rlts);
                    if (rlts != null && rlts.Count > 0)
                    {
                        List<ResultItem> tmp = new List<ResultItem>(0);
                        foreach (string tpth in rlts)
                        {
                            tmp.Add(new ResultItem(tpth.Substring(tpth.LastIndexOf("\\") + 1), tpth, ht ? tpth.Replace(args.MC.HomeDirectory, "~") : tpth));
                        }
                        return tmp;
                    }
                    return null;
                }
            }
            return null;
        }

        public void RunSingleBackgroundStream(MultiboxFunctionParam args)
        {
            int ind = args.MultiboxText.IndexOf(">>>");
            if (ind > 1)
            {
                string pth = args.MultiboxText.Substring(1, ind - 1);
                if (pth.Length > 0 && pth[0] == '~')
                    pth = args.MC.HomeDirectory + pth.Substring(1);
                string ending = sizeEndings[sizeEndings.Length - 1];
                try
                {
                    ending = args.MultiboxText.Substring(ind + 3);
                }
                catch { }
                ending = ending.ToLower();
                currentEnding = ending;
                if (lastSizePath != null && lastSizePath.Equals(pth) && isCalculating)
                    return;
                cancelCalc = true;
                Thread.Sleep(10);
                cancelCalc = false;
                if (lastSizeValue <= 0 || lastSizePath == null || lastSizeTime == null || !lastSizePath.Equals(pth) || (DateTime.Now - lastSizeTime).TotalMinutes >= 5)
                {
                    lastSizePath = pth;
                    args.MC.OutputLabelText = "Calculating size, please wait...";
                    args.MC.UpdateSize();
                    DateTime ld = DateTime.Now;
                    lastSizeValue = 0;
                    lastSizeFiles = 0;
                    lastSizeFolders = 0;
                    isCalculating = true;
                    if (!GetFolderSize(pth, args.MC, ref lastSizeValue, ref lastSizeFiles, ref lastSizeFolders, ref ld, 500))
                    {
                        lastSizeValue = -1;
                        lastSizeFiles = -1;
                        lastSizeFolders = -1;
                    }
                    isCalculating = false;
                    if (cancelCalc)
                        return;
                    lastSizeTime = DateTime.Now;
                }
                double size = (double)lastSizeValue;
                if (size <= 0)
                {
                    args.MC.OutputLabelText = "Invalid selection";
                    return;
                }
                int i = 0;
                while (size >= 1024 && i < sizeEndings.Length - 1)
                {
                    if (sizeEndings[i].ToLower().Equals(currentEnding))
                        break;
                    i++;
                    size = size / 1024.0;
                }
                args.MC.OutputLabelText = lastSizeFiles + " file(s); " + lastSizeFolders + " folder(s); " + size.ToString("#.###") + sizeEndings[i];
                args.MC.UpdateSize();
            }
        }

        public void RunMultiBackgroundStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasDetails(MultiboxFunctionParam args)
        {
            return true;
        }

        public bool IsBackgroundDetailsStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public string GetDetails(MultiboxFunctionParam args)
        {
            string pth = args.MC.LabelManager.CurrentSelection.FullText;
            if (pth.Length > 0 && pth[0] == '~')
                pth = args.MC.HomeDirectory + pth.Substring(1);
            long sz = GetFileSize(pth);
            double size = (double)sz;
            string sizestr = "--";
            if (size > 0)
            {
                int i = 0;
                while (size >= 1024 && i < sizeEndings.Length - 1)
                {
                    if (sizeEndings[i].ToLower().Equals(currentEnding))
                        break;
                    i++;
                    size = size / 1024.0;
                }
                sizestr = size.ToString("#.###") + sizeEndings[i];
            }
            string typ = "";
            if (sz > 0)
            {
                try
                {
                    typ = GetFileInfo.GetTypeName(pth);
                }
                catch
                {
                    typ = "File";
                }
            }
            else
                typ = "Folder";
            string lmd = "";
            try
            {
                DateTime lmddt = File.GetLastWriteTime(pth);
                lmd = lmddt.ToShortDateString() + " " + lmddt.ToLongTimeString();
            }
            catch { }
            return "Name: " + args.MC.LabelManager.CurrentSelection.DisplayText + "\nType: " + typ + "\nSize: " + sizestr + "\nLast Modified: " + lmd;
        }

        public void GetBackgroundDetailsStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasActions(MultiboxFunctionParam args)
        {
            return false;
        }

        public bool IsBackgroundActionsStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public List<ResultItem> GetActions(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public void GetBackgroundActionsStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasAction(MultiboxFunctionParam args)
        {
            return false;
        }

        public void RunAction(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool SupressKeyPress(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Up || args.Key == Keys.Down || args.Key == Keys.Tab);
        }

        public bool HasKeyDownAction(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Up || args.Key == Keys.Down);
        }

        public void RunKeyDownAction(MultiboxFunctionParam args)
        {
            if (args.Key == Keys.Up)
                args.MC.LabelManager.SelectPrev();
            else if (args.Key == Keys.Down)
                args.MC.LabelManager.SelectNext();
        }

        public bool HasActionKeyEvent(MultiboxFunctionParam args)
        {
            return (args.MultiboxText.IndexOf(">>>") <= 1);
        }

        public void RunActionKeyEvent(MultiboxFunctionParam args)
        {
            if (args.MultiboxText.IndexOf(">>>") > 1)
                throw new InvalidOperationException();
            int ind2 = args.MultiboxText.IndexOf("<<");
            if (ind2 <= 1)
            {
                ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
                if (tmp2 != null)
                {
                    string tmpt = tmp2.FullText;
                    if (tmpt.Length > 0 && tmpt[0] == '~')
                        tmpt = args.MC.HomeDirectory + tmpt.Substring(1);
                    Process.Start(tmpt);
                }
            }
            else
            {
                string pth = args.MultiboxText.Substring(1, ind2 - 1);
                string name = "";
                try
                {
                    name = args.MultiboxText.Substring(ind2 + 2);
                }
                catch { }
                if (name.Length > 0)
                    BookmarkList.Add(new BookmarkItem(name, pth));
            }
        }

        public bool HasSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return IsMulti(args);
        }

        public string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
            if (tmp2 != null)
            {
                string tmpt = tmp2.FullText;
                if (tmpt.Length > 0 && tmpt[0] == '~')
                    tmpt = args.MC.HomeDirectory + tmpt.Substring(1);
                return tmpt;
            }
            return null;
        }

        public bool HasSpecialInputCopyHandling(MultiboxFunctionParam args)
        {
            return false;
        }

        public string RunSpecialInputCopyHandling(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}