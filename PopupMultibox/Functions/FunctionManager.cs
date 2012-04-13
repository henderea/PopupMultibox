using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Multibox.Core.UI;

namespace Multibox.Core.Functions
{
    public class FunctionManager
    {
        private static List<IMultiboxFunction> functions;

        public static void Setup()
        {
            functions = new List<IMultiboxFunction>(0);
            LoadPlugins();
            IEnumerable<Type> pluginClasses = TypesExtendingClass(typeof (IMultiboxFunction));
            foreach (Type t in pluginClasses)
            {
                if (IsRealClass(t))
                    functions.Add((IMultiboxFunction) Activator.CreateInstance(t));
            }
            functions = new List<IMultiboxFunction>(functions.OrderByDescending(f => f.SuggestedIndex()));
            /*functions.Add(new ScreensaverFunction());
            functions.Add(new UpdateFunction());
            functions.Add(new WebSearchFunction());
            functions.Add(new FilesystemFunction());
            functions.Add(new HelpLaunchFuncion());
            functions.Add(new FilesystemBookmarkFunction());
            functions.Add(new AppLaunchFunction());
            functions.Add(new CalculatorFunction());*/
        }

        private static void LoadPlugins()
        {
            foreach (string f in Directory.EnumerateFiles(Application.StartupPath + "\\plugins\\"))
            {
                if (!f.EndsWith(".dll")) continue;
                Assembly.LoadFrom(f);
            }
        }

        public static IEnumerable<Type> TypesExtendingClass(Type desiredType)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(desiredType.IsAssignableFrom);
        }

        public static bool IsRealClass(Type testType)
        {
            return testType.IsAbstract == false && testType.IsGenericTypeDefinition == false && testType.IsInterface == false;
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
                if (af.HasKeyDownAction(p))
                {
                    af.RunKeyDownAction(p);
                }
                if (af.SupressKeyPress(p))
                    e.SuppressKeyPress = true;
            }
            catch {}
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
            catch {}
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
            catch {}
        }
    }
}