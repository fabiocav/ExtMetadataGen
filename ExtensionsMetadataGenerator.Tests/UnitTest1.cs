using ExtensionsMetadataGenerator.BuildTasks;
using System;
using Xunit;

namespace ExtensionsMetadataGenerator.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
          //  ExtensionsMetadataGenerator.Generate("D:\\funcbits", "d:\\funcbits\\extensions.json", s => { });

            var task = new GenerateFunctionsExtensionsMetadata();
            task.SourcePath = "D:\\funcbits";
            task.OutputPath = "d:\\funcbits";

            task.Execute();
        }
    }
}
