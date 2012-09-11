using System;
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
            LineEnding = "\n";
        }

        private static string[] ParsePath(string path)
        {
            try
            {
                return path.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
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
            if (DebugMode)
                return (IterateToPath(path, false) != null);
            return Directory.Exists(path);
        }

        public static void CreateDirectory(string path)
        {
            if (DebugMode)
                IterateToPath(path, true)["@IsDirectory"] = true;
            else
                Directory.CreateDirectory(path);
        }

        public static void FileWriteAllText(string path, string text)
        {
            if(DebugMode)
            {
                MyDictionary file = IterateToPath(path, true);
                file["@IsDirectory"] = false;
                file["@Contents"] = text;
                file["@Size"] = text.Length;
                file["@Type"] = "Text";
            }
            File.WriteAllText(path, text);
        }

        public static void FileWriteAllLines(string path, string[] lines)
        {
            if(DebugMode)
            {
                FileWriteAllText(path, string.Join(LineEnding, lines));
            }
            File.WriteAllLines(path, lines);
        }
    }
}