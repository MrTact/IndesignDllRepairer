using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndesignDllRepairer
{
    class Program
    {
        static void Main(string[] args)
        {
            ILParser p = new ILParser();
            p.Parse("Interop.InDesign.il");
            Console.ReadKey(true);
        }
    }
}
