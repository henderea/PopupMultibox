using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Multibox.Core.Functions;
using Multibox.Test.TestFramework;
using NUnit.Framework;

namespace Multibox.Plugin.CalculatorFunction.Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1()
        {
            Tester tester = new Tester(new IMultiboxFunction[] {new CalculatorFunction()}, 10);
            tester.SetText("1+2", Keys.NumPad2, false, false, false).CheckOutputLabelText("3");
        }
    }
}
