using System.Collections.Generic;
using System.Windows.Forms;
using PopupMultibox.UI;

namespace PopupMultibox.Functions
{
    public class HelpLaunchFuncion : AbstractFunction
    {
        #region IMultiboxFunction Members

        public override bool Triggers(MultiboxFunctionParam args)
        {
            return (!string.IsNullOrEmpty(args.MultiboxText) && args.MultiboxText[0] == '?');
        }

        public override bool IsMulti(MultiboxFunctionParam args)
        {
            return true;
        }

        public override bool ShouldRun(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Tab || args.Key == Keys.Back || args.MC.LabelManager.ResultItems == null || args.MC.LabelManager.ResultItems.Count <= 0);
        }

        public override List<ResultItem> RunMulti(MultiboxFunctionParam args)
        {
            if (args.MultiboxText.Trim().Length == 1 && !(args.Key == Keys.Up || args.Key == Keys.Down || args.Key == Keys.Tab || args.Key == Keys.Enter || args.Key == Keys.Back))
                return args.MC.HelpDialog.GetAutocompleteOptions("");
            if (args.Key == Keys.Tab)
            {
                ResultItem tmp = args.MC.LabelManager.CurrentSelection;
                if (tmp != null)
                {
                    args.MC.InputFieldText = "?" + tmp.FullText;
                    return args.MC.HelpDialog.GetAutocompleteOptions(tmp.FullText);
                }
            }
            else if (args.Key == Keys.Back)
            {
                if (args.MultiboxText.Length <= 1)
                {
                    args.MC.InputFieldText = "";
                    return null;
                }
                int ind = args.MultiboxText.LastIndexOf(">", args.MultiboxText.Length - 2);
                if (ind > 1)
                {
                    args.MC.InputFieldText = args.MultiboxText.Remove(ind + 1);
                    return args.MC.HelpDialog.GetAutocompleteOptions(args.MultiboxText.Substring(1));
                }
                args.MC.InputFieldText = "?";
                return args.MC.HelpDialog.GetAutocompleteOptions("");
            }
            return null;
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
                    return "Path: " + (!string.IsNullOrEmpty(tmp2.FullText) ? tmp2.FullText : "--") + "\nSection #: " + (!string.IsNullOrEmpty(tmp2.EvalText) ? tmp2.EvalText : "--");
            }
            catch { }
            return "";
        }

        public override bool SupressKeyPress(MultiboxFunctionParam args)
        {
            return true;
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

        public override bool HasActionKeyEvent(MultiboxFunctionParam args)
        {
            return true;
        }

        public override void RunActionKeyEvent(MultiboxFunctionParam args)
        {
            ResultItem tmp = args.MC.LabelManager.CurrentSelection;
            if (tmp != null)
                args.MC.HelpDialog.LaunchPage(tmp.EvalText);
        }

        public override bool HasSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return true;
        }

        public override string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            ResultItem tmp = args.MC.LabelManager.CurrentSelection;
            if (tmp != null)
                return tmp.FullText;
            return null;
        }

        #endregion
    }
}