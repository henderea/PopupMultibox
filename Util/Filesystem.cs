using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Henderson.Util.MyDictionary;

namespace Multibox.Plugin.Util
{
    public class Filesystem
    {
        private static MyDictionary mockFilesystem;
        private static string userProfile;
        private static List<Drive> drives;
        private static Dictionary<Environment.SpecialFolder, string> folderPaths; 
        public static bool DebugMode { get; set; }
        private const string SEPARATOR = "\\";
        private const string DEFAULT_LINE_ENDING = "\n";
        private const string IS_DIRECTORY = "@IsDirectory";
        private const string CONTENTS = "@Contents";
        private const string SIZE = "@Size";
        private const string TYPE = "@Type";
        private const string LAST_WRITE_TIME = "@LastWriteTime";

        public static string UserProfile
        {
            get
            {
                return userProfile ?? (userProfile = Environment.GetEnvironmentVariable("USERPROFILE"));
            }
            set
            {
                userProfile = value;
            }
        }

        public static string LineEnding { get; set; }

        static Filesystem()
        {
            Reset();
        }

        public static void Reset()
        {
            mockFilesystem = new MyDictionary();
            userProfile = null;
            drives = new List<Drive>(0);
            folderPaths = new Dictionary<Environment.SpecialFolder, string>(0);
            LineEnding = DEFAULT_LINE_ENDING;
        }

