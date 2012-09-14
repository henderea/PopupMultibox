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
            Tester tester = new Tester(new IMultiboxFunction[] { new CalculatorFunction() }, 10);
            tester.SetText("1024*1024*1024", Keys.NumPad4, false, false, false)
                .CheckIsMulti(false)
                .CheckOutputLabelText("1,073,741,824")
                .KeyPress(Keys.Enter, true, false, false)
                .CheckClipboard("1073741824")
                .KeyPress(Keys.Enter, false, false, true)
                .CheckClipboard("1024*1024*1024");
            Console.WriteLine(tester.PrintHistory());
        }
    }
}
