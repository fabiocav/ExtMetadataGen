using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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

            ExtensionsMetadataGenerator.Generate(args[0], args[1], s => { });
        }

    }
}
