using System.Collections.Generic;
using System.Xml.Serialization;

namespace SDL.TridionVSRazorExtension
{
    public class Configuration : List<MappingInfo>
    {
        [XmlAttribute("DefaultConfiguration")]
        public string DefaultConfiguration { get; set; }
    }
}