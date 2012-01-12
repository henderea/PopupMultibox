using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Startup
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string str = "";
                for (int i = 0; i < args.Length; i++)
                {
                    str += args[i];
                    if (i < args.Length - 1)
                        str += " ";
                }
                Console.WriteLine(str);
                Process.Start(str);
            }
            catch { }
        }
    }
}
