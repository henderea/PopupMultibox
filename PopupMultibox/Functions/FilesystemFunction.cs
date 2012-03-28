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
                    if (!pth.Contains("\\"))
                    {
                        if (args.Key == Keys.Back && args.Control)
                        {
                            if(String.IsNullOrEmpty(pth))
                            {
                                args.MC.InputFieldText = "";
                                return null;
                            }
                            args.MC.InputFieldText = ":";
                            pth = "";
                        }
                        string[] pths = FileList.DriveList(pth);
                        if (pths != null)
                        {
                            List<ResultItem> tmp = new List<ResultItem>(0);
                            tmp.AddRange(pths.Select(tpth => new ResultItem(tpth, tpth, tpth)));
                            return tmp;
                        }
                        return null;
                    }
                    else
                    {
                        if (args.Key == Keys.Back && args.Control)
                        {
                            pth = pth.EndsWith("\\") ? pth.Remove(pth.LastIndexOf("\\", pth.Length - 2) + 1) : pth.Remove(pth.LastIndexOf("\\")+1);
                            args.MC.InputFieldText = ":" + pth;
                        }
                        string pth2 = pth.Substring(0, pth.LastIndexOf("\\") + 1);
                        string pth3 = FileList.ExtendPath(args, pth2);
                        string pth4 = FileList.ExtendPath(args, pth);
                        string[] pths = FileList.DirList(pth3, pth4);
                        if (pths != null)
                        {
                            List<ResultItem> tmp = new List<ResultItem>(0);
                            tmp.AddRange(pths.Select(tpth => new ResultItem(tpth, pth3 + tpth, pth2 + tpth)));
                            return tmp;
                        }
                    }
                }
                else
                {
                    string pth = args.MultiboxText.Substring(1, ind - 1);
                    string pth2 = pth.Substring(0, pth.LastIndexOf("\\") + 1);
                    string pth3 = FileList.ExtendPath(args, pth2);
                    string fnd = args.MultiboxText.Substring(ind + 3);
                    if (fnd.Length > 0 && fnd[0] == '/')
                        fnd = @"^" + fnd.Substring(1) + @"$";
                    List<string> rlts = new List<string>(0);
                    FileList.DirSearch(pth3, fnd, rlts);
                    if (rlts.Count > 0)
                    {
                        List<ResultItem> tmp = new List<ResultItem>(0);
                        tmp.AddRange(rlts.Select(tpth => new ResultItem(tpth.Substring(tpth.LastIndexOf("\\") + 1), tpth, (pth2.Length > 0 && pth2[0] == '~') ? tpth.Replace(args.MC.HomeDirectory, "~") : tpth)));
                        return tmp;
                    }
                }
            }
            return null;
        }

        public override void RunSingleBackgroundStream(MultiboxFunctionParam args)
        {
            int ind = args.MultiboxText.IndexOf(">>>");
            if (ind <= 1) return;
            string pth = FileList.ExtendPath(args, args.MultiboxText.Substring(1, ind - 1));
            string ending = FileList.SizeEndings[FileList.SizeEndings.Length - 1];
            try
            {
                ending = args.MultiboxText.Substring(ind + 3);
            }
            catch { }
            ending = ending.ToLower();
            FileList.CurrentEnding = ending;
            if (args.Key != Keys.Tab && FileList.LastSizePath != null && FileList.LastSizePath.Equals(pth) && FileList.IsCalculating)
                return;
            FileList.CancelCalc = true;
            Thread.Sleep(10);
            FileList.CancelCalc = false;
            if (args.Key == Keys.Tab || FileList.LastSizeValue <= 0 || FileList.LastSizePath == null || !FileList.LastSizePath.Equals(pth) || (DateTime.Now - FileList.LastSizeTime).TotalMinutes >= 5)
            {
                FileList.LastSizePath = pth;
                args.MC.OutputLabelText = "Calculating size, please wait...";
                args.MC.UpdateSize();
                DateTime ld = DateTime.Now;
                FileList.LastSizeValue = 0;
                FileList.LastSizeFiles = 0;
                FileList.LastSizeFolders = 0;
                FileList.IsCalculating = true;
                if (!FileList.GetFolderSize(pth, args.MC, ref ld, 500))
                {
                    FileList.LastSizeValue = -1;
                    FileList.LastSizeFiles = -1;
                    FileList.LastSizeFolders = -1;
                }
                FileList.IsCalculating = false;
                if (FileList.CancelCalc)
                    return;
                FileList.LastSizeTime = DateTime.Now;
            }
            if (FileList.LastSizeValue <= 0)
            {
                args.MC.OutputLabelText = "Invalid selection";
                return;
            }
            FileList.SetMCOutputLabelText(args.MC, FileList.LastSizeValue, FileList.LastSizeFiles, FileList.LastSizeFolders, FileList.CurrentEnding);
            args.MC.UpdateSize();
        }

        public override bool HasDetails(MultiboxFunctionParam args)
        {
            return true;
        }

        public override string GetDetails(MultiboxFunctionParam args)
        {
            string pth = FileList.ExtendPath(args, args.MC.LabelManager.CurrentSelection.FullText);
            double size = FileList.GetFileSize(pth);
            string sizestr = "--";
            if (size > 0)
            {
                sizestr = FileList.FormatSizeStr(size);
            }
            string typ;
            if (size > 0)
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
            else if(pth.EndsWith(":\\"))
            {
                typ = "Drive";
                sizestr = FileList.DriveSizeStr(pth);
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
            {
                ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
                if (tmp2 != null)
                {
                    string tmpt = FileList.ExtendPath(args, tmp2.FullText);
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
                string tmpt = FileList.ExtendPath(args, tmp2.FullText);
                return tmpt;
            }
            return null;
        }

        #endregion
    }
}