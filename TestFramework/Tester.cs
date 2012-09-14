using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Multibox.Core.Functions;
using Multibox.Core.UI;
using Multibox.Plugin.Util;
using NUnit.Framework;

namespace Multibox.Test.TestFramework
{
    public class Tester
    {
        private readonly IMainClass mainClass;
        private readonly MockFunctionManager functionManager;
        private bool lastWasMulti;
        private readonly List<string[]> history;

        public Tester(IMultiboxFunction[] functions, int maxResults, bool mockFilesystem = true)
        {
            functionManager = new MockFunctionManager(functions);
            mainClass = new MockMainClass(maxResults);
            mainClass.LabelManager.Sc = SelectionChanged;
            Filesystem.DebugMode = mockFilesystem;
            history = new List<string[]>(0);
        }

        public Tester ResetHistory()
        {
            history.Clear();
            return this;
        }

        public string PrintHistory()
        {
            string str = "";
            for (int i = 0; i < history.Count; i++)
            {
                str += (i+1) + ": " + string.Join(" ; ", history[i]) + "\n";
            }
            return str.Trim();
        }

        private void SelectionChanged(int i)
        {
            functionManager.SelectionChanged(mainClass);
        }

        private static string CreateKeyString(Keys key, bool control, bool alt, bool shift)
        {
            List<string> parts = new List<string>(0);
            if (control)
                parts.Add("ctrl");
            if (alt)
                parts.Add("alt");
            if (shift)
                parts.Add("shift");
            parts.Add(key + "");
            return string.Join("+", parts);
        }

        public Tester SetText(string text, Keys eventKey, bool eventControl, bool eventAlt, bool eventShift)
        {
            mainClass.InputFieldText = text;
            functionManager.KeyDown(mainClass, eventKey, eventControl, eventAlt, eventShift);
            lastWasMulti = functionManager.KeyUp(mainClass, eventKey, eventControl, eventAlt, eventShift);
            history.Add(new[]
                        {
                            "Text = \"" + text + "\"",
                            "KeyDown: " + CreateKeyString(eventKey, eventControl, eventAlt, eventShift),
                            "KeyUp: " + CreateKeyString(eventKey, eventControl, eventAlt, eventShift)
                        });
            return this;
        }

        public Tester KeyDown(Keys eventKey, bool eventControl, bool eventAlt, bool eventShift)
        {
            functionManager.KeyDown(mainClass, eventKey, eventControl, eventAlt, eventShift);
            history.Add(new[] { "KeyDown: " + CreateKeyString(eventKey, eventControl, eventAlt, eventShift) });
            return this;
        }

        public Tester KeyUp(Keys eventKey, bool eventControl, bool eventAlt, bool eventShift)
        {
            lastWasMulti = functionManager.KeyUp(mainClass, eventKey, eventControl, eventAlt, eventShift);
            history.Add(new[] { "KeyUp: " + CreateKeyString(eventKey, eventControl, eventAlt, eventShift) });
            return this;
        }

        public Tester KeyPress(Keys eventKey, bool eventControl, bool eventAlt, bool eventShift)
        {
            functionManager.KeyDown(mainClass, eventKey, eventControl, eventAlt, eventShift);
            lastWasMulti = functionManager.KeyUp(mainClass, eventKey, eventControl, eventAlt, eventShift);
            history.Add(new[]
                        {
                            "KeyDown: " + CreateKeyString(eventKey, eventControl, eventAlt, eventShift),
                            "KeyUp: " + CreateKeyString(eventKey, eventControl, eventAlt, eventShift)
                        });
            return this;
        }

        public Tester CheckInputText(string text, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                Assert.AreEqual(text, mainClass.InputFieldText);
            else
                Assert.AreEqual(text, mainClass.InputFieldText, message);
            return this;
        }

        public Tester CheckOutputLabelText(string text, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                Assert.AreEqual(text, mainClass.OutputLabelText);
            else
                Assert.AreEqual(text, mainClass.OutputLabelText, message);
            return this;
        }

        public Tester CheckCurrentListItemDisplayText(string text, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                Assert.AreEqual(text, mainClass.LabelManager.CurrentSelection.DisplayText);
            else
                Assert.AreEqual(text, mainClass.LabelManager.CurrentSelection.DisplayText, message);
            return this;
        }

        public Tester CheckCurrentListItemFullText(string text, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                Assert.AreEqual(text, mainClass.LabelManager.CurrentSelection.FullText);
            else
                Assert.AreEqual(text, mainClass.LabelManager.CurrentSelection.FullText, message);
            return this;
        }

        public Tester CheckCurrentListItemEvalText(string text, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                Assert.AreEqual(text, mainClass.LabelManager.CurrentSelection.EvalText);
            else
                Assert.AreEqual(text, mainClass.LabelManager.CurrentSelection.EvalText, message);
            return this;
        }

        public Tester CheckCurrentListItemIndex(int index, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                Assert.AreEqual(index, mainClass.LabelManager.CurrentSelectionIndex);
            else
                Assert.AreEqual(index, mainClass.LabelManager.CurrentSelectionIndex, message);
            return this;
        }

        public Tester CheckCurrentListItems(List<ResultItem> items, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                Assert.AreEqual(items, mainClass.LabelManager.ResultItems);
            else
                Assert.AreEqual(items, mainClass.LabelManager.ResultItems, message);
            return this;
        }

        public Tester CheckCurrentVisibleListItems(List<ResultItem> items, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                Assert.AreEqual(items,
                                mainClass.LabelManager.ResultItems.GetRange(mainClass.LabelManager.ResultIndex,
                                                                            mainClass.LabelManager.DisplayCount));
            else
                Assert.AreEqual(items,
                                mainClass.LabelManager.ResultItems.GetRange(mainClass.LabelManager.ResultIndex,
                                                                            mainClass.LabelManager.DisplayCount),
                                message);
            return this;
        }

