using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SDL.TridionVSRazorExtension.Common.Configuration;
using SDL.TridionVSRazorExtension.Common.Misc;

namespace SDL.TridionVSRazorExtension
{
    public partial class SelectTridionFolderDialogWindow
    {
        public string Path { private get; set; }
        public List<TridionFolderInfo> TridionFolders { private get; set; }
        public TridionFolderInfo SelectedTridionFolder { get; private set; }

        public SelectTridionFolderDialogWindow()
        {
            InitializeComponent();
        }

        public MappingInfo Mapping { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.lblPath.Content = "Path: " + this.Path.CutPath("\\", 80, true);

            this.lstTridionFolders.ItemsSource = this.TridionFolders;
            this.lstTridionFolders.DisplayMemberPath = "NamedPath";
            this.lstTridionFolders.SelectedIndex = 0;
            this.lstTridionFolders.IsEnabled = this.TridionFolders.Count > 1;
        }

        private void lstTridionFolders_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedTridionFolder = this.TridionFolders[this.lstTridionFolders.SelectedIndex];
        }

        private void lstTridionFolders_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}