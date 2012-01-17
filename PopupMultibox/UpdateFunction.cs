using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PopupMultibox
{
    class UpdateFunction : AbstractFunction
    {
        #region MultiboxFunction Members

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
