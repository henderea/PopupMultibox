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
            catch { }
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
                if (af.HasKeyDownAction(p))
                {
                    af.RunKeyDownAction(p);
                }
                if (af.SupressKeyPress(p))
                    e.SuppressKeyPress = true;
            }
            catch { }
        }

        public static bool KeyUp(MainClass mc, KeyEventArgs e)
        {
            try
            {
                MultiboxFunctionParam p = new MultiboxFunctionParam(e.KeyCode, e.Control, e.Alt, e.Shift, mc);
                IMultiboxFunction af = GetActivatedFunction(p);
                if (af == null)
                {
                    mc.OutputLabelText = "";
                    mc.LabelManager.ResultItems = null;
                    return false;
                }
                if (p.Key == Keys.Enter && !p.Control && !p.Shift && !p.Alt && p.MultiboxText.Trim().Length > 0)
                {
                    if (af.HasActionKeyEvent(p))
                        af.RunActionKeyEvent(p);
                }
                else if (p.Key == Keys.Enter && p.Control && !p.Shift && !p.Alt && p.DisplayText.Trim().Length > 0)
                {
                    string tc = p.DisplayText;
                    if (af.HasSpecialDisplayCopyHandling(p))
                        tc = af.RunSpecialDisplayCopyHandling(p);
                    if (!string.IsNullOrEmpty(tc))
                        Clipboard.SetText(tc);
                }
                else if (p.Key == Keys.Enter && p.Shift && !p.Control && !p.Alt && p.MultiboxText.Trim().Length > 0)
                {
                    string tc = p.MultiboxText;
                    if (af.HasSpecialInputCopyHandling(p))
                        tc = af.RunSpecialInputCopyHandling(p);
                    if (!string.IsNullOrEmpty(tc))
                        Clipboard.SetText(tc);
                }
                else
                {
                    bool sr = af.ShouldRun(p);
                    bool ibs = af.IsBackgroundStream(p);
                    if (af.IsMulti(p))
                    {
                        if (sr)
                        {
                            if (ibs)
                                new RunBgS(af.RunMultiBackgroundStream).BeginInvoke(p, null, null);
                            else
                                p.MC.LabelManager.ResultItems = af.RunMulti(p);
                        }
                        if (af.SupressKeyPress(p))
                            e.SuppressKeyPress = true;
                        return true;
                    }
                    if (sr)
                    {
                        if (ibs)
                            new RunBgS(af.RunSingleBackgroundStream).BeginInvoke(p, null, null);
                        else
                            p.MC.OutputLabelText = af.RunSingle(p);
                    }
                }
                if (af.SupressKeyPress(p))
                    e.SuppressKeyPress = true;
            }
            catch { }
            return false;
        }

        public static void SelectionChanged(MainClass mc)
        {
            try
            {
                MultiboxFunctionParam p = new MultiboxFunctionParam(Keys.None, false, false, false, mc);
                IMultiboxFunction af = GetActivatedFunction(p);
                if (!af.IsMulti(p) || !af.HasDetails(p))
                {
                    p.MC.DetailsLabelText = "";
                    p.MC.UpdateSize();
                    return;
                }
                bool ibs = af.IsBackgroundDetailsStream(p);
                if (ibs)
                    new RunBgS(af.GetBackgroundDetailsStream).BeginInvoke(p, null, null);
                else
                {
                    p.MC.DetailsLabelText = af.GetDetails(p);
                    p.MC.UpdateSize();
                }
            }
            catch { }
        }
    }
}