using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multibox.Core.Functions;

namespace Multibox.Test.TestFramework
{
    public class Tester
    {
        private IMultiboxFunction function;
        private string multiboxText;

        public Tester(IMultiboxFunction function)
        {
            this.function = function;
        }


    }
}