using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using PopupMultibox.UI;
using PopupMultibox.helpers;

namespace PopupMultibox.Functions
{
    public class FilesystemFunction : AbstractFunction
    {
        public FilesystemFunction()
        {
            BookmarkList.Load();
        }

        #region IMultiboxFunction Members

        public override bool Triggers(MultiboxFunctionParam args)
        {
            return (!String.IsNullOrEmpty(args.MultiboxText) && args.MultiboxText[0] == ':');
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
            if (IsBookmark(args))
            {
                return GetBookmarkString(args, args.MultiboxText.IndexOf("<<"));
            }
            throw new InvalidOperationException();
        }

        private static bool IsBookmark(MultiboxFunctionParam args)
        {
            return args.MultiboxText.IndexOf(">>>") <= 1 && args.MultiboxText.IndexOf("<<") > 1;
        }

        private static string GetBookmarkString(MultiboxFunctionParam args, int ind2)
        {
            string pth = args.MultiboxText.Substring(1, ind2 - 1);
            string pth2 = pth.Substring(0, pth.LastIndexOf("\\") + 1);
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
            if ((args.MultiboxText.IndexOf(">>>") > 1) || (args.MultiboxText.IndexOf("<<") > 1))
                throw new InvalidOperationException();
            AutocompleteIfNeeded(args);
            int ind = args.MultiboxText.IndexOf(">> ");
            if (args.Key == Keys.Up || args.Key == Keys.Down)
                return null;
            if (ind <= 1)
            {
                string pth = args.MultiboxText.Substring(1);
                if (!pth.Contains("\\"))
                {
                    return DeleteIfNeeded(args, ref pth) ? null : GetDriveList(pth);
                }
                pth = DeleteIfNeeded2(args, pth);
                return GetFileList(args, pth);
            }
            return GetSearchResults(args, ind);
        }

        private static List<ResultItem> GetSearchResults(MultiboxFunctionParam args, int ind)
        {
            string pth = args.MultiboxText.Substring(1, ind - 1);
            string pth2 = pth.Substring(0, pth.LastIndexOf("\\") + 1);
            string pth3 = FileList.ExtendPath(args, pth2);
            string fnd = args.MultiboxText.Substring(ind + 3);
            fnd = ExactMatchIfNeeded(fnd);
            List<string> rlts = new List<string>(0);
            FileList.DirSearch(pth3, fnd, rlts);
            if (rlts.Count <= 0)
                return null;
            List<ResultItem> tmp = new List<ResultItem>(0);
            tmp.AddRange(rlts.Select(tpth => new ResultItem(tpth.Substring(tpth.LastIndexOf("\\") + 1), tpth, (pth2.Length > 0 && pth2[0] == '~') ? tpth.Replace(args.MC.HomeDirectory, "~") : tpth)));
            return tmp;
        }

        private static string ExactMatchIfNeeded(string fnd)
        {
            if (fnd.Length > 0 && fnd[0] == '/')
                fnd = @"^" + fnd.Substring(1) + @"$";
            return fnd;
        }

        private static List<ResultItem> GetFileList(MultiboxFunctionParam args, string pth)
        {
            string pth2 = pth.Substring(0, pth.LastIndexOf("\\") + 1);
            string pth3 = FileList.ExtendPath(args, pth2);
            string pth4 = FileList.ExtendPath(args, pth);
            string[] pths = FileList.DirList(pth3, pth4);
            if (pths == null)
                return null;
            List<ResultItem> tmp = new List<ResultItem>(0);
            tmp.AddRange(pths.Select(tpth => new ResultItem(tpth, pth3 + tpth, pth2 + tpth)));
            return tmp;
        }

        private static string DeleteIfNeeded2(MultiboxFunctionParam args, string pth)
        {
            if (args.Key == Keys.Back && args.Control)
            {
                pth = pth.EndsWith("\\") ? pth.Remove(pth.LastIndexOf("\\", pth.Length - 2) + 1) : pth.Remove(pth.LastIndexOf("\\") + 1);
                args.MC.InputFieldText = ":" + pth;
            }
            return pth;
        }

        private static List<ResultItem> GetDriveList(string pth)
        {
            string[] pths = FileList.DriveList(pth);
            if (pths != null)
            {
                List<ResultItem> tmp = new List<ResultItem>(0);
                tmp.AddRange(pths.Select(tpth => new ResultItem(tpth, tpth, tpth)));
                return tmp;
            }
            return null;
        }

        private static bool DeleteIfNeeded(MultiboxFunctionParam args, ref string pth)
        {
            if (args.Key == Keys.Back && args.Control)
            {
                if (String.IsNullOrEmpty(pth))
                {
                    args.MC.InputFieldText = "";
                    return true;
                }
                args.MC.InputFieldText = ":";
                pth = "";
            }
            return false;
        }

        private static void AutocompleteIfNeeded(MultiboxFunctionParam args)
        {
            if (args.Key != Keys.Tab) return;
            ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
            if (tmp2 != null)
                args.MC.InputFieldText = ":" + tmp2.EvalText;
        }

        public override void RunSingleBackgroundStream(MultiboxFunctionParam args)
        {
            int ind = args.MultiboxText.IndexOf(">>>");
            if (ind <= 1)
                return;
            string pth = FileList.ExtendPath(args, args.MultiboxText.Substring(1, ind - 1));
            string ending = GetEnding(args, ind);
            FileList.CurrentEnding = ending;
            if (IsCalculatingSamePath(args, pth))
                return;
            CancelCurrentCalc();
            if (CalculateIfNeeded(args, pth)) return;
            if (IsInvalidSize())
            {
                args.MC.OutputLabelText = "Invalid selection";
                return;
            }
            FileList.SetMCOutputLabelText(args.MC, FileList.LastSizeValue, FileList.LastSizeFiles, FileList.LastSizeFolders, FileList.CurrentEnding);
            args.MC.UpdateSize();
        }

