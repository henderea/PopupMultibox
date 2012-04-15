using System.Text.RegularExpressions;
using Multibox.Core.Functions;
using NCalc;

namespace Multibox.Plugin.CalculatorFunction
{
    public class CalculatorFunction : AbstractFunction
    {
        public override int SuggestedIndex()
        {
            return 0;
        }

        private readonly Regex intToDec;
        private readonly Regex prefixDec;

        public CalculatorFunction()
        {
            intToDec = new Regex(@"(\d+(\.)?\d*)", RegexOptions.IgnoreCase);
            prefixDec = new Regex(@"(\D|^)(\.\d+)", RegexOptions.IgnoreCase);
        }

        private static string IntToDecHelper(Match m)
        {
            return m.Value + ((m.Value.Length > 0 && !m.Value.Contains(".")) ? ".0" : "");
        }

        private static string PrefixDecHelper(Match m)
        {
            return m.Groups[1].Value + "0" + m.Groups[2].Value;
        }

        #region IMultiboxFunction Members

        public override bool Triggers(MultiboxFunctionParam args)
        {
            return !string.IsNullOrEmpty(args.MultiboxText);
        }

        public override string RunSingle(MultiboxFunctionParam args)
        {
            try
            {
                string rval;
                Expression tmp = new Expression(intToDec.Replace(prefixDec.Replace(args.MultiboxText, PrefixDecHelper), IntToDecHelper), EvaluateOptions.IgnoreCase);
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
                //return intToDec.Replace(prefixDec.Replace(args.MultiboxText, PrefixDecHelper), IntToDecHelper);
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