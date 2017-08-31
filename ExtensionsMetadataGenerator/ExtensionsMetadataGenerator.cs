using Microsoft.Azure.WebJobs.Script.ExtensionsMetadataGenerator;
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
        public static void Generate(string sourcePath, string outputPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                throw new DirectoryNotFoundException($"The path `{sourcePath}` does not exist. Unable to generate Azure Functions extensions metadata file.");
            }

            var extensionReferences = new List<ExtensionReference>();
            var targetAssemblies = Directory.EnumerateFiles(sourcePath, "*.dll")
                .Where(f => ! Path.GetFileName(f).StartsWith("System", StringComparison.OrdinalIgnoreCase));

            foreach (var path in targetAssemblies)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(path);

                    var extensions = assembly.ExportedTypes
                        .Where(t => t.IsExtensionType())
                        .Select(t => new ExtensionReference
                        {
                            Name = t.Name,
                            TypeName = t.AssemblyQualifiedName
                        });

                    extensionReferences.AddRange(extensions);
                }
                catch (Exception)
                {
                }
            }

            var referenceObjects = extensionReferences.Select(r => string.Format("{{ \"name\": \"{0}\", \"typeName\":\"{1}\"}}", r.Name, r.TypeName));
            string metadataContents = string.Format("{{ \"extensions\":[{0}]}}", string.Join(",", referenceObjects));
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
    }
}
