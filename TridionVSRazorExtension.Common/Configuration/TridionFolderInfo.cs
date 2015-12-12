using System.Collections.Generic;
using System.Xml.Serialization;
using SDL.TridionVSRazorExtension.Common.Misc;

namespace SDL.TridionVSRazorExtension.Common.Configuration
{
    public class TridionFolderInfo
    {
        public string TcmId { get; set; }

        public List<string> TcmIdPath { get; set; }

        public string NamedPath { get; set; }

        public TridionRole TridionRole { get; set; }

        public bool ScanForItems { get; set; }

        [XmlIgnore]
        public List<TridionFolderInfo> ChildFolders { get; set; }

        [XmlIgnore]
        public TridionFolderInfo ParentFolder { get; set; }

        public string NamedPathCut
        {
            get
            {
                return this.NamedPath.CutPath("/", 50, true);
            }
        }

        public string NamedPathCut2
        {
            get
            {
                return this.NamedPath.CutPath("/", 95, true);
            }
        }
    }
}