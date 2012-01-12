using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PopupMultibox
{
    class UpdateFunction : MultiboxFunction
    {
        #region MultiboxFunction Members

        public bool Triggers(MultiboxFunctionParam args)
        {
            return args.MultiboxText.Equals("update");
        }

        public bool IsMulti(MultiboxFunctionParam args)
        {
            return false;
        }

        public bool IsBackgroundStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public bool ShouldRun(MultiboxFunctionParam args)
        {
            return true;
        }

        public string RunSingle(MultiboxFunctionParam args)
        {
            if (args.Key == Keys.Tab)
            {
                args.MC.VChk.checkForUpdateForce();
                return "No update found";
            }
            return "Check for updates";
        }

        public List<ResultItem> RunMulti(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
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
            return false;
        }

        public bool IsBackgroundDetailsStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public string GetDetails(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
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
            return (args.Key == Keys.Tab);
        }

        public bool HasKeyDownAction(MultiboxFunctionParam args)
        {
            return false;
        }

        public void RunKeyDownAction(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
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
            return false;
        }

        public string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
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
