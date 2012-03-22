using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using PopupMultibox.UI;

namespace PopupMultibox.Functions
{
    public class FilesystemBookmarkFunction : AbstractFunction
    {
        #region IMultiboxFunction Members

        public override bool Triggers(MultiboxFunctionParam args)
        {
            return args.MultiboxText.StartsWith(">>");
        }

        public override bool IsMulti(MultiboxFunctionParam args)
        {
            return true;
        }

        public override bool ShouldRun(MultiboxFunctionParam args)
        {
            return !(args.Key == Keys.Up || args.Key == Keys.Down);
        }

        public override List<ResultItem> RunMulti(MultiboxFunctionParam args)
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
            return ritms.Count <= 0 ? null : ritms;
        }

        public override bool HasDetails(MultiboxFunctionParam args)
        {
            return true;
        }

        public override string GetDetails(MultiboxFunctionParam args)
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

        public override bool SupressKeyPress(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Up || args.Key == Keys.Down || args.Key == Keys.Tab || (args.Key == Keys.Delete && args.Shift));
        }

        public override bool HasKeyDownAction(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Up || args.Key == Keys.Down);
        }

        public override void RunKeyDownAction(MultiboxFunctionParam args)
        {
            if (args.Key == Keys.Up)
                args.MC.LabelManager.SelectPrev();
            else if (args.Key == Keys.Down)
                args.MC.LabelManager.SelectNext();
        }

        public override bool HasSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return true;
        }

        public override string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
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
            Name = n;
            Path = p;
        }

        public string ToFileString()
        {
            return Name + ";;;" + Path;
        }

        public static BookmarkItem FromFileString(string data)
        {
            try
            {
                string[] parts = data.Split(new[] { ";;;" }, StringSplitOptions.None);
                return new BookmarkItem(parts[0], parts[1]);
            }
            catch { }
            return null;
        }

        #region IComparable<BookmarkItem> Members

        public int CompareTo(BookmarkItem other)
        {
            return Name.CompareTo(other.Name);
        }

        #endregion
    }

    public class BookmarkList
    {
        private static readonly List<BookmarkItem> items;

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
                tmp.AddRange(items.Where(itm => itm.Name.ToLower().StartsWith(fnd2)));
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
                lines.AddRange(items.Select(i => i.ToFileString()).Where(tmp => !tmp.Equals(";;;")));
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