        public Tester CheckClipboard(string text, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                Assert.AreEqual(text, functionManager.Clipboard);
            else
                Assert.AreEqual(text, functionManager.Clipboard, message);
            return this;
        }

        public Tester CheckIsMulti(bool isMulti, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                Assert.AreEqual(isMulti, lastWasMulti);
            else
                Assert.AreEqual(isMulti, lastWasMulti, message);
            return this;
        }

        public Tester CheckSuppressKeyPress(bool suppress, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                Assert.AreEqual(suppress, functionManager.SuppressKeyPress);
            else
                Assert.AreEqual(suppress, functionManager.SuppressKeyPress, message);
            return this;
        }
    }

    internal class MockFunctionManager
    {
        private readonly List<IMultiboxFunction> functions;
        public string Clipboard { get; set; }
        public bool SuppressKeyPress { get; set; }

        public MockFunctionManager(IMultiboxFunction[] functions)
        {
            this.functions = new List<IMultiboxFunction>(functions.OrderByDescending(f => f.SuggestedIndex()));
        }

        private IMultiboxFunction GetActivatedFunction(MultiboxFunctionParam args)
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

        public void KeyDown(IMainClass mc, Keys key, bool control, bool alt, bool shift)
        {
            try
            {
                MultiboxFunctionParam p = new MultiboxFunctionParam(key, control, alt, shift, mc);
                IMultiboxFunction function = GetActivatedFunction(p);
                if (function == null)
                    return;
                if (function.IsMulti(p) && (p.Key == Keys.Up || p.Key == Keys.Down))
                {
                    switch (p.Key)
                    {
                        case Keys.Up:
                            p.MC.LabelManager.SelectPrev();
                            break;
                        case Keys.Down:
                            p.MC.LabelManager.SelectNext();
                            break;
                    }
                }
                if (function.HasKeyDownAction(p))
                    function.RunKeyDownAction(p);
                if (function.SupressKeyPress(p))
                    SuppressKeyPress = true;
            }
            catch {}
        }

        public bool KeyUp(IMainClass mc, Keys key, bool control, bool alt, bool shift)
        {
            try
            {
                MultiboxFunctionParam p = new MultiboxFunctionParam(key, control, alt, shift, mc);
                IMultiboxFunction function = GetActivatedFunction(p);
                if (function == null)
                {
                    mc.OutputLabelText = "";
                    mc.LabelManager.ResultItems = null;
                    return false;
                }
                if (p.Key == Keys.Enter && !p.Control && !p.Shift && !p.Alt && p.MultiboxText.Trim().Length > 0)
                {
                    if (function.HasActionKeyEvent(p))
                        function.RunActionKeyEvent(p);
                }
                else if (p.Key == Keys.Enter && p.Control && !p.Shift && !p.Alt)
                {
                    string tc = p.DisplayText;
                    if (function.IsMulti(p))
                        tc = p.MC.LabelManager.CurrentSelection != null
                                 ? p.MC.LabelManager.CurrentSelection.FullText
                                 : null;
                    if (function.HasSpecialDisplayCopyHandling(p))
                        tc = function.RunSpecialDisplayCopyHandling(p);
                    if (!string.IsNullOrEmpty(tc))
                        Clipboard = tc;
                }
                else if (p.Key == Keys.Enter && p.Shift && !p.Control && !p.Alt && p.MultiboxText.Trim().Length > 0)
                {
                    string tc = p.MultiboxText;
                    if (function.HasSpecialInputCopyHandling(p))
                        tc = function.RunSpecialInputCopyHandling(p);
                    if (!string.IsNullOrEmpty(tc))
                        Clipboard = tc;
                }
                else
                {
                    bool sr = function.ShouldRun(p);
                    bool ibs = function.IsBackgroundStream(p);
                    if (function.IsMulti(p))
                    {
                        if (sr
                            &&
                            !(p.Key == Keys.Up || p.Key == Keys.Down || p.Key == Keys.ControlKey
                              || p.Key == Keys.ShiftKey))
                        {
                            if (ibs)
                                new RunBgS(function.RunMultiBackgroundStream).BeginInvoke(p, null, null);
                            else
                                p.MC.LabelManager.ResultItems = function.RunMulti(p);
                        }
                        if (function.SupressKeyPress(p) || p.Key == Keys.Up || p.Key == Keys.Down)
                            SuppressKeyPress = true;
                        return true;
                    }
                    if (sr)
                    {
                        if (ibs)
                            new RunBgS(function.RunSingleBackgroundStream).BeginInvoke(p, null, null);
                        else
                            p.MC.OutputLabelText = function.RunSingle(p);
                    }
                }
                if (function.SupressKeyPress(p))
                    SuppressKeyPress = true;
            }
            catch {}
            return false;
        }

        public void SelectionChanged(IMainClass mc)
        {
            try
            {
                MultiboxFunctionParam p = new MultiboxFunctionParam(Keys.None, false, false, false, mc);
                IMultiboxFunction function = GetActivatedFunction(p);
                if (function == null || !function.IsMulti(p) || !function.HasDetails(p))
                {
                    p.MC.DetailsLabelText = "";
                    p.MC.UpdateSize();
                    return;
                }
                bool ibs = function.IsBackgroundDetailsStream(p);
                if (ibs)
                    new RunBgS(function.GetBackgroundDetailsStream).BeginInvoke(p, null, null);
                else
                {
                    p.MC.DetailsLabelText = function.GetDetails(p);
                    p.MC.UpdateSize();
                }
            }
            catch {}
        }
    }
}