        private static string[] ParsePath(string path)
        {
            try
            {
                return path.Split(new[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch {}
            return null;
        }

        private static MyDictionary IterateToPath(string[] parts, bool create)
        {
            MyDictionary temp = mockFilesystem;
            foreach (string part in parts)
            {
                if (!create && !temp.SKeys.Contains(part)) return null;
                temp = temp[part];
            }
            return temp;
        }

        private static MyDictionary IterateToPath(string path, bool create)
        {
            string[] parts = ParsePath(path);
            return IterateToPath(parts, create);
        }

        public static bool DirectoryExists(string path)
        {
            return DebugMode ? IsDirectory(path) : Directory.Exists(path);
        }

        public static void CreateDirectory(string path)
        {
            if (DebugMode) IterateToPath(path, true)[IS_DIRECTORY] = true;
            else Directory.CreateDirectory(path);
        }

        public static string[] GetFiles(string path)
        {
            if(DebugMode)
            {
                MyDictionary dir = IterateToPath(path, false);
                if (dir == null || !dir.SKeys.Contains(IS_DIRECTORY) || dir[IS_DIRECTORY] == false)
                    return null;
                List<string> files = new List<string>(0);
                files.AddRange((from KeyValuePair<MyKey, MyDictionary> val in dir where !(val.Key + "").StartsWith("@") && val.Value[IS_DIRECTORY] != true select val.Key).Select(dummy => (string) dummy));
                return files.ToArray();
            }
            return Directory.GetFiles(path);
        }

        public static string[] GetDirectories(string path)
        {
            if (DebugMode)
            {
                MyDictionary dir = IterateToPath(path, false);
                if (dir == null || !dir.SKeys.Contains(IS_DIRECTORY) || dir[IS_DIRECTORY] == false) return null;
                List<string> files = new List<string>(0);
                files.AddRange((from KeyValuePair<MyKey, MyDictionary> val in dir where !(val.Key + "").StartsWith("@") && val.Value[IS_DIRECTORY] == true select val.Key).Select(dummy => (string)dummy));
                return files.ToArray();
            }
            return Directory.GetDirectories(path);
        }

        public static bool IsDirectory(string path)
        {
            if (DebugMode)
            {
                MyDictionary dir = IterateToPath(path, false);
                return (dir != null && dir.SKeys.Contains(IS_DIRECTORY) && dir[IS_DIRECTORY] == true);
            }
            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public static void FileWriteAllText(string path, string text)
        {
            if (DebugMode) DebugAddFile(path, text, text.Length, "Text", DateTime.Now);
            File.WriteAllText(path, text);
        }

        public static void FileWriteAllLines(string path, string[] lines)
        {
            if (DebugMode) FileWriteAllText(path, string.Join(LineEnding, lines));
            else File.WriteAllLines(path, lines);
        }

        public static string FileReadAllText(string path)
        {
            if(DebugMode)
            {
                MyDictionary file = IterateToPath(path, false);
                if (file == null || !file.SKeys.Contains(IS_DIRECTORY) || file[IS_DIRECTORY] == true || !file.SKeys.Contains(CONTENTS))
                    return null;
                return file[CONTENTS];
            }
            return File.ReadAllText(path);
        }

        public static string[] FileReadAllLines(string path)
        {
            return DebugMode ? FileReadAllText(path).Split(new[] { LineEnding }, StringSplitOptions.None) : File.ReadAllLines(path);
        }

        public static bool FileExists(string path)
        {
            if(DebugMode)
            {
                MyDictionary file = IterateToPath(path, false);
                return (file != null || file[IS_DIRECTORY] == false);
            }
            return File.Exists(path);
        }

        public static string GetFileType(string path)
        {
            if (IsDirectory(path) || !FileExists(path)) return null;
            return DebugMode ? (string) IterateToPath(path, false)[TYPE] : GetFileInfo.GetTypeName(path);
        }

        public static long GetFileSize(string path)
        {
            if (IsDirectory(path) || !FileExists(path)) return -1;
            return DebugMode ? (long) IterateToPath(path, false)[SIZE] : new FileInfo(path).Length;
        }

        public static DateTime GetFileLastWriteTime(string path)
        {
            if (!FileExists(path)) return default(DateTime);
            return DebugMode ? (DateTime) IterateToPath(path, false)[LAST_WRITE_TIME] : File.GetLastWriteTime(path);
        }

        public static void DebugAddDrive(Drive drive)
        {
            if (DebugMode && drive != null) drives.Add(drive);
        }

        public static Drive[] GetDrives()
        {
            if(DebugMode) return drives.ToArray();
            DriveInfo[] infos = DriveInfo.GetDrives();
            List<Drive> tmp = new List<Drive>(0);
            tmp.AddRange(infos.Select(info => new Drive(info)));
            return tmp.ToArray();
        }

        public static void DebugAddFolderPath(Environment.SpecialFolder folder, string path)
        {
            if (DebugMode && path != null) folderPaths[folder] = path;
        }

        public static string GetFolderPath(Environment.SpecialFolder folder)
        {
            return DebugMode ? (folderPaths.ContainsKey(folder) ? folderPaths[folder] : null) : Environment.GetFolderPath(folder);
        }

        public static void DebugAddFile(string path, string contents, long size, string type, DateTime lastWriteTime)
        {
            if (!DebugMode) return;
            MyDictionary file = IterateToPath(path, true);
            file[IS_DIRECTORY] = false;
            file[CONTENTS] = contents;
            file[SIZE] = size;
            file[TYPE] = type;
            file[LAST_WRITE_TIME] = lastWriteTime;
        }
    }

    public class Drive
    {
        public long AvailableFreeSpace { get; private set; }
        public string DriveFormat { get; private set; }
        public DriveType DriveType { get; private set; }
        public bool IsReady { get; private set; }
        public string Name { get; private set; }
        public long TotalFreeSpace { get; private set; }
        public long TotalSize { get; private set; }
        public string VolumeLabel { get; private set; }

        public Drive(DriveInfo di)
        {
            AvailableFreeSpace = di.AvailableFreeSpace;
            DriveFormat = di.DriveFormat;
            DriveType = di.DriveType;
            IsReady = di.IsReady;
            Name = di.Name;
            TotalFreeSpace = di.TotalFreeSpace;
            TotalSize = di.TotalSize;
            VolumeLabel = di.VolumeLabel;
        }

        public Drive(long availableFreeSpace, string driveFormat, DriveType driveType, bool isReady, string name, long totalFreeSpace, long totalSize, string volumeLabel)
        {
            AvailableFreeSpace = availableFreeSpace;
            DriveFormat = driveFormat;
            DriveType = driveType;
            IsReady = isReady;
            Name = name;
            TotalFreeSpace = totalFreeSpace;
            TotalSize = totalSize;
            VolumeLabel = volumeLabel;
        }
    }
}