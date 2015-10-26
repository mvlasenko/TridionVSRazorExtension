using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SDL.TridionVSRazorExtension
{
    public partial class MappingWindow
    {
        public string RootPath { private get; set; }

        private Configuration _Configuration;
        public Configuration Configuration
        {
            get
            {
                return _Configuration ?? (_Configuration = Functions.GetDefault(this.RootPath));
            }
            set
            {
                _Configuration = value;
            }
        }

        public MappingInfo CurrentMapping
        {
            get
            {
                return this.Configuration.Find(x => x.Name == this.cbMappingName.SelectedValue.ToString());
            }
        }

        private void ReloadMappings(string mappingName)
        {
            this.cbMappingName.Items.Clear();
            foreach (MappingInfo mapping in this.Configuration)
            {
                this.cbMappingName.Items.Add(mapping.Name);
            }

            this.cbMappingName.SelectedIndex = this.Configuration.FindIndex(x => x.Name == mappingName);
        }

        private void ReloadForm()
        {
            this.txtName.Text = this.CurrentMapping.Name;
            this.txtHost.Text = this.CurrentMapping.Host;
            this.txtUsername.Text = this.CurrentMapping.Username;
            this.txtPassword.Password = this.CurrentMapping.Password;

            ICollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
            this.cbTimeZone.ItemsSource = tz;
            this.cbTimeZone.DisplayMemberPath = "DisplayName";

            string timeZoneId = String.IsNullOrEmpty(this.CurrentMapping.TimeZoneId) ? TimeZoneInfo.Local.Id : this.CurrentMapping.TimeZoneId;
            this.cbTimeZone.SelectedItem = tz.First(t => t.Id == timeZoneId);

            this.dataGridTridionMapping.DataContext = this.CurrentMapping;

            this.dataGridProjectMapping.DataContext = this.CurrentMapping;

            this.btnSaveRun.IsEnabled = this.CurrentMapping.Valid;
        }

        private void SaveConfiguration()
        {
            this.Configuration.DefaultConfiguration = this.txtName.Text;
            this.CurrentMapping.Name = this.txtName.Text;

            this.CurrentMapping.Host = this.txtHost.Text;
            this.CurrentMapping.Username = this.txtUsername.Text;
            this.CurrentMapping.Password = this.txtPassword.Password;
            this.CurrentMapping.TimeZoneId = (this.cbTimeZone.SelectedValue as TimeZoneInfo ?? TimeZoneInfo.Local).Id;

            Functions.SaveConfiguration(this.RootPath, "TridionRazorMapping.xml", this.Configuration);
        }

        public MappingWindow()
        {
            InitializeComponent();
            Functions.CredentialsChanged += CredentialsChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Functions.TxtLog = this.txtLog;
            Functions.RootPath = this.RootPath;

            //get configuration from xml
            try
            {
                this.Configuration = Functions.GetConfiguration(this.RootPath, "TridionRazorMapping.xml");
            }
            catch (Exception)
            {

            }

            string mappingName = this.Configuration.DefaultConfiguration ?? "Default";
            this.ReloadMappings(mappingName);
            this.ReloadForm();
        }

        private void CredentialsChanged()
        {
            this.txtUsername.Text = this.CurrentMapping.Username;
            this.txtPassword.Password = this.CurrentMapping.Password;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            this.Configuration.Add(Functions.GetDefaultMapping(this.RootPath, "(new mapping)"));
            this.ReloadMappings("(new mapping)");
            this.ReloadForm();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.Configuration.Count > 1)
            {
                this.Configuration.Remove(this.CurrentMapping);
                this.ReloadMappings(this.Configuration[0].Name);
                this.ReloadForm();
            }
            else
            {
                MessageBox.Show("This is the last mapping", "This is the last mapping", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbMappingName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbMappingName.SelectedValue == null || this.cbMappingName.SelectedValue.ToString() == String.Empty || this.CurrentMapping == null)
                return;
            
            this.ReloadForm();

            Functions.ResetClient();
            Functions.ResetDownloadClient();
            Functions.ResetUploadClient();
        }

        private void DataGridTridionMapping_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender == null)
                return;

            DataGrid grid = sender as DataGrid;
            if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
            {
                TridionFolderInfo tridionFolder = grid.SelectedItem as TridionFolderInfo;
                if (tridionFolder != null)
                {
                    SelectTridionTreeNodeDialogWindow dialog = new SelectTridionTreeNodeDialogWindow();
                    dialog.TridionSelectorMode = TridionSelectorMode.Folder;
                    dialog.TridionFolder = tridionFolder;
                    dialog.CurrentMapping = this.CurrentMapping;
                    bool res = dialog.ShowDialog() == true;
                    if (res)
                    {
                        CollectionViewSource.GetDefaultView(this.dataGridTridionMapping.ItemsSource).Refresh();
                    }
                }
            }
        }

        private void btnAddTridionFolder_Click(object sender, RoutedEventArgs e)
        {
            SelectTridionTreeNodeDialogWindow dialog = new SelectTridionTreeNodeDialogWindow();
            dialog.TridionSelectorMode = TridionSelectorMode.Folder;
            dialog.TridionFolder = new TridionFolderInfo();
            dialog.CurrentMapping = this.CurrentMapping;
            bool res = dialog.ShowDialog() == true;
            if (res)
            {
                this.CurrentMapping.TridionFolders.Add(dialog.TridionFolder);
                CollectionViewSource.GetDefaultView(this.dataGridTridionMapping.ItemsSource).Refresh();
            }
        }

        private void DataGridProjectMapping_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender == null)
                return;

            DataGrid grid = sender as DataGrid;
            if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
            {
                ProjectFolderInfo projectFolder = grid.SelectedItem as ProjectFolderInfo;
                if (projectFolder != null)
                {
                    Functions.CreateVSFolder(projectFolder.FullPath);
                    
                    ProjectFolderDialogWindow dialog = new ProjectFolderDialogWindow();
                    dialog.RootPath = this.RootPath;
                    dialog.CurrentProjectFolder = projectFolder;
                    dialog.TridionFolders = this.CurrentMapping.TridionFolders.FillNamedPath(this.CurrentMapping);
                    bool res = dialog.ShowDialog() == true;
                    if (res)
                    {
                        CollectionViewSource.GetDefaultView(this.dataGridProjectMapping.ItemsSource).Refresh();
                    }
                }
            }
        }

        private void btnAddProjectFolder_Click(object sender, RoutedEventArgs e)
        {
            ProjectFolderDialogWindow dialog = new ProjectFolderDialogWindow();
            dialog.RootPath = this.RootPath;
            dialog.CurrentProjectFolder = new ProjectFolderInfo { RootPath = this.RootPath, Path = "", Checked = false };
            dialog.TridionFolders = this.CurrentMapping.TridionFolders.FillNamedPath(this.CurrentMapping);
            bool res = dialog.ShowDialog() == true;
            if (res)
            {
                ProjectFolderInfo projectFolder = dialog.CurrentProjectFolder;
                if (projectFolder != null)
                {
                    this.CurrentMapping.ProjectFolders.Add(projectFolder);
                    CollectionViewSource.GetDefaultView(this.dataGridProjectMapping.ItemsSource).Refresh();
                }
            }
        }

        private void txtName_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Configuration.DefaultConfiguration = this.txtName.Text;
            this.CurrentMapping.Name = this.txtName.Text;
            this.ReloadMappings(this.txtName.Text);
        }

        private void mapping_LostFocus(object sender, RoutedEventArgs e)
        {
            this.CurrentMapping.Host = this.txtHost.Text;
            this.CurrentMapping.Username = this.txtUsername.Text;
            this.CurrentMapping.Password = this.txtPassword.Password;
            this.CurrentMapping.TimeZoneId = (this.cbTimeZone.SelectedValue as TimeZoneInfo ?? TimeZoneInfo.Local).Id;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.SaveConfiguration();
            this.DialogResult = true;
            this.Close();
        }

        private void btnSaveRun_Click(object sender, RoutedEventArgs e)
        {
            this.SaveConfiguration();

            //run current
            foreach (ProjectFolderInfo folder in this.CurrentMapping.ProjectFolders)
            {
                Functions.ProcessFolder(this.CurrentMapping, folder);
            }

            foreach (TridionFolderInfo tridionFolder in this.CurrentMapping.TridionFolders)
            {
                Functions.ProcessTridionFolder(this.CurrentMapping, tridionFolder);
            }

            Functions.CheckRazorWebConfig(this.CurrentMapping);

            this.SaveConfiguration();

            MessageBox.Show("Synchronization finished for mapping \"" + this.CurrentMapping.Name + "\"", "Finish", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            if (Functions.TestConnection(this.CurrentMapping) && Functions.EnsureValidClient(this.CurrentMapping))
            {
                this.CurrentMapping.Valid = true;
                this.btnSaveRun.IsEnabled = true;
                MessageBox.Show("SUCCSESS", "SUCCSESS", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}