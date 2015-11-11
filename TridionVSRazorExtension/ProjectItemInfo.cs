using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using SDL.TridionVSRazorExtension.Annotations;

namespace SDL.TridionVSRazorExtension
{
    [XmlInclude(typeof(ProjectFileInfo))]
    [XmlInclude(typeof(ProjectFolderInfo))]
    public class ProjectItemInfo : INotifyPropertyChanged
    {
        private bool? _Checked;
        private bool? _SyncTemplate;
        private string _Path;
        private bool _IsSelected;
        private string _TcmId;

        [XmlIgnore]
        public string RootPath { get; set; }

        [XmlIgnore]
        public bool Handled { get; set; }

        [XmlAttribute("Path")]
        public string Path
        {
            get { return _Path; }
            set
            {
                if (value == _Path) return;
                _Path = value;
                OnPropertyChanged("Path");
                OnPropertyChanged("FullPath");
                OnPropertyChanged("Name");
                if(this.IsFolder) this.OnPropertyChanged("FullName");
            }
        }

        public string FullPath
        {
            get
            {
                return System.IO.Path.Combine(this.RootPath, Path);
            }
        }

        public string Name
        {
            get
            {
                return String.IsNullOrEmpty(Path) ? "(Root)" : System.IO.Path.GetFileName(Path);
            }
        }

        [XmlIgnore]
        public bool? Checked
        {
            get { return _Checked; }
            set
            {
                if (value.Equals(_Checked)) return;
                _Checked = value;
                OnPropertyChanged("Checked");
            }
        }

        [XmlIgnore]
        public bool? SyncTemplate
        {
            get { return _SyncTemplate; }
            set
            {
                if (value.Equals(_SyncTemplate)) return;
                _SyncTemplate = value;
                OnPropertyChanged("SyncTemplate");
            }
        }

        [XmlAttribute("Checked")]
        public string strChecked
        {
            get
            {
                return this.Checked.HasValue ? Convert.ToString(this.Checked.Value) : null;
            }
            set
            {
                this.Checked = !String.IsNullOrEmpty(value) ? Convert.ToBoolean(value) : (bool?)null;
            }
        }

        [XmlAttribute("SyncTemplate")]
        public string strSyncTemplate
        {
            get
            {
                return this.SyncTemplate.HasValue ? Convert.ToString(this.SyncTemplate.Value) : null;
            }
            set
            {
                this.SyncTemplate = !String.IsNullOrEmpty(value) ? Convert.ToBoolean(value) : (bool?)null;
            }
        }

        [XmlAttribute("TcmId")]
        public string TcmId
        {
            get { return _TcmId; }
            set
            {
                if (value == _TcmId) return;
                _TcmId = value;
                OnPropertyChanged("TcmId");
            }
        }

        [XmlIgnore]
        public ProjectFolderInfo Parent { get; set; }

        [XmlIgnore]
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (value.Equals(_IsSelected)) return;
                _IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public bool IsFolder
        {
            get
            {
                return this is ProjectFolderInfo;
            }
        }

        public bool IsFile
        {
            get
            {
                return this is ProjectFileInfo;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}