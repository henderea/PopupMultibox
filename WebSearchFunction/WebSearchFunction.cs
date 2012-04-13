using System.Diagnostics;
using System.Web;
using Multibox.Core.Functions;
using Multibox.Core.helpers;

namespace Multibox.Plugin.WebSearchFunction
{
    public class WebSearchFunction : AbstractFunction
    {
        public override int SuggestedIndex()
        {
            return 1;
        }

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
            int ind = args.MultiboxText.IndexOf(" ");
            string k;
            string t;
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
            foreach (SearchItem i in SearchList.Items)
            {
                if (!i.Keyword.Equals(k)) continue;
                rval = "Search " + i.Name + " for \"" + t + "\"";
                break;
            }
            return rval;
        }

        public override bool HasActionKeyEvent(MultiboxFunctionParam args)
        {
            return true;
        }

        public override void RunActionKeyEvent(MultiboxFunctionParam args)
        {
            int ind = args.MultiboxText.IndexOf(" ");
            string k;
            string t;
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
            t = HttpUtility.UrlEncode(t);
            foreach (SearchItem i in SearchList.Items)
            {
                if (!i.Keyword.Equals(k)) continue;
                Process.Start(i.SearchPath.Replace("%s", t));
                break;
            }
        }

        #endregion
    }
}