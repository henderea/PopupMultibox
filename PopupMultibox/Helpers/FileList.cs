using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PopupMultibox.Functions;
using PopupMultibox.UI;

namespace PopupMultibox.helpers
{
    public class FileList
    {
        public static void DirSearch(string sDir, string fnd, List<string> itms)
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

        public static string[] DirList(string sDir, string fnd)
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

        public static string[] DirList2(string sDir)
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

        public static string[] DriveList(string fnd)
        {
            try
            {
                List<string> itms = new List<string>(0);
                itms.AddRange(from di in DriveInfo.GetDrives() where String.IsNullOrEmpty(fnd) || di.Name.StartsWith(fnd) select di.Name);
                return itms.ToArray();
            }
            catch {}
            return null;
        }

        public static string DriveSizeStr(string driveName)
        {
            try
            {
                foreach (DriveInfo di in DriveInfo.GetDrives())
                {
                    if (di.Name.Equals(driveName))
                        return FormatSizeStr(di.TotalSize - di.TotalFreeSpace) + " / " + FormatSizeStr(di.TotalSize);
                }
            }
            catch {}
            return "--";
        }

        public static long GetFileSize(string path)
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

        public static long GetFileSize2(string path)
        {
            try
            {
                return new FileInfo(path).Length;
            }
            catch { }
            return -1;
        }

        public static bool GetFolderSize(string path, MainClass mc, ref DateTime ld, long delay)
        {
            try
            {
                if (CancelCalc)
                    return false;
                FileAttributes fa = File.GetAttributes(path);
                if ((fa & FileAttributes.Directory) != FileAttributes.Directory)
                {
                    GetFileSize2(path);
                    LastSizeFiles++;
                    DateTime nw = DateTime.Now;
                    if ((nw - ld).TotalMilliseconds > delay)
                    {
                        ld = nw;
                        if (CancelCalc)
                            return false;
                        UpdateLabel(mc, LastSizeValue, LastSizeFiles, LastSizeFolders);
                    }
                    return true;
                }
                LastSizeFolders++;
                string[] itms = DirList2(path);
                if (itms == null || itms.Length <= 0 || CancelCalc)
                    return false;
                foreach (string itm in itms)
                {
                    if (CancelCalc)
                        return false;
                    GetFolderSize(itm, mc, ref ld, delay);
                }
                return true;
            }
            catch { }
            return false;
        }

        public static readonly string[] SizeEndings = new[] { "B", "KB", "MB", "GB", "TB" };
        private static volatile bool cancelCalc;
        private static volatile bool isCalculating;
        private static long lastSizeValue = -1;
        private static long lastSizeFiles = -1;
        private static long lastSizeFolders = -1;
        private static volatile string currentEnding;

        public static bool CancelCalc
        {
            get
            {
                return cancelCalc;
            }
            set
            {
                cancelCalc = value;
            }
        }

        public static bool IsCalculating
        {
            get
            {
                return isCalculating;
            }
            set
            {
                isCalculating = value;
            }
        }

        public static string LastSizePath { get; set; }

        public static long LastSizeValue
        {
            get
            {
                return lastSizeValue;
            }
            set
            {
                lastSizeValue = value;
            }
        }

        public static long LastSizeFiles
        {
            get
            {
                return lastSizeFiles;
            }
            set
            {
                lastSizeFiles = value;
            }
        }

        public static long LastSizeFolders
        {
            get
            {
                return lastSizeFolders;
            }
            set
            {
                lastSizeFolders = value;
            }
        }

        public static DateTime LastSizeTime { get; set; }

        public static string CurrentEnding
        {
            get
            {
                return currentEnding;
            }
            set
            {
                currentEnding = value;
            }
        }

        public static string FormatSizeStr(double size)
        {
            int i = 0;
            while (size >= 1024 && i < SizeEndings.Length - 1)
            {
                if (SizeEndings[i].ToLower().Equals(CurrentEnding))
                    break;
                i++;
                size = size / 1024.0;
            }
            return size.ToString("#.###") + SizeEndings[i];
        }

        public static void SetMCOutputLabelText(MainClass mc, long cs, long files, long folders, string ending)
        {
            mc.OutputLabelText = files + " file(s); " + folders + " folder(s); " + FormatSizeStr(cs);
        }

        public delegate void SetMCOutputLabelTextDel(MainClass mc, long cs, long files, long folders, string ending);

        public static string ExtendPath(MultiboxFunctionParam args, string pth)
        {
            if (pth.Length > 0 && pth[0] == '~')
                pth = args.MC.HomeDirectory + pth.Substring(1);
            return pth;
        }

        public static void UpdateLabel(MainClass mc, long cs, long files, long folders)
        {
            new SetMCOutputLabelTextDel(SetMCOutputLabelText).BeginInvoke(mc, cs, files, folders, CurrentEnding, null, null);
        }
    }
}