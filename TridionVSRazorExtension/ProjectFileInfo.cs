using System.Collections.Generic;
using System.Xml.Serialization;

namespace SDL.TridionVSRazorExtension
{
    public class ProjectFileInfo : ProjectItemInfo
    {
        private List<string> _shemaNames;

        [XmlIgnore]
        public List<string> SchemaNames
        {
            get { return _shemaNames; }
            set
            {
                if (value == _shemaNames) return;
                _shemaNames = value;
                OnPropertyChanged("SchemaNames");
            }
        }

        [XmlAttribute("SchemaNames")]
        public string strSchemaNames
        {
            get
            {
                return _shemaNames == null ? null : string.Join(";", _shemaNames);
            }
            set
            {
                _shemaNames = new List<string>(value.Split(';'));
            }
        }

        [XmlAttribute("Title")]
        public string Title { get; set; }

        [XmlAttribute("TemplateTitle")]
        public string TemplateTitle { get; set; }
    }
}