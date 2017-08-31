using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExtensionsMetadataGenerator.BuildTasks
{
#if NET46
    public class GenerateFunctionsExtensionsMetadata : AppDomainIsolatedTask    
#else
    public class GenerateFunctionsExtensionsMetadata : Task
#endif
    {
        [Required]
        public string SourcePath { get; set; }

        [Required]
        public string OutputPath { get; set; }

        public override bool Execute()
        {
            Log.LogWarning("Executing");
            string outputPath = Path.Combine(OutputPath, "extensions.json");

            ExtensionsMetadataGenerator.Generate(SourcePath, outputPath);

            return true;
        }

        //private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    Log.LogWarning("Resolving:" + args.Name);
        //    var assemblySearchPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), args.Name.Split(',')[0]);
        //    Log.LogWarning("Path" + assemblySearchPath);
        //    if (File.Exists(assemblySearchPath))
        //    {
        //        try
        //        {
        //            return Assembly.LoadFrom(assemblySearchPath);
        //        }
        //        catch (FileLoadException e)
        //        {
        //            var asm = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains(args.Name)).FirstOrDefault();

        //            if (asm != null)
        //            {
        //                return asm;
        //            }
        //        }
        //    }

        //    return null;
        //}
    }
}
