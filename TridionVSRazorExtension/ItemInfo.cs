using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SDL.TridionVSRazorExtension.Annotations;
using Tridion.ContentManager.CoreService.Client;

namespace SDL.TridionVSRazorExtension
{
    public class ItemInfo : INotifyPropertyChanged
    {
        private List<ItemInfo> _childItems;
        private string _tcmId;
        private string _title;
        private bool _isSelected;
        private bool _isExpanded;

        public string TcmId
        {
            get { return _tcmId; }
            set
            {
                if (value == _tcmId) return;
                _tcmId = value;
                OnPropertyChanged("TcmId");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public ItemType ItemType { get; set; }

        public string MimeType { get; set; }

        public string FromPub { get; set; }

        public bool IsPublished { get; set; }

        public List<ItemInfo> ChildItems
        {
            get { return _childItems; }
            set
            {
                if (Equals(value, _childItems)) return;
                _childItems = value;
                OnPropertyChanged("ChildItems");
            }
        }

        public ItemInfo Parent { get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value.Equals(_isSelected)) return;
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

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

        public Uri Icon
        {
            get
            {
                if (this.ItemType == ItemType.Publication)
                    return new Uri("pack://application:,,,/TridionVSRazorExtension;component/Resources/pub.png");
                if (this.ItemType == ItemType.Folder)
                    return new Uri("pack://application:,,,/TridionVSRazorExtension;component/Resources/fld.png");
                if (this.ItemType == ItemType.StructureGroup)
                    return new Uri("pack://application:,,,/TridionVSRazorExtension;component/Resources/sg.png");

                return new Uri("pack://application:,,,/TridionVSRazorExtension;component/Resources/tbb.png");
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