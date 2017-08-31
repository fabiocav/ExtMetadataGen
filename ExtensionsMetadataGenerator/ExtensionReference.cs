using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsMetadataGenerator
{
    public class ExtensionReference
    {
        public string Name { get; set; }

        public string TypeName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HintPath { get; set; }
    }
}
