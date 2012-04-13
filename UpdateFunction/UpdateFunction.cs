using System.Windows.Forms;
using Multibox.Core.Functions;

namespace Multibox.Plugin.UpdateFunction
{
    class UpdateFunction : AbstractFunction
    {
        public override int SuggestedIndex()
        {
            return 6;
        }

        #region IMultiboxFunction Members

        public override bool Triggers(MultiboxFunctionParam args)
        {
            return args.MultiboxText.Equals("update");
        }

        public override string RunSingle(MultiboxFunctionParam args)
        {
            if (args.Key == Keys.Tab)
            {
                args.MC.VChk.checkForUpdateForce();
                return "No update found";
            }
            return "Check for updates";
        }

        public override bool SupressKeyPress(MultiboxFunctionParam args)
        {
            return (args.Key == Keys.Tab);
        }

        #endregion
    }
}
