using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using SDL.TridionVSRazorExtension.Common.Configuration;
using SDL.TridionVSRazorExtension.Common.Misc;

namespace SDL.TridionVSRazorExtension
{
    public partial class ProjectBinaryDestinationDialogWindow
    {
        public ProjectBinaryDestinationDialogWindow()
        {
            InitializeComponent();
        }

        public MappingInfo Mapping { private get; set; }
        public string TridionTcmId { private get; set; }
        public string TridionTitle { private get; set; }

        public ProjectFolderInfo ProjectFolder { get; private set; }
        public ProjectFileInfo ProjectFile { get; private set; }

        private List<ProjectFolderInfo> ProjectFolders
        {
            get
            {
                return Mapping.ProjectFolders.Where(x => x.ProjectFolderRole == ProjectFolderRole.Binary).ToList();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string extension = MainService.GetMultimediaComponentFileExtension(this.Mapping, this.TridionTcmId);

            this.txtTitle.Text = this.TridionTitle == Path.GetFileNameWithoutExtension(this.TridionTitle) ? this.TridionTitle + extension : this.TridionTitle;

            this.lstDestination.ItemsSource = this.ProjectFolders;
            this.lstDestination.DisplayMemberPath = "FullName";
            this.lstDestination.SelectedIndex = 0;

            this.chkChecked.IsChecked = true;
        }

        private void ListBox1_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.SaveResult();

            this.DialogResult = true;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.SaveResult();

            if (this.chkSkip.IsChecked == true)
            {
                MainService.ProjectDestination_Skip = true;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void SaveResult()
        {
            this.ProjectFolder = this.ProjectFolders[this.lstDestination.SelectedIndex];
            if (this.ProjectFolder == null)
                return;

            if (this.ProjectFolder.ChildItems == null)
                this.ProjectFolder.ChildItems = new List<ProjectItemInfo>();

            string path = Path.Combine(this.ProjectFolder.Path, this.txtTitle.Text);

            this.ProjectFile = this.ProjectFolder.ChildItems.OfType<ProjectFileInfo>().FirstOrDefault(x => x.Path == path) ?? new ProjectFileInfo();
            this.ProjectFile.Parent = this.ProjectFolder;
            this.ProjectFile.RootPath = this.ProjectFolder.RootPath;
            this.ProjectFile.Path = path;
            this.ProjectFile.TcmId = this.TridionTcmId;
            this.ProjectFile.Checked = this.chkChecked.IsChecked == true;

            if (this.ProjectFolder.ChildItems.OfType<ProjectFileInfo>().All(x => x.Path != path))
            {
                this.ProjectFolder.ChildItems.Add(this.ProjectFile);
            }
        }

    }
}