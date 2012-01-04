using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NCalc;
using System.Windows.Forms;

namespace PopupMultibox
{
    public class CalculatorFunction : MultiboxFunction
    {
        private Regex intToDec;

        public CalculatorFunction()
        {
            intToDec = new Regex(@"(\d+(\.)?\d*)", RegexOptions.IgnoreCase);
        }

        private string IntToDecHelper(Match m)
        {
            return m.Value + ((m.Value.Length > 0 && !m.Value.Contains(".")) ? ".0" : "");
        }

        #region MultiboxFunction Members

        public bool Triggers(MultiboxFunctionParam args)
        {
            return (args.MultiboxText != null && args.MultiboxText.Length > 0);
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
            try
            {
                string rval = "";
                Expression tmp = new Expression(intToDec.Replace(args.MultiboxText, new MatchEvaluator(IntToDecHelper)), EvaluateOptions.IgnoreCase);
                if (tmp.HasErrors())
                    rval = tmp.Error;
                else
                {
                    try
                    {
                        rval = ((int)tmp.Evaluate()).ToString("#,##0.#########");
                    }
                    catch
                    {
                        try
                        {
                            rval = ((float)tmp.Evaluate()).ToString("#,##0.#########");
                        }
                        catch
                        {
                            try
                            {
                                rval = ((double)tmp.Evaluate()).ToString("#,##0.#########");
                            }
                            catch
                            {
                                rval = double.Parse("" + tmp.Evaluate()).ToString("#,##0.#########");
                            }
                        }
                    }
                }
                return rval;
            }
            catch { }
            return "";
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
            return false;
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
            return true;
        }

        public string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return args.DisplayText.Replace(",", "");
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