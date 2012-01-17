﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PopupMultibox
{
    public class HelpLaunchFuncion : AbstractFunction
    {
        #region MultiboxFunction Members

        public override bool Triggers(MultiboxFunctionParam args)
        {
            return (args.MultiboxText != null && args.MultiboxText.Length > 0 && args.MultiboxText[0] == '?');
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
                    return "Path: " + ((tmp2.FullText != null && tmp2.FullText.Length > 0) ? tmp2.FullText : "--") + "\nSection #: " + ((tmp2.EvalText != null && tmp2.EvalText.Length > 0) ? tmp2.EvalText : "--");
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