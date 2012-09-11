using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using Multibox.Core.Functions;
using Multibox.Core.UI;
using Multibox.Plugin.Util;

namespace Multibox.Plugin.FilesystemFunction
{
    public class FilesystemFunction : AbstractFunction
    {
        public override int SuggestedIndex()
        {
            return 1;
        }

        private static void DirSearch(string sDir, string fnd, List<string> itms)
        {
            try
            {
                Regex tmp = new Regex(fnd.Replace(@"\\", @"\\\\").Replace(@".", @"\.").Replace(@"*", @".*").Replace(@"?", @".?").Replace(@"[", @"\[").Replace(@"]", @"\]".Replace(@"(", @"\(").Replace(@")", @"\)")), RegexOptions.IgnoreCase);
                itms.AddRange(Filesystem.GetFiles(sDir).Where(f => tmp.IsMatch(f.Substring(f.LastIndexOf("\\") + 1))));
                foreach (string d in Filesystem.GetDirectories(sDir))
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
                itms.AddRange(from d in Filesystem.GetDirectories(sDir) where d.StartsWith(fnd) select d.Remove(0, sDir.Length) + "\\");
                itms.AddRange(from f in Filesystem.GetFiles(sDir) where f.StartsWith(fnd) select f.Remove(0, sDir.Length));
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
                itms.AddRange(Filesystem.GetDirectories(sDir).Select(d => d + "\\"));
                itms.AddRange(Filesystem.GetFiles(sDir));
                return itms.ToArray();
            }
            catch { }
            return null;
        }

        private static string[] DriveList(string fnd)
        {
            try
            {
                List<string> itms = new List<string>(0);
                itms.AddRange(from di in DriveInfo.GetDrives() where string.IsNullOrEmpty(fnd) || di.Name.StartsWith(fnd) select di.Name);
                return itms.ToArray();
            }
            catch {}
            return null;
        }

        private static long GetFileSize(string path)
        {
            try
            {
                return Filesystem.GetFileSize(path);
            }
            catch { }
            return -1;
        }

        private static bool GetFolderSize(string path, MainClass mc, ref long cs, ref long files, ref long folders, ref DateTime ld, long delay)
        {
            try
            {
                if (cancelCalc) return false;
                if (Filesystem.FileExists(path))
                {
                    cs += GetFileSize(path);
                    files++;
                    DateTime nw = DateTime.Now;
                    if ((nw - ld).TotalMilliseconds > delay)
                    {
                        ld = nw;
                        if (cancelCalc) return false;
                        new SetMCOutputLabelTextDel(SetMCOutputLabelText).BeginInvoke(mc, cs, files, folders, currentEnding, null, null);
                    }
                    return true;
                }
                folders++;
                string[] itms = DirList2(path);
                if (itms == null || itms.Length <= 0 || cancelCalc) return false;
                foreach (string itm in itms)
                {
                    if (cancelCalc) return false;
                    GetFolderSize(itm, mc, ref cs, ref files, ref folders, ref ld, delay);
                }
                return true;
            }
            catch { }
            return false;
        }

