using System;
using System.Diagnostics;
using System.Threading;

namespace ExtensionsMetadataGenerator.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage: ");
                System.Console.WriteLine("metadatagen <sourcepath> <output>");

                return;
            }

            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }

            ExtensionsMetadataGenerator.Generate(args[0], args[1]);
        }
    }
}
