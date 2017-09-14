﻿using Microsoft.Azure.WebJobs.Script.ExtensionsMetadataGenerator;
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
            string outputPath = Path.Combine(OutputPath, "extensions.json");

            if (SourcePath.EndsWith("\\"))
            {
                SourcePath = Path.GetDirectoryName(SourcePath);
            }

            Assembly taskAssembly = typeof(GenerateFunctionsExtensionsMetadata).Assembly;

            var info = new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(taskAssembly.Location),
                FileName = DotNetMuxer.MuxerPathOrDefault(),
                Arguments = $"{taskAssembly.GetName().Name}.dll \"{SourcePath}\" \"{outputPath}\""
            };

            var process = new Process { StartInfo = info };
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += (s, e) => { if (e.Data != null) Log.LogWarning(e.Data); };
            
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Log.LogError("Metadata generation failed.");

                return false;
            }

            process.Close();

            //ExtensionsMetadataGenerator.Generate(SourcePath, outputPath, s => Log.LogWarning(s));
            return true;
        }
    }
}