        private static void SetMCOutputLabelText(MainClass mc, long cs, long files, long folders, string ending)
        {
            mc.OutputLabelText = files.ToString("#,##0") + " file(s); " + folders.ToString("#,##0") + " folder(s); " + FormatSizestr(cs);
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

        public override string RunSingle(MultiboxFunctionParam args)
        {
            if (args.MultiboxText.IndexOf(">>>") <= 1 && args.MultiboxText.IndexOf("<<") > 1) return GetBookmarkString(args, args.MultiboxText.IndexOf("<<"));
            throw new InvalidOperationException();
        }

        private static string GetBookmarkString(MultiboxFunctionParam args, int ind2)
        {
            string pth2 = args.MultiboxText.Substring(1, ind2 - 1).Substring(0, args.MultiboxText.Substring(1, ind2 - 1).LastIndexOf("\\") + 1);
            string name = "";
            try
            {
                name = args.MultiboxText.Substring(ind2 + 2);
            }
            catch {}
            return "Bookmark " + pth2 + " as \"" + name + "\"";
        }

        public override List<ResultItem> RunMulti(MultiboxFunctionParam args)
        {
            if ((args.MultiboxText.IndexOf(">>>") > 1) || (args.MultiboxText.IndexOf("<<") > 1)) throw new InvalidOperationException();
            AutocompleteIfNeeded(args);
            int ind = args.MultiboxText.IndexOf(">> ");
            if (args.Key != Keys.Up && args.Key != Keys.Down)
            {
                if (ind <= 1)
                {
                    string pth = args.MultiboxText.Substring(1);
                    if (!pth.Contains("\\")) return BackspaceIfNeededAndGetFileResultItems(args, pth);
                    pth = BackspaceIfNeeded(args, pth);
                    string pth2 = pth.Substring(0, pth.LastIndexOf("\\") + 1);
                    string pth3 = AddHDIfNeeded(args, pth2);
                    string pth4 = AddHDIfNeeded(args, pth);
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
                    string pth3 = AddHDIfNeeded(args, pth2);
                    string fnd = args.MultiboxText.Substring(ind + 3);
                    if (fnd.Length > 0 && fnd[0] == '/')
                        fnd = @"^" + fnd.Substring(1) + @"$";
                    List<string> rlts = new List<string>(0);
                    DirSearch(pth3, fnd, rlts);
                    if (rlts.Count > 0) return GetSearchResultItems(args, pth2.Length > 0 && pth2[0] == '~', rlts);
                }
            }
            return null;
        }

        private static List<ResultItem> GetSearchResultItems(MultiboxFunctionParam args, bool ht, List<string> rlts)
        {
            List<ResultItem> tmp = new List<ResultItem>(0);
            tmp.AddRange(rlts.Select(tpth => new ResultItem(tpth.Substring(tpth.LastIndexOf("\\") + 1), tpth, ht ? tpth.Replace(Filesystem.UserProfile, "~") : tpth)));
            return tmp;
        }

        private static string BackspaceIfNeeded(MultiboxFunctionParam args, string pth)
        {
            if (args.Key == Keys.Back && args.Control)
            {
                pth = pth.EndsWith("\\") ? pth.Remove(pth.LastIndexOf("\\", pth.Length - 2) + 1) : pth.Remove(pth.LastIndexOf("\\") + 1);
                args.MC.InputFieldText = ":" + pth;
            }
            return pth;
        }

        private static string AddHDIfNeeded(MultiboxFunctionParam args, string pth)
        {
            return pth.Length > 0 && pth[0] == '~' ? Filesystem.UserProfile + pth.Substring(1) : pth;
        }

        private static List<ResultItem> BackspaceIfNeededAndGetFileResultItems(MultiboxFunctionParam args, string pth)
        {
            if (args.Key == Keys.Back && args.Control)
            {
                if (string.IsNullOrEmpty(pth))
                {
                    args.MC.InputFieldText = "";
                    return null;
                }
                args.MC.InputFieldText = ":";
                pth = "";
            }
            return GetFileResultItems(pth);
        }

        private static List<ResultItem> GetFileResultItems(string pth)
        {
            string[] pths = DriveList(pth);
            if (pths == null) return null;
            List<ResultItem> tmp = new List<ResultItem>(0);
            tmp.AddRange(pths.Select(tpth => new ResultItem(tpth, tpth, tpth)));
            return tmp;
        }

        private static void AutocompleteIfNeeded(MultiboxFunctionParam args)
        {
            if (args.Key != Keys.Tab) return;
            ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
            if (tmp2 == null) return;
            args.MC.InputFieldText = ":" + tmp2.EvalText;
        }

        public override void RunSingleBackgroundStream(MultiboxFunctionParam args)
        {
            int ind = args.MultiboxText.IndexOf(">>>");
            if (ind <= 1) return;
            string pth = AddHDIfNeeded(args, args.MultiboxText.Substring(1, ind - 1));
            currentEnding = GetEnding(args, ind);
            if (args.Key != Keys.Tab && lastSizePath != null && lastSizePath.Equals(pth) && isCalculating) return;
            cancelCalc = true;
            Thread.Sleep(10);
            cancelCalc = false;
            if (args.Key == Keys.Tab || lastSizeValue <= 0 || lastSizePath == null || !lastSizePath.Equals(pth) || (DateTime.Now - lastSizeTime).TotalMinutes >= 5)
            {
                lastSizePath = pth;
                args.MC.OutputLabelText = "Calculating size, please wait...";
                args.MC.UpdateSize();
                DateTime ld = DateTime.Now;
                SetLastSizeValues(0);
                isCalculating = true;
                if (!GetFolderSize(pth, args.MC, ref lastSizeValue, ref lastSizeFiles, ref lastSizeFolders, ref ld, 500))
                    SetLastSizeValues(-1);
                isCalculating = false;
                if (cancelCalc) return;
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

        private static void SetLastSizeValues(int val)
        {
            lastSizeValue = val;
            lastSizeFiles = val;
            lastSizeFolders = val;
        }

        private static string GetEnding(MultiboxFunctionParam args, int ind)
        {
            try
            {
                return args.MultiboxText.Substring(ind + 3).ToLower();
            }
            catch {}
            return sizeEndings[sizeEndings.Length - 1].ToLower();
        }

        public override bool HasDetails(MultiboxFunctionParam args)
        {
            return true;
        }

        public override string GetDetails(MultiboxFunctionParam args)
        {
            string pth = AddHDIfNeeded(args, args.MC.LabelManager.CurrentSelection.FullText);
            long sz = GetFileSize(pth);
            string sizestr = FormatSizestr(sz);
            string typ = GetTypeString(pth, sz);
            if(sz <= 0 && IsDrive(pth))
            {
                try
                {
                    foreach (DriveInfo di in DriveInfo.GetDrives())
                    {
                        if (!di.Name.Equals(pth)) continue;
                        sizestr = FormatSizestr(di.TotalSize - di.TotalFreeSpace) + " / " + FormatSizestr(di.TotalSize);
                        break;
                    }
                }
                catch { }
            }
            string lmd = "";
            try
            {
                DateTime lmddt = Filesystem.GetFileLastWriteTime(pth);
                lmd = lmddt.ToShortDateString() + " " + lmddt.ToLongTimeString();
            }
            catch { }
            return "Name: " + args.MC.LabelManager.CurrentSelection.DisplayText + "\nType: " + typ + "\nSize: " + sizestr + "\nLast Modified: " + lmd;
        }

        private static string GetTypeString(string pth, long sz)
        {
            if (sz > 0)
            {
                try
                {
                    return Filesystem.GetFileType(pth);
                }
                catch
                {
                    return "File";
                }
            }
            return IsDrive(pth) ? "Drive" : "Folder";
        }

        private static bool IsDrive(string pth)
        {
            return pth.EndsWith(":\\");
        }

        private static string FormatSizestr(double size)
        {
            if (size <= 0) return "--";
            int i = 0;
            while (size >= 1024 && i < sizeEndings.Length - 1)
            {
                if (sizeEndings[i].ToLower().Equals(currentEnding)) break;
                i++;
                size = size / 1024.0;
            }
            return size.ToString("#,##0.###") + sizeEndings[i];
        }

        public override bool SupressKeyPress(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Tab || (args.Key == Keys.Back && args.Control));
        }

        public override bool HasActionKeyEvent(MultiboxFunctionParam args)
        {
            return (args.MultiboxText.IndexOf(">>>") <= 1);
        }

        public override void RunActionKeyEvent(MultiboxFunctionParam args)
        {
            if (args.MultiboxText.IndexOf(">>>") > 1) throw new InvalidOperationException();
            int ind2 = args.MultiboxText.IndexOf("<<");
            if (ind2 <= 1)
            {
                ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
                if (tmp2 != null)
                    Process.Start(AddHDIfNeeded(args, tmp2.FullText));
                return;
            }
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

        public override bool HasSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return IsMulti(args);
        }

        public override string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return args.MC.LabelManager.CurrentSelection != null ? AddHDIfNeeded(args, args.MC.LabelManager.CurrentSelection.FullText) : null;
        }
    }
}