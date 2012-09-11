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
            if (DebugMode)
                IterateToPath(path, true)[IS_DIRECTORY] = true;
            else
                Directory.CreateDirectory(path);
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
                if (dir == null || !dir.SKeys.Contains(IS_DIRECTORY) || dir[IS_DIRECTORY] == false)
                    return null;
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
            if (DebugMode)
            {
                MyDictionary file = IterateToPath(path, true);
                file[IS_DIRECTORY] = false;
                file[CONTENTS] = text;
                file[SIZE] = text.Length;
                file[TYPE] = "Text";
                file[LAST_WRITE_TIME] = DateTime.Now;
            }
            File.WriteAllText(path, text);
        }

        public static void FileWriteAllLines(string path, string[] lines)
        {
            if (DebugMode)
                FileWriteAllText(path, string.Join(LineEnding, lines));
            else
                File.WriteAllLines(path, lines);
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
            if (IsDirectory(path))
                return null;
            if(DebugMode)
            {
                MyDictionary file = IterateToPath(path, false);
                return (file == null || !file.SKeys.Contains(TYPE)) ? null : file[TYPE];
            }
            return GetFileInfo.GetTypeName(path);
        }

        public static long GetFileSize(string path)
        {
            if (IsDirectory(path) || !FileExists(path))
                return -1;
            return DebugMode ? (long) IterateToPath(path, false)[SIZE] : new FileInfo(path).Length;
        }

        public static DateTime GetFileLastWriteTime(string path)
        {
            if (!FileExists(path)) return default(DateTime);
            return DebugMode ? (DateTime) IterateToPath(path, false)[LAST_WRITE_TIME] : File.GetLastWriteTime(path);
        }
    }
}