using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExtensionsMetadataGenerator
{
    public class ExtensionsMetadataGenerator
    {
        private static Type configProviderType = typeof(IExtensionConfigProvider);
        
        public static void Generate(string sourcePath, string outputPath)
        {
            System.Console.WriteLine($"Generating extension {sourcePath} - {outputPath}");

            if (!Directory.Exists(sourcePath))
            {
                throw new DirectoryNotFoundException($"The path `{sourcePath}` does not exist. Unable to generate Azure Functions extensions metadata file.");
            }

            List<string> extensionObjects = new List<string>();
            foreach (var path in Directory.EnumerateFiles(sourcePath, "*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(path);

                    assembly.ExportedTypes
                        .Where(t => configProviderType.IsAssignableFrom(t))
                        .Select(t => new ExtensionReference
                        {
                            Name = t.Name,
                            TypeName = t.AssemblyQualifiedName
                        })
                        .Aggregate(extensionObjects, (a, r) =>
                        {
                            a.Add(string.Format("{{ \"name\": \"{0}\", \"typeName\":\"{1}\"}}", r.Name, r.TypeName));
                            return a;
                        });

                    //var metadata = new
                    //{
                    //    extensions = new List<ExtensionReference>(extensions)
                    //};

                    //var serializationSettings = new JsonSerializerSettings
                    //{
                    //    Formatting = Formatting.Indented,
                    //    ContractResolver = new DefaultContractResolver
                    //    {
                    //        NamingStrategy = new CamelCaseNamingStrategy()
                    //    }
                    //};

                    

                    

                }
                catch (Exception)
                {
                    // TODO: Support logging

                }
            }

            string metadataContents = string.Format("{{ \"extensions\":[{0}]}}", string.Join(",", extensionObjects));

            File.WriteAllText(outputPath, metadataContents);
        }
    }
}
