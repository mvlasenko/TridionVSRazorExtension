using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using SDL.TridionVSRazorExtension.Common.Configuration;
using SDL.TridionVSRazorExtension.Common.Misc;

namespace SDL.TridionVSRazorExtension
{
    public partial class ProjectDestinationDialogWindow
    {
        public ProjectDestinationDialogWindow()
        {
            InitializeComponent();
        }

        public MappingInfo Mapping { private get; set; }
        public TridionRole TridionRole { private get; set; }
        public string TridionTcmId { private get; set; }
        public string TridionTitle { private get; set; }
        public string TridionContent { private get; set; }

        public ProjectFolderInfo ProjectFolder { get; private set; }
        public ProjectFileInfo ProjectFile { get; private set; }

        private List<ProjectFolderInfo> ProjectFolders
        {
            get
            {
                if (this.TridionRole == TridionRole.PageLayoutContainer)
                {
                    return Mapping.ProjectFolders.Where(x => x.ProjectFolderRole == ProjectFolderRole.PageLayout).ToList();
                }
                if (this.TridionRole == TridionRole.ComponentLayoutContainer)
                {
                    return Mapping.ProjectFolders.Where(x => x.ProjectFolderRole == ProjectFolderRole.ComponentLayout).ToList();
                }
                return Mapping.ProjectFolders.Where(x => x.ProjectFolderRole != ProjectFolderRole.PageLayout && x.ProjectFolderRole != ProjectFolderRole.ComponentLayout).ToList();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtTitle.Text = this.TridionTitle;

            this.txtSource.Text = this.TridionContent;

            this.lstDestination.ItemsSource = this.ProjectFolders;
            this.lstDestination.DisplayMemberPath = "FullName";
            this.lstDestination.SelectedIndex = 0;

            this.chkChecked.IsChecked = true;
        }

        private void ListBox1_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProjectFolderInfo folder = this.ProjectFolders[this.lstDestination.SelectedIndex];
            if (folder == null)
                return;

            ProjectFolderRole role = folder.ProjectFolderRole;

            this.chkSyncTemplate.IsEnabled = role == ProjectFolderRole.PageLayout || role == ProjectFolderRole.ComponentLayout;
            this.chkSyncTemplate.IsChecked = Common.IsolatedStorage.Service.GetFromIsolatedStorage(Common.IsolatedStorage.Service.GetId(Mapping.Host, "ProjectDestination_SyncTemplate")) == "true";
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

            Common.IsolatedStorage.Service.SaveToIsolatedStorage(Common.IsolatedStorage.Service.GetId(Mapping.Host, "ProjectDestination_SyncTemplate"), this.chkSyncTemplate.IsChecked == true ? "true" : "");

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

            string path = Path.Combine(this.ProjectFolder.Path, this.txtTitle.Text.Replace(".cshtml", "") + ".cshtml");

            this.ProjectFile = this.ProjectFolder.ChildItems.OfType<ProjectFileInfo>().FirstOrDefault(x => x.Path == path) ?? new ProjectFileInfo();
            this.ProjectFile.Parent = this.ProjectFolder;
            this.ProjectFile.RootPath = this.ProjectFolder.RootPath;
            this.ProjectFile.Path = path;
            this.ProjectFile.TcmId = TridionTcmId;
            this.ProjectFile.Checked = this.chkChecked.IsChecked == true;

            if (this.ProjectFolder.ChildItems.OfType<ProjectFileInfo>().All(x => x.Path != path))
            {
                this.ProjectFolder.ChildItems.Add(this.ProjectFile);
            }
        }

    }
}