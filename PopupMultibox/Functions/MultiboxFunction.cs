using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PopupMultibox
{
    public interface MultiboxFunction
    {
        bool Triggers(MultiboxFunctionParam args);
        bool IsMulti(MultiboxFunctionParam args);
        bool IsBackgroundStream(MultiboxFunctionParam args);
        bool ShouldRun(MultiboxFunctionParam args);
        string RunSingle(MultiboxFunctionParam args);
        List<ResultItem> RunMulti(MultiboxFunctionParam args);
        void RunSingleBackgroundStream(MultiboxFunctionParam args);
        void RunMultiBackgroundStream(MultiboxFunctionParam args);
        bool HasDetails(MultiboxFunctionParam args);
        bool IsBackgroundDetailsStream(MultiboxFunctionParam args);
        string GetDetails(MultiboxFunctionParam args);
        void GetBackgroundDetailsStream(MultiboxFunctionParam args);
        bool HasActions(MultiboxFunctionParam args);
        bool IsBackgroundActionsStream(MultiboxFunctionParam args);
        List<ResultItem> GetActions(MultiboxFunctionParam args);
        void GetBackgroundActionsStream(MultiboxFunctionParam args);
        bool HasAction(MultiboxFunctionParam args);
        void RunAction(MultiboxFunctionParam args);
        bool SupressKeyPress(MultiboxFunctionParam args);
        bool HasKeyDownAction(MultiboxFunctionParam args);
        void RunKeyDownAction(MultiboxFunctionParam args);
        bool HasActionKeyEvent(MultiboxFunctionParam args);
        void RunActionKeyEvent(MultiboxFunctionParam args);
        bool HasSpecialDisplayCopyHandling(MultiboxFunctionParam args);
        string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args);
        bool HasSpecialInputCopyHandling(MultiboxFunctionParam args);
        string RunSpecialInputCopyHandling(MultiboxFunctionParam args);
    }

    public delegate void RunBgS(MultiboxFunctionParam args);

    public class MultiboxFunctionParam
    {
        private Keys key;
        private bool control;
        private bool alt;
        private bool shift;
        private MainClass mc;

        public string MultiboxText
        {
            get
            {
                return mc.InputFieldText;
            }
        }

        public string DisplayText
        {
            get
            {
                return mc.OutputLabelText;
            }
        }

        public Keys Key
        {
            get
            {
                return key;
            }
        }

        public bool Control
        {
            get
            {
                return control;
            }
        }

        public bool Alt
        {
            get
            {
                return alt;
            }
        }

        public bool Shift
        {
            get
            {
                return shift;
            }
        }

        public MainClass MC
        {
            get
            {
                return mc;
            }
        }

        public MultiboxFunctionParam(Keys key, bool control, bool alt, bool shift, MainClass mc)
        {
            this.key = key;
            this.control = control;
            this.alt = alt;
            this.shift = shift;
            this.mc = mc;
        }
    }
}