        private static void CancelCurrentCalc()
        {
            FileList.CancelCalc = true;
            Thread.Sleep(10);
            FileList.CancelCalc = false;
        }

        private static bool IsInvalidSize()
        {
            return FileList.LastSizeValue <= 0;
        }

        private static bool CalculateIfNeeded(MultiboxFunctionParam args, string pth)
        {
            if (ShouldRecalculate(args, pth))
            {
                FileList.LastSizePath = pth;
                args.MC.OutputLabelText = "Calculating size, please wait...";
                args.MC.UpdateSize();
                DateTime ld = DateTime.Now;
                ResetLastSizeFields();
                CalculateSize(args, ld, pth);
                if (FileList.CancelCalc)
                    return true;
                FileList.LastSizeTime = DateTime.Now;
            }
            return false;
        }

        private static void CalculateSize(MultiboxFunctionParam args, DateTime ld, string pth)
        {
            FileList.IsCalculating = true;
            if (!FileList.GetFolderSize(pth, args.MC, ref ld, 500))
                InvalidateLastSizeFields();
            FileList.IsCalculating = false;
        }

        private static void InvalidateLastSizeFields()
        {
            FileList.LastSizeValue = -1;
            FileList.LastSizeFiles = -1;
            FileList.LastSizeFolders = -1;
        }

        private static void ResetLastSizeFields()
        {
            FileList.LastSizeValue = 0;
            FileList.LastSizeFiles = 0;
            FileList.LastSizeFolders = 0;
        }

        private static bool ShouldRecalculate(MultiboxFunctionParam args, string pth)
        {
            return args.Key == Keys.Tab || FileList.LastSizeValue <= 0 || FileList.LastSizePath == null || !FileList.LastSizePath.Equals(pth) || (DateTime.Now - FileList.LastSizeTime).TotalMinutes >= 5;
        }

        private static bool IsCalculatingSamePath(MultiboxFunctionParam args, string pth)
        {
            return args.Key != Keys.Tab && FileList.LastSizePath != null && FileList.LastSizePath.Equals(pth) && FileList.IsCalculating;
        }

        private static string GetEnding(MultiboxFunctionParam args, int ind)
        {
            string ending = FileList.SizeEndings[FileList.SizeEndings.Length - 1];
            try
            {
                ending = args.MultiboxText.Substring(ind + 3);
            }
            catch {}
            ending = ending.ToLower();
            return ending;
        }

        public override bool HasDetails(MultiboxFunctionParam args)
        {
            return true;
        }

        public override string GetDetails(MultiboxFunctionParam args)
        {
            string pth = FileList.ExtendPath(args, args.MC.LabelManager.CurrentSelection.FullText);
            double size = FileList.GetFileSize(pth);
            string sizestr = GetSizestr(pth, size);
            string typ = GetElementType(pth, size);
            string lmd = "";
            try
            {
                DateTime lmddt = File.GetLastWriteTime(pth);
                lmd = lmddt.ToShortDateString() + " " + lmddt.ToLongTimeString();
            }
            catch { }
            return "Name: " + args.MC.LabelManager.CurrentSelection.DisplayText + "\nType: " + typ + "\nSize: " + sizestr + "\nLast Modified: " + lmd;
        }

        private static string GetSizestr(string pth, double size)
        {
            string sizestr = "--";
            if (size > 0)
                sizestr = FileList.FormatSizeStr(size);
            else if (pth.EndsWith(":\\"))
                sizestr = FileList.DriveSizeStr(pth);
            return sizestr;
        }

        private static string GetElementType(string pth, double size)
        {
            if (size > 0)
            {
                try
                {
                    return GetFileInfo.GetTypeName(pth);
                }
                catch
                {
                    return "File";
                }
            }
            return pth.EndsWith(":\\") ? "Drive" : "Folder";
        }

        public override bool SupressKeyPress(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Up || args.Key == Keys.Down || args.Key == Keys.Tab || (args.Key == Keys.Back && args.Control));
        }

        public override bool HasKeyDownAction(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Up || args.Key == Keys.Down);
        }

        public override void RunKeyDownAction(MultiboxFunctionParam args)
        {
            switch (args.Key)
            {
                case Keys.Up:
                    args.MC.LabelManager.SelectPrev();
                    break;
                case Keys.Down:
                    args.MC.LabelManager.SelectNext();
                    break;
            }
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
                OpenSelectedFile(args);
            else
                CreateBookmark(args, ind2);
        }

        private static void CreateBookmark(MultiboxFunctionParam args, int ind2)
        {
            string pth = args.MultiboxText.Substring(1, ind2 - 1);
            string name = "";
            try
            {
                name = args.MultiboxText.Substring(ind2 + 2);
            }
            catch {}
            if (name.Length > 0)
                BookmarkList.Add(new BookmarkItem(name, pth));
        }

        private static void OpenSelectedFile(MultiboxFunctionParam args)
        {
            ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
            if (tmp2 == null)
                return;
            string tmpt = FileList.ExtendPath(args, tmp2.FullText);
            Process.Start(tmpt);
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
                string tmpt = FileList.ExtendPath(args, tmp2.FullText);
                return tmpt;
            }
            return null;
        }

        #endregion
    }
}