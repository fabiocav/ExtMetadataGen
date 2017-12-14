using Microsoft.Azure.WebJobs.Script.ExtensionsMetadataGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
#if !NET46
using System.Runtime.Loader;
#endif

namespace ExtensionsMetadataGenerator
{
    public class ExtensionsMetadataGenerator
    {
        public static void Generate(string sourcePath, string outputPath, Action<string> logger)
        {
            if (!Directory.Exists(sourcePath))
            {
                throw new DirectoryNotFoundException($"The path `{sourcePath}` does not exist. Unable to generate Azure Functions extensions metadata file.");
            }

            var assemblyLoader = new AssemblyLoader(sourcePath);

            var extensionReferences = new List<ExtensionReference>();
            var targetAssemblies = Directory.EnumerateFiles(sourcePath, "*.dll")
                .Where(f => !Path.GetFileName(f).StartsWith("System", StringComparison.OrdinalIgnoreCase));

            foreach (var path in targetAssemblies)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(path);

                    var extensions = assembly.GetExportedTypes()
                        .Where(t => t.IsExtensionType())
                        .Select(t => new ExtensionReference
                        {
                            Name = t.Name,
                            TypeName = t.AssemblyQualifiedName
                        });

                    extensionReferences.AddRange(extensions);
                }
                catch (Exception exc)
                {
                    logger(exc.Message ?? $"Errot processing {path}");
                }
            }

            var referenceObjects = extensionReferences.Select(r => string.Format("{2}    {{ \"name\": \"{0}\", \"typeName\":\"{1}\"}}", r.Name, r.TypeName, Environment.NewLine));
            string metadataContents = string.Format("{{{1}  \"extensions\":[{0}{1}  ]{1}}}", string.Join(",", referenceObjects), Environment.NewLine);
            File.WriteAllText(outputPath, metadataContents);

            //var serializationSettings = new JsonSerializerSettings
            //{
            //    Formatting = Formatting.Indented,
            //    ContractResolver = new DefaultContractResolver
            //    {
            //        NamingStrategy = new CamelCaseNamingStrategy()
            //    }
            //};

            //File.WriteAllText(outputPath, JsonConvert.SerializeObject(new { extensions = extensionReferences }, serializationSettings));
        }

        private class AssemblyLoader
        {
            private readonly string _basePath;

            public AssemblyLoader(string basePath)
            {
                _basePath = basePath;
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }

            private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
            {
                
                string assemblyName = new AssemblyName(args.Name).Name;
                string assemblyPath = Path.Combine(_basePath, assemblyName + ".dll");

                if (File.Exists(assemblyPath))
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);//Assembly.LoadFrom(assemblyPath);

                    return assembly;
                }

                return null;
            }
        }
    }
}
