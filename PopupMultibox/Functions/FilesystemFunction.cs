using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using PopupMultibox.UI;
using PopupMultibox.helpers;

namespace PopupMultibox.Functions
{
    public class FilesystemFunction : AbstractFunction
    {
        private static void DirSearch(string sDir, string fnd, List<string> itms)
        {
            try
            {
                Regex tmp = new Regex(fnd.Replace(@"\\", @"\\\\").Replace(@".", @"\.").Replace(@"*", @".*").Replace(@"?", @".?").Replace(@"[", @"\[").Replace(@"]", @"\]".Replace(@"(", @"\(").Replace(@")", @"\)")), RegexOptions.IgnoreCase);
                itms.AddRange(Directory.GetFiles(sDir).Where(f => tmp.IsMatch(f.Substring(f.LastIndexOf("\\") + 1))));
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirSearch(d, fnd, itms);
                }
            }
            catch { }
        }

        private static string[] DirList(string sDir, string fnd)
        {
            try
            {
                List<string> itms = new List<string>(0);
                itms.AddRange(from d in Directory.GetDirectories(sDir) where d.StartsWith(fnd) select d.Remove(0, sDir.Length) + "\\");
                itms.AddRange(from f in Directory.GetFiles(sDir) where f.StartsWith(fnd) select f.Remove(0, sDir.Length));
                return itms.ToArray();
            }
            catch { }
            return null;
        }

        private static string[] DirList2(string sDir)
        {
            try
            {
                List<string> itms = new List<string>(0);
                itms.AddRange(Directory.GetDirectories(sDir).Select(d => d + "\\"));
                itms.AddRange(Directory.GetFiles(sDir));
                return itms.ToArray();
            }
            catch { }
            return null;
        }

        private static long GetFileSize(string path)
        {
            try
            {
                FileAttributes fa = File.GetAttributes(path);
                if ((fa & FileAttributes.Directory) != FileAttributes.Directory)
                    return new FileInfo(path).Length;
            }
            catch { }
            return -1;
        }

        private static long GetFileSize2(string path)
        {
            try
            {
                return new FileInfo(path).Length;
            }
            catch { }
            return -1;
        }

        private static bool GetFolderSize(string path, MainClass mc, ref long cs, ref long files, ref long folders, ref DateTime ld, long delay)
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

        private static void SetMCOutputLabelText(MainClass mc, long cs, long files, long folders, string ending)
        {
            int i = 0;
            double size = cs;
            while (size >= 1024 && i < sizeEndings.Length - 1)
            {
                if (sizeEndings[i].ToLower().Equals(ending))
                    break;
                i++;
                size = size / 1024.0;
            }
            mc.OutputLabelText = files + " file(s); " + folders + " folder(s); " + size.ToString("#.###") + sizeEndings[i];
        }

        private static string lastSizePath;
        private static long lastSizeValue = -1;
        private static long lastSizeFiles = -1;
        private static long lastSizeFolders = -1;
        private static DateTime lastSizeTime;
        private static volatile string currentEnding;
        private static readonly string[] sizeEndings = new[] { "B", "KB", "MB", "GB", "TB" };
        private static volatile bool cancelCalc;
        private static volatile bool isCalculating;
        private delegate void SetMCOutputLabelTextDel(MainClass mc, long cs, long files, long folders, string ending);

        public FilesystemFunction()
        {
            BookmarkList.Load();
        }

        #region IMultiboxFunction Members

        public override bool Triggers(MultiboxFunctionParam args)
        {
            return (!string.IsNullOrEmpty(args.MultiboxText) && args.MultiboxText[0] == ':');
        }

        public override bool IsMulti(MultiboxFunctionParam args)
        {
            return ((args.MultiboxText.IndexOf(">>>") <= 1) && (args.MultiboxText.IndexOf("<<") <= 1));
        }

        public override bool IsBackgroundStream(MultiboxFunctionParam args)
        {
            return (args.MultiboxText.IndexOf(">>>") > 1);
        }

        public override bool ShouldRun(MultiboxFunctionParam args)
        {
            return !(args.Key == Keys.Up || args.Key == Keys.Down);
        }

        public override string RunSingle(MultiboxFunctionParam args)
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

        public override List<ResultItem> RunMulti(MultiboxFunctionParam args)
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
                        tmp.AddRange(pths.Select(tpth => new ResultItem(tpth, pth3 + tpth, pth2 + tpth)));
                        return tmp;
                    }
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
                    if (rlts.Count > 0)
                    {
                        List<ResultItem> tmp = new List<ResultItem>(0);
                        tmp.AddRange(rlts.Select(tpth => new ResultItem(tpth.Substring(tpth.LastIndexOf("\\") + 1), tpth, ht ? tpth.Replace(args.MC.HomeDirectory, "~") : tpth)));
                        return tmp;
                    }
                }
            }
            return null;
        }

        public override void RunSingleBackgroundStream(MultiboxFunctionParam args)
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
                if (args.Key != Keys.Tab && lastSizePath != null && lastSizePath.Equals(pth) && isCalculating)
                    return;
                cancelCalc = true;
                Thread.Sleep(10);
                cancelCalc = false;
                if (args.Key == Keys.Tab || lastSizeValue <= 0 || lastSizePath == null || !lastSizePath.Equals(pth) || (DateTime.Now - lastSizeTime).TotalMinutes >= 5)
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
                if (lastSizeValue <= 0)
                {
                    args.MC.OutputLabelText = "Invalid selection";
                    return;
                }
                SetMCOutputLabelText(args.MC, lastSizeValue, lastSizeFiles, lastSizeFolders, currentEnding);
                args.MC.UpdateSize();
            }
        }

        public override bool HasDetails(MultiboxFunctionParam args)
        {
            return true;
        }

        public override string GetDetails(MultiboxFunctionParam args)
        {
            string pth = args.MC.LabelManager.CurrentSelection.FullText;
            if (pth.Length > 0 && pth[0] == '~')
                pth = args.MC.HomeDirectory + pth.Substring(1);
            long sz = GetFileSize(pth);
            double size = sz;
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
            string typ;
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

        public override bool SupressKeyPress(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Up || args.Key == Keys.Down || args.Key == Keys.Tab);
        }

        public override bool HasKeyDownAction(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Up || args.Key == Keys.Down);
        }

        public override void RunKeyDownAction(MultiboxFunctionParam args)
        {
            if (args.Key == Keys.Up)
                args.MC.LabelManager.SelectPrev();
            else if (args.Key == Keys.Down)
                args.MC.LabelManager.SelectNext();
        }

        public override bool HasActionKeyEvent(MultiboxFunctionParam args)
        {
            return (args.MultiboxText.IndexOf(">>>") <= 1);
        }

        public override void RunActionKeyEvent(MultiboxFunctionParam args)
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

        public override bool HasSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return IsMulti(args);
        }

        public override string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
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

        #endregion
    }
}