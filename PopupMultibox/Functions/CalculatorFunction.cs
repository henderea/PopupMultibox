using System.Text.RegularExpressions;
using NCalc;

namespace PopupMultibox.Functions
{
    public class CalculatorFunction : AbstractFunction
    {
        private readonly Regex intToDec;

        public CalculatorFunction()
        {
            intToDec = new Regex(@"(\d+(\.)?\d*)", RegexOptions.IgnoreCase);
        }

        private static string IntToDecHelper(Match m)
        {
            return m.Value + ((m.Value.Length > 0 && !m.Value.Contains(".")) ? ".0" : "");
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
                Expression tmp = new Expression(intToDec.Replace(args.MultiboxText, IntToDecHelper), EvaluateOptions.IgnoreCase);
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