using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Multibox.Core.helpers
{
    public class SearchItem
    {
        public string Name
        {
            get;
            set;
        }

        public string Keyword
        {
            get;
            set;
        }

        public string SearchPath
        {
            get;
            set;
        }

        public SearchItem()
        {
            Name = "";
            Keyword = "";
            SearchPath = "";
        }

        public SearchItem(string n, string k, string s)
        {
            Name = n;
            Keyword = k;
            SearchPath = s;
        }

        public string ToFileString()
        {
            return Name + ";;;" + Keyword + ";;;" + SearchPath;
        }

        public static SearchItem FromFileString(string data)
        {
            try
            {
                string[] parts = data.Split(new[] { ";;;" }, StringSplitOptions.None);
                return new SearchItem(parts[0], parts[1], parts[2]);
            }
            catch { }
            return null;
        }
    }

    public class SearchList
    {
        private static readonly List<SearchItem> items;

        public static SearchItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }

        public static int Count
        {
            get
            {
                return items.Count;
            }
        }

        static SearchList()
        {
            items = new List<SearchItem>(0);
        }

        public static SearchItem Get(int i)
        {
            try
            {
                return items[i];
            }
            catch { }
            return null;
        }

        public static void Set(int i, SearchItem itm)
        {
            try
            {
                items[i] = itm;
            }
            catch { }
        }

        public static void Add(SearchItem i)
        {
            try
            {
                items.Add(i);
            }
            catch { }
        }

        public static void Remove(SearchItem i)
        {
            try
            {
                items.Remove(i);
            }
            catch { }
        }

        public static void RemoveAt(int i)
        {
            try
            {
                items.RemoveAt(i);
            }
            catch { }
        }

        public static void Clear()
        {
            try
            {
                items.Clear();
            }
            catch { }
        }

        public static void Store()
        {
            try
            {
                List<string> lines = new List<string>(0);
                lines.AddRange(items.Select(i => i.ToFileString()).Where(tmp => !tmp.Equals(";;;;;;")));
                if (!Directory.Exists(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox"))
                    Directory.CreateDirectory(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox");
                File.WriteAllLines(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox\\searches.txt", lines.ToArray());
            }
            catch { }
        }

        public static void Load()
        {
            try
            {
                string[] lines = File.ReadAllLines(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox\\searches.txt");
                items.Clear();
                foreach (string line in lines)
                {
                    SearchItem tmp = SearchItem.FromFileString(line);
                    if (tmp != null)
                        items.Add(tmp);
                }
            }
            catch
            {
                try
                {
                    List<string> lines = new List<string>(0);
                    lines.Add("Google;;;google;;;http://www.google.com/search?q=%s");
                    lines.Add("Yahoo!;;;yahoo;;;http://search.yahoo.com/search?fr=crmas&p=%s");
                    lines.Add("Bing;;;bing;;;http://www.bing.com/search?q=%s");
                    lines.Add("Wikipedia;;;wiki;;;http://en.wikipedia.org/w/index.php?title=Special:Search&search=%s");
                    if (!Directory.Exists(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox"))
                        Directory.CreateDirectory(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox");
                    // write the log file output lines to the file
                    File.WriteAllLines(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox\\searches.txt", lines.ToArray());
                    items.Clear();
                    foreach (string line in lines)
                    {
                        SearchItem tmp = SearchItem.FromFileString(line);
                        if (tmp != null)
                            items.Add(tmp);
                    }
                }
                catch { }
            }
        }
    }
}
