﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SDL.TridionVSRazorExtension.Annotations;
using Tridion.ContentManager.CoreService.Client;

namespace SDL.TridionVSRazorExtension.Tridion
{
    public class HistoryItemInfo : INotifyPropertyChanged
    {
        private string _TcmId;
        private string _Title;
        private int _Version;
        private DateTime _Modified; 
        private bool _Current;

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

        public string Title
        {
            get { return _Title; }
            set
            {
                if (value == _Title) return;
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        public int Version
        {
            get { return _Version; }
            set
            {
                if (value == _Version) return;
                _Version = value;
                OnPropertyChanged("Version");
            }
        }

        public DateTime Modified
        {
            get { return _Modified; }
            set
            {
                if (value == _Modified) return;
                _Modified = value;
                OnPropertyChanged("Modified");
            }
        }

        public bool Current
        {
            get { return _Current; }
            set
            {
                if (value == _Current) return;
                _Current = value;
                OnPropertyChanged("Current");
            }
        }

        public ItemType ItemType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}