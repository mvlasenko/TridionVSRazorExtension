using System.Collections.Generic;
using System.Xml.Serialization;

namespace SDL.TridionVSRazorExtension.Common.Configuration
{
    public class ProjectFileInfo : ProjectItemInfo
    {
        private List<string> _shemaNames;
        private string _TestItemTcmId;
        private string _TestTemplateTcmId;

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

        [XmlAttribute("TestItemTcmId")]
        public string TestItemTcmId
        {
            get { return _TestItemTcmId; }
            set
            {
                if (value == _TestItemTcmId) return;
                _TestItemTcmId = value;
                OnPropertyChanged("TestItemTcmId");
            }
        }

        [XmlAttribute("TestTemplateTcmId")]
        public string TestTemplateTcmId
        {
            get { return _TestTemplateTcmId; }
            set
            {
                if (value == _TestTemplateTcmId) return;
                _TestTemplateTcmId = value;
                OnPropertyChanged("TestTemplateTcmId");
            }
        }

    }
}