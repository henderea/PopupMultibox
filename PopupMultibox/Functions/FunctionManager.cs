using System.Collections.Generic;
using System.Windows.Forms;
using PopupMultibox.UI;

namespace PopupMultibox.Functions
{
    public class FunctionManager
    {
        private static readonly List<IMultiboxFunction> functions;

        static FunctionManager()
        {
            functions = new List<IMultiboxFunction>(0);
            functions.Add(new ScreensaverFunction());
            functions.Add(new UpdateFunction());
            functions.Add(new WebSearchFunction());
            functions.Add(new FilesystemFunction());
            functions.Add(new HelpLaunchFuncion());
            functions.Add(new FilesystemBookmarkFunction());
            functions.Add(new AppLaunchFunction());
            functions.Add(new CalculatorFunction());
        }

        private static IMultiboxFunction GetActivatedFunction(MultiboxFunctionParam args)
        {
            try
            {
                foreach (IMultiboxFunction f in functions)
                {
                    if (f.Triggers(args))
                        return f;
                }
            }
            catch {}
            return null;
        }

        public static void KeyDown(MainClass mc, KeyEventArgs e)
        {
            try
            {
                MultiboxFunctionParam p = new MultiboxFunctionParam(e.KeyCode, e.Control, e.Alt, e.Shift, mc);
                IMultiboxFunction af = GetActivatedFunction(p);
                if (af == null)
                    return;
                RunKeyDownActionIfNeeded(p, af);
                SupressKeyPressIfNeeded(e, p, af);
            }
            catch {}
        }

        private static void RunKeyDownActionIfNeeded(MultiboxFunctionParam p, IMultiboxFunction af)
        {
            if (af.HasKeyDownAction(p))
                af.RunKeyDownAction(p);
        }

        public static bool KeyUp(MainClass mc, KeyEventArgs e)
        {
            try
            {
                MultiboxFunctionParam p = new MultiboxFunctionParam(e.KeyCode, e.Control, e.Alt, e.Shift, mc);
                IMultiboxFunction af = GetActivatedFunction(p);
                if (!NoActivatedFunction(mc, af))
                    return false;
                if (PlainEnter(p))
                    RunActionKeyEventIfNeeded(p, af);
                else if (ControlEnter(p))
                    CopyDisplayText(af, p);
                else if (ShiftEnter(p))
                    CopyMultiboxText(af, p);
                else if (Run(e, p, af))
                    return true;
                SupressKeyPressIfNeeded(e, p, af);
            }
            catch {}
            return false;
        }

        private static void CopyMultiboxText(IMultiboxFunction af, MultiboxFunctionParam p)
        {
            string tc = p.MultiboxText;
            if (af.HasSpecialInputCopyHandling(p))
                tc = af.RunSpecialInputCopyHandling(p);
            if (!string.IsNullOrEmpty(tc))
                Clipboard.SetText(tc);
        }

        private static void CopyDisplayText(IMultiboxFunction af, MultiboxFunctionParam p)
        {
            string tc = p.DisplayText;
            if (af.HasSpecialDisplayCopyHandling(p))
                tc = af.RunSpecialDisplayCopyHandling(p);
            if (!string.IsNullOrEmpty(tc))
                Clipboard.SetText(tc);
        }

        private static void RunActionKeyEventIfNeeded(MultiboxFunctionParam p, IMultiboxFunction af)
        {
            if (af.HasActionKeyEvent(p))
                af.RunActionKeyEvent(p);
        }

        private static bool NoActivatedFunction(MainClass mc, IMultiboxFunction af)
        {
            return af != null || ClearMultibox(mc);
        }

        private static bool ClearMultibox(MainClass mc)
        {
            mc.OutputLabelText = "";
            mc.LabelManager.ResultItems = null;
            return false;
        }

        private static bool Run(KeyEventArgs e, MultiboxFunctionParam p, IMultiboxFunction af)
        {
            bool sr = af.ShouldRun(p);
            bool ibs = af.IsBackgroundStream(p);
            if (RunIfMulti(e, p, af, ibs, sr))
                return true;
            RunSingleIfNeeded(af, p, sr, ibs);
            return false;
        }

        private static bool RunIfMulti(KeyEventArgs e, MultiboxFunctionParam p, IMultiboxFunction af, bool ibs, bool sr)
        {
            if (af.IsMulti(p))
            {
                RunMultiIfNeeded(af, p, sr, ibs);
                SupressKeyPressIfNeeded(e, p, af);
                return true;
            }
            return false;
        }

        private static void RunSingleIfNeeded(IMultiboxFunction af, MultiboxFunctionParam p, bool sr, bool ibs)
        {
            if (!sr)
                return;
            if (ibs)
                new RunBgS(af.RunSingleBackgroundStream).BeginInvoke(p, null, null);
            else
                p.MC.OutputLabelText = af.RunSingle(p);
        }

        private static void RunMultiIfNeeded(IMultiboxFunction af, MultiboxFunctionParam p, bool sr, bool ibs)
        {
            if (!sr)
                return;
            if (ibs)
                new RunBgS(af.RunMultiBackgroundStream).BeginInvoke(p, null, null);
            else
                p.MC.LabelManager.ResultItems = af.RunMulti(p);
        }

        private static void SupressKeyPressIfNeeded(KeyEventArgs e, MultiboxFunctionParam p, IMultiboxFunction af)
        {
            if (af.SupressKeyPress(p))
                e.SuppressKeyPress = true;
        }

        private static bool ShiftEnter(MultiboxFunctionParam p)
        {
            return p.Key == Keys.Enter && p.Shift && !p.Control && !p.Alt && p.MultiboxText.Trim().Length > 0;
        }

        private static bool ControlEnter(MultiboxFunctionParam p)
        {
            return p.Key == Keys.Enter && p.Control && !p.Shift && !p.Alt && p.DisplayText.Trim().Length > 0;
        }

        private static bool PlainEnter(MultiboxFunctionParam p)
        {
            return p.Key == Keys.Enter && !p.Control && !p.Shift && !p.Alt && p.MultiboxText.Trim().Length > 0;
        }

        public static void SelectionChanged(MainClass mc)
        {
            try
            {
                MultiboxFunctionParam p = new MultiboxFunctionParam(Keys.None, false, false, false, mc);
                IMultiboxFunction af = GetActivatedFunction(p);
                if ((!af.IsMulti(p) || !af.HasDetails(p)) && !ClearMultibox(mc))
                    return;
                RunDetails(p, af);
            }
            catch {}
        }

        private static void RunDetails(MultiboxFunctionParam p, IMultiboxFunction af)
        {
            bool ibs = af.IsBackgroundDetailsStream(p);
            if (ibs)
                RunBackgroundDetails(p, af);
            else
                RunForegroundDetails(af, p);
        }

        private static void RunForegroundDetails(IMultiboxFunction af, MultiboxFunctionParam p)
        {
            p.MC.DetailsLabelText = af.GetDetails(p);
            p.MC.UpdateSize();
        }

        private static void RunBackgroundDetails(MultiboxFunctionParam p, IMultiboxFunction af)
        {
            new RunBgS(af.GetBackgroundDetailsStream).BeginInvoke(p, null, null);
        }
    }
}