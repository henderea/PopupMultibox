using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.IO;

namespace PopupMultibox.Functions
{
    public class WebSearchFunction : AbstractFunction
    {
        public WebSearchFunction()
        {
            SearchList.Load();
        }

        #region IMultiboxFunction Members

        public override bool Triggers(MultiboxFunctionParam args)
        {
            return (!string.IsNullOrEmpty(args.MultiboxText) && args.MultiboxText[0] == '@');
        }

        public override string RunSingle(MultiboxFunctionParam args)
        {
            string rval = "Search engine not found";
            string t;
            string k = ParseSearchText(args, out t);
            foreach (SearchItem i in SearchList.Items)
            {
                if (!i.Keyword.Equals(k))
                    continue;
                rval = "Search " + i.Name + " for \"" + t + "\"";
                break;
            }
            return rval;
        }

        private static string ParseSearchText(MultiboxFunctionParam args, out string t)
        {
            int ind = args.MultiboxText.IndexOf(" ");
            string k;
            if (ind > 1)
            {
                k = args.MultiboxText.Substring(1, ind - 1);
                try
                {
                    t = args.MultiboxText.Substring(ind + 1);
                }
                catch
                {
                    t = "";
                }
            }
            else
            {
                k = args.MultiboxText.Substring(1);
                t = "";
            }
            return k;
        }

        public override bool HasActionKeyEvent(MultiboxFunctionParam args)
        {
            return true;
        }

        public override void RunActionKeyEvent(MultiboxFunctionParam args)
        {
            string t;
            string k = ParseSearchText(args, out t);
            t = HttpUtility.UrlEncode(t);
            foreach (SearchItem i in SearchList.Items)
            {
                if (!i.Keyword.Equals(k))
                    continue;
                Process.Start(i.SearchPath.Replace("%s", t));
                break;
            }
        }

        #endregion
    }

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