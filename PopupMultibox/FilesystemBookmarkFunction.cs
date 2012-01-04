using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PopupMultibox
{
    public class FilesystemBookmarkFunction : MultiboxFunction
    {
        #region MultiboxFunction Members

        public bool Triggers(MultiboxFunctionParam args)
        {
            return args.MultiboxText.StartsWith(">>");
        }

        public bool IsMulti(MultiboxFunctionParam args)
        {
            return true;
        }

        public bool IsBackgroundStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public bool ShouldRun(MultiboxFunctionParam args)
        {
            return !(args.Key == Keys.Up || args.Key == Keys.Down);
        }

        public string RunSingle(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public List<ResultItem> RunMulti(MultiboxFunctionParam args)
        {
            if (args.Key == Keys.Tab)
            {
                ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
                if (tmp2 != null)
                {
                    try
                    {
                        args.MC.InputFieldText = ":" + tmp2.FullText;
                        return new FilesystemFunction().RunMulti(args);
                    }
                    catch { }
                    return null;
                }
            }
            if (args.Key == Keys.Delete && args.Shift)
            {
                ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
                if (tmp2 != null)
                        BookmarkList.Delete(tmp2.DisplayText);
            }
            BookmarkItem[] itms = BookmarkList.Find(args.MultiboxText.Substring(2));
            if (itms == null || itms.Length <= 0)
                return null;
            List<ResultItem> ritms = new List<ResultItem>(0);
            foreach (BookmarkItem itm in itms)
            {
                try
                {
                    ritms.Add(new ResultItem(itm.Name, itm.Path, itm.Path));
                }
                catch { }
            }
            if (ritms == null || ritms.Count <= 0)
                return null;
            return ritms;
        }

        public void RunSingleBackgroundStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public void RunMultiBackgroundStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasDetails(MultiboxFunctionParam args)
        {
            return true;
        }

        public bool IsBackgroundDetailsStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public string GetDetails(MultiboxFunctionParam args)
        {
            try
            {
                ResultItem tmp2 = args.MC.LabelManager.CurrentSelection;
                if (tmp2 != null)
                    return "Name: " + tmp2.DisplayText + "\nPath: " + tmp2.FullText;
            }
            catch { }
            return "";
        }

        public void GetBackgroundDetailsStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasActions(MultiboxFunctionParam args)
        {
            return false;
        }

        public bool IsBackgroundActionsStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public List<ResultItem> GetActions(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public void GetBackgroundActionsStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasAction(MultiboxFunctionParam args)
        {
            return false;
        }

        public void RunAction(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool SupressKeyPress(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Up || args.Key == Keys.Down || args.Key == Keys.Tab || (args.Key == Keys.Delete && args.Shift));
        }

        public bool HasKeyDownAction(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Up || args.Key == Keys.Down);
        }

        public void RunKeyDownAction(MultiboxFunctionParam args)
        {
            if (args.Key == Keys.Up)
                args.MC.LabelManager.SelectPrev();
            else if (args.Key == Keys.Down)
                args.MC.LabelManager.SelectNext();
        }

        public bool HasActionKeyEvent(MultiboxFunctionParam args)
        {
            return false;
        }

        public void RunActionKeyEvent(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return true;
        }

        public string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
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

        public bool HasSpecialInputCopyHandling(MultiboxFunctionParam args)
        {
            return false;
        }

        public string RunSpecialInputCopyHandling(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }

    public class BookmarkItem : IComparable<BookmarkItem>
    {
        public string Name
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public BookmarkItem() : this("", "") { }

        public BookmarkItem(string n, string p)
        {
            this.Name = n;
            this.Path = p;
        }

        public string ToFileString()
        {
            return this.Name + ";;;" + this.Path;
        }

        public static BookmarkItem FromFileString(string data)
        {
            try
            {
                string[] parts = data.Split(new string[] { ";;;" }, StringSplitOptions.None);
                return new BookmarkItem(parts[0], parts[1]);
            }
            catch { }
            return null;
        }

        #region IComparable<BookmarkItem> Members

        public int CompareTo(BookmarkItem other)
        {
            return this.Name.CompareTo(other.Name);
        }

        #endregion
    }

    public class BookmarkList
    {
        private static List<BookmarkItem> items;

        public static BookmarkItem[] Items
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

        static BookmarkList()
        {
            items = new List<BookmarkItem>(0);
        }

        public static BookmarkItem Get(int i)
        {
            try
            {
                return items[i];
            }
            catch { }
            return null;
        }

        public static void Set(int i, BookmarkItem itm)
        {
            try
            {
                items[i] = itm;
                Store();
            }
            catch { }
        }

        public static void Add(BookmarkItem i)
        {
            try
            {
                items.Add(i);
                Store();
            }
            catch { }
        }

        public static void Remove(BookmarkItem i)
        {
            try
            {
                items.Remove(i);
                Store();
            }
            catch { }
        }

        public static void RemoveAt(int i)
        {
            try
            {
                items.RemoveAt(i);
                Store();
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

        public static BookmarkItem[] Find(string fnd)
        {
            if (items == null || items.Count <= 0)
                return null;
            try
            {
                List<BookmarkItem> tmp = new List<BookmarkItem>(0);
                string fnd2 = fnd.ToLower();
                foreach (BookmarkItem itm in items)
                {
                    if (itm.Name.ToLower().StartsWith(fnd2))
                        tmp.Add(itm);
                }
                try
                {
                    tmp.Sort();
                }
                catch { }
                return tmp.ToArray();
            }
            catch { }
            return null;
        }

        public static void Delete(string name)
        {
            if (items == null || items.Count <= 0)
                return;
            try
            {
                string name2 = name.ToLower();
                foreach (BookmarkItem itm in items)
                {
                    if (itm.Name.ToLower().Equals(name2))
                    {
                        items.Remove(itm);
                        break;
                    }
                }
                Store();
            }
            catch { }
        }

        public static void Store()
        {
            try
            {
                try
                {
                    items.Sort();
                }
                catch { }
                List<string> lines = new List<string>(0);
                foreach (BookmarkItem i in items)
                {
                    string tmp = i.ToFileString();
                    if (!tmp.Equals(";;;"))
                        lines.Add(tmp);
                }
                if (!Directory.Exists(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox"))
                    Directory.CreateDirectory(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox");
                File.WriteAllLines(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox\\bookmarks.txt", lines.ToArray());
            }
            catch { }
        }

        public static void Load()
        {
            try
            {
                string[] lines = File.ReadAllLines(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox\\bookmarks.txt");
                items.Clear();
                foreach (string line in lines)
                {
                    BookmarkItem tmp = BookmarkItem.FromFileString(line);
                    if (tmp != null)
                        items.Add(tmp);
                }
                items.Sort();
            }
            catch { }
        }
    }
}