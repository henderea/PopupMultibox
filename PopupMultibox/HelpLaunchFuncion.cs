using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PopupMultibox
{
    public class HelpLaunchFuncion : MultiboxFunction
    {
        #region MultiboxFunction Members

        public bool Triggers(MultiboxFunctionParam args)
        {
            return (args.MultiboxText != null && args.MultiboxText.Length > 0 && args.MultiboxText[0] == '?');
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
            if (args.MultiboxText.Trim().Length == 1 && !(args.Key == Keys.Up || args.Key == Keys.Down || args.Key == Keys.Tab || args.Key == Keys.Enter || args.Key == Keys.Back))
                return args.MC.HelpDialog.GetAutocompleteOptions("");
            else if (args.Key == Keys.Tab)
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
                else
                {
                    int ind = args.MultiboxText.LastIndexOf(">", args.MultiboxText.Length - 2);
                    if (ind > 1)
                    {
                        args.MC.InputFieldText = args.MultiboxText.Remove(ind + 1);
                        return args.MC.HelpDialog.GetAutocompleteOptions(args.MultiboxText.Substring(1));
                    }
                    else
                    {
                        args.MC.InputFieldText = "?";
                        return args.MC.HelpDialog.GetAutocompleteOptions("");
                    }
                }
            }
            return null;
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
                    return "Path: " + ((tmp2.FullText != null && tmp2.FullText.Length > 0) ? tmp2.FullText : "--") + "\nSection #: " + ((tmp2.EvalText != null && tmp2.EvalText.Length > 0) ? tmp2.EvalText : "--");
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
            return true;
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
            return true;
        }

        public void RunActionKeyEvent(MultiboxFunctionParam args)
        {
            ResultItem tmp = args.MC.LabelManager.CurrentSelection;
            if (tmp != null)
                args.MC.HelpDialog.LaunchPage(tmp.EvalText);
        }

        public bool HasSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return true;
        }

        public string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            ResultItem tmp = args.MC.LabelManager.CurrentSelection;
            if (tmp != null)
                return tmp.FullText;
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
}
