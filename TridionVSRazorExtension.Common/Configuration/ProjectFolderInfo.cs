using System.Collections.Generic;
using System.Xml.Serialization;
using SDL.TridionVSRazorExtension.Common.Misc;

namespace SDL.TridionVSRazorExtension.Common.Configuration
{
    public class ProjectFolderInfo : ProjectItemInfo
    {
        private string _templateFormat;
        private ProjectFolderRole _projectFolderRole;
        private bool _isExpanded;

        [XmlAttribute("TemplateFormat")]
        public string TemplateFormat
        {
            get { return _templateFormat; }
            set
            {
                if (value == _templateFormat) return;
                _templateFormat = value;
                OnPropertyChanged("TemplateFormat");
            }
        }

        [XmlAttribute("ProjectFolderRole")]
        public ProjectFolderRole ProjectFolderRole
        {
            get { return _projectFolderRole; }
            set
            {
                if (value == _projectFolderRole) return;
                _projectFolderRole = value;
                OnPropertyChanged("ProjectFolderRole");
                OnPropertyChanged("FullName");
            }
        }

        public string FullName
        {
            get
            {
                return string.Format("{0} - {1}", this.ProjectFolderRole, this.Path);
            }
        }

        public List<ProjectItemInfo> ChildItems { get; set; }

        [XmlIgnore]
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value.Equals(_isExpanded)) return;
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }
    }
}