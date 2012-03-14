using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NCalc;
using System.Windows.Forms;

namespace PopupMultibox
{
    public class CalculatorFunction : AbstractFunction
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

        public override bool Triggers(MultiboxFunctionParam args)
        {
            return (args.MultiboxText != null && args.MultiboxText.Length > 0);
        }

        public override string RunSingle(MultiboxFunctionParam args)
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

        public override bool HasSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return true;
        }

        public override string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return args.DisplayText.Replace(",", "");
        }

        #endregion
    }
}