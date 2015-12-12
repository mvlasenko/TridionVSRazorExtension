using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using SDL.TridionVSRazorExtension.Common.Configuration;
using SDL.TridionVSRazorExtension.Common.Misc;

namespace SDL.TridionVSRazorExtension
{
    public partial class ProjectFolderDialogWindow
    {
        public string RootPath { private get; set; }
        public List<TridionFolderInfo> TridionFolders { private get; set; }
        public ProjectFolderInfo CurrentProjectFolder { get; set; }
        public MappingInfo CurrentMapping { get; set; }

        private ProjectItemInfo _ProjectItem;
        private ProjectFolderInfo _TopFolder;
        private IEnumerable<ProjectFolderInfo> _AllTree;
        private TridionRole _TridionRole;

        public ProjectFolderDialogWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this._ProjectItem = this.CurrentProjectFolder;
            this._AllTree = Service.GetFileTree(this.CurrentProjectFolder, this.RootPath);
            this._TopFolder = this.CurrentProjectFolder;

            this.CurrentProjectFolder.Expand();
            this.treeProjectFolder.ItemsSource = this._AllTree;

            this.cbRoles.ItemsSource = Enum.GetValues(typeof(ProjectFolderRole)).Cast<ProjectFolderRole>();
            this.cbRoles.SelectedValue = this.CurrentProjectFolder.ProjectFolderRole;
            this.cbRoles.IsEnabled = true;

            this.SetVisibility(this.CurrentProjectFolder.ProjectFolderRole);
            this.SetForm();
        }

        private void SetVisibility(ProjectFolderRole role)
        {
            if (role == ProjectFolderRole.PageLayout)
            {
                this.txtTemplateFormat.IsEnabled = true;
                this.txtSchemaNames.IsEnabled = false;

                this.txtSchemaNames.Text = null;

                this._TridionRole = TridionRole.PageLayoutContainer;
            }

            if (role == ProjectFolderRole.ComponentLayout)
            {
                this.txtTemplateFormat.IsEnabled = true;
                this.txtSchemaNames.IsEnabled = true;

                this._TridionRole = TridionRole.ComponentLayoutContainer;
            }

            if (role == ProjectFolderRole.Binary)
            {
                this.txtTemplateFormat.IsEnabled = false;
                this.txtSchemaNames.IsEnabled = false;

                this.txtTemplateFormat.Text = null;
                this.txtSchemaNames.Text = null;

                this._TridionRole = TridionRole.MultimediaComponentContainer;
            }

            if (role == ProjectFolderRole.Other)
            {
                this.txtTemplateFormat.IsEnabled = false;
                this.txtSchemaNames.IsEnabled = false;

                this.txtTemplateFormat.Text = null;
                this.txtSchemaNames.Text = null;

                this._TridionRole = TridionRole.Other;
            }

            List<TridionFolderInfo> items = this.TridionFolders.Where(x => x.TridionRole == this._TridionRole).ToList();
            this.lstTridionFolders.ItemsSource = items;
            this.lstTridionFolders.DisplayMemberPath = "NamedPathCut2";
            this.lstTridionFolders.SelectedIndex = 0;
            this.lstTridionFolders.IsEnabled = items.Count > 1;
        }

        private void SetForm()
        {
            ProjectFolderRole role = this._TopFolder.ProjectFolderRole;

            this.cbRoles.IsEnabled = this._ProjectItem.IsFolder;

            this.chkSyncTemplate.IsEnabled = this._ProjectItem.IsFile && (role == ProjectFolderRole.ComponentLayout || role == ProjectFolderRole.PageLayout);
            if (role == ProjectFolderRole.ComponentLayout || role == ProjectFolderRole.PageLayout)
            {
                this.chkSyncTemplate.IsChecked = this._ProjectItem.IsSyncTemplate();
            }
            else
            {
                this.chkSyncTemplate.IsChecked = false;
            }

            this.btnDebug.IsEnabled = this._ProjectItem.IsFile && (role == ProjectFolderRole.ComponentLayout || role == ProjectFolderRole.PageLayout);
            if (this.btnDebug.IsEnabled)
            {
                ProjectFileInfo projectFile = (ProjectFileInfo) this._ProjectItem;
                this.btnDebug.Foreground = new SolidColorBrush(!string.IsNullOrEmpty(projectFile.TestItemTcmId) && !string.IsNullOrEmpty(projectFile.TestTemplateTcmId) ? Colors.Green : Colors.Red);
            }
            else
            {
                this.btnDebug.Foreground = new SolidColorBrush(Colors.Gray);
            }

            this.cbRoles.SelectedValue = role;

            if (role == ProjectFolderRole.Other)
                return;

            if (role == ProjectFolderRole.Binary)
            {
                this.txtTemplateFormat.Text = String.Empty;
                this.txtTemplateFormat.IsEnabled = false;

                this.txtSchemaNames.Text = String.Empty;
                this.txtSchemaNames.IsEnabled = false;

                if (this._ProjectItem.IsFolder)
                {
                    this.txtTitle.Text = null;
                    this.txtTitle.IsEnabled = false;
                }
                else
                {
                    ProjectFileInfo file = (ProjectFileInfo)this._ProjectItem;

                    this.txtTitle.Text = String.IsNullOrEmpty(file.Title) ? Path.GetFileNameWithoutExtension(file.Path) : file.Title;
                    file.Title = String.IsNullOrEmpty(this.txtTitle.Text) ? null : this.txtTitle.Text;
                    this.txtTitle.IsEnabled = true;
                }
                
                this.txtTemplateTitle.Text = null;
                this.txtTemplateTitle.IsEnabled = false;
            }
            else
            {
                if (this._ProjectItem.IsFolder)
                {
                    if (this._ProjectItem.Path == this._TopFolder.Path)
                    {
                        this.txtTemplateFormat.Text = this._TopFolder.TemplateFormat.PrettyXml();
                        this.txtTemplateFormat.IsEnabled = true;
                    }
                    else
                    {
                        this.txtTemplateFormat.Text = String.Empty;
                        this.txtTemplateFormat.IsEnabled = false;
                    }

                    this.txtSchemaNames.Text = String.Empty;
                    this.txtSchemaNames.IsEnabled = false;

                    this.txtTitle.Text = null;
                    this.txtTitle.IsEnabled = false;

                    this.txtTemplateTitle.Text = null;
                    this.txtTemplateTitle.IsEnabled = false;
                }
                else
                {
                    ProjectFileInfo file = (ProjectFileInfo)this._ProjectItem;

                    this.txtTemplateFormat.Text = this._TopFolder.TemplateFormat.PrettyXml();
                    this.txtTemplateFormat.IsEnabled = false;

                    this.txtSchemaNames.Text = file.SchemaNames == null ? String.Empty : string.Join("\n", file.SchemaNames);
                    this.txtSchemaNames.IsEnabled = true;

                    string title = Path.GetFileNameWithoutExtension(file.Path);

                    this.txtTitle.Text = String.IsNullOrEmpty(file.Title) ? (string.IsNullOrEmpty(title) ? null : title.Replace(" Layout", "") + " Layout") : file.Title;
                    file.Title = String.IsNullOrEmpty(this.txtTitle.Text) ? null : this.txtTitle.Text;
                    this.txtTitle.IsEnabled = true;

                    this.txtTemplateTitle.Text = String.IsNullOrEmpty(file.TemplateTitle) ? (string.IsNullOrEmpty(title) ? null : title.Replace(" Layout", "")) : file.TemplateTitle;
                    file.TemplateTitle = String.IsNullOrEmpty(this.txtTemplateTitle.Text) ? null : this.txtTemplateTitle.Text;
                    this.txtTemplateTitle.IsEnabled = this.chkSyncTemplate.IsChecked == true;
                }
            }

            List<TridionFolderInfo> tridionFolders = this.TridionFolders.Where(x => x.TridionRole == this._TridionRole).ToList();
            this.lstTridionFolders.SelectedIndex = tridionFolders.Any(x => x.TcmId == this._TopFolder.TcmId) ? tridionFolders.FindIndex(x => x.TcmId == this._TopFolder.TcmId) : 0;
        }

        private void SetChildRoles(ProjectFolderInfo folder)
        {
            if (folder == null)
                return;

            if (folder.ChildItems != null)
            {
                foreach (ProjectItemInfo child in folder.ChildItems)
                {
                    if (!child.IsFolder) continue;
                    
                    ProjectFolderInfo childFolder = (ProjectFolderInfo)child;
                    childFolder.ProjectFolderRole = folder.ProjectFolderRole;
                    SetChildRoles(childFolder);
                }
            }
        }

        private void SetChildProperties(ProjectFolderInfo folder)
        {
            if (folder == null)
                return;

            if (folder.Checked == null)
            {
                folder.Handled = true;
                folder.Checked = false;
                folder.Handled = false;
            }

            if ((folder.Checked == true || folder.Checked == false) && folder.ChildItems != null)
            {
                foreach (ProjectItemInfo child in folder.ChildItems)
                {
                    child.Handled = true;
                    child.Checked = folder.Checked;
                    child.Handled = false;
                }
            }
        }

        private void SetParentProperties(ProjectItemInfo item)
        {
            if (item == null)
                return;

            if (this._TopFolder == null)
                return;

            if (item.Path == this._TopFolder.Path)
                return;

            if (item.Parent == null)
                return;

            item.Parent.Handled = true;

            if (item.Parent.ChildItems != null && item.Parent.ChildItems.All(x => x.Checked == true))
            {
                item.Parent.Checked = true;
            }
            else if (item.Parent.ChildItems != null && item.Parent.ChildItems.All(x => x.Checked == false))
            {
                item.Parent.Checked = false;
            }
            else
            {
                item.Parent.Checked = null;
            }

            if (item.Parent.ChildItems != null && item.Parent.ChildItems.All(x => x.SyncTemplate == true))
            {
                item.Parent.SyncTemplate = true;
            }
            else if (item.Parent.ChildItems != null && item.Parent.ChildItems.All(x => x.SyncTemplate == false))
            {
                item.Parent.SyncTemplate = false;
            }
            else
            {
                item.Parent.SyncTemplate = null;
            }

            item.Parent.Handled = false;

            this.SetParentProperties(item.Parent);
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            this._ProjectItem = ((TreeViewItem)e.OriginalSource).DataContext as ProjectItemInfo;
            if (this._ProjectItem == null)
                return;

            ProjectFolderInfo selectedFolder = Service.GetSelectedFolderFromTree(this._AllTree);
            if (selectedFolder != null)
                this._TopFolder = selectedFolder;

            if (this._TopFolder != null)
                this.SetVisibility(this._TopFolder.ProjectFolderRole);
            
            this.SetForm();
        }

        private void TreeViewItem_Checked(object sender, RoutedEventArgs e)
        {
            this._ProjectItem = ((CheckBox)e.OriginalSource).DataContext as ProjectItemInfo;
            if (this._ProjectItem == null)
                return;

            if (this._ProjectItem.Handled)
                return;

            ProjectFolderInfo selectedFolder = Service.GetSelectedFolderFromTree(this._AllTree);
            if (selectedFolder != null)
                this._TopFolder = selectedFolder;

            this.SetChildProperties(this._ProjectItem as ProjectFolderInfo);

            this.SetParentProperties(this._ProjectItem);
        }

        private void chkSyncTemplate_Checked(object sender, RoutedEventArgs e)
        {
            if (this._ProjectItem == null)
                return;

            this._ProjectItem.SyncTemplate = this.chkSyncTemplate.IsChecked;

            this.SetParentProperties(this._ProjectItem);

            this.txtTemplateTitle.IsEnabled = this.chkSyncTemplate.IsChecked == true;
        }

        private void txtTitle_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (this._ProjectItem == null)
                return;

            ProjectFileInfo file = this._ProjectItem as ProjectFileInfo;
            if (file == null)
                return;

            file.Title = String.IsNullOrEmpty(this.txtTitle.Text) ? null : this.txtTitle.Text.Trim();
        }

        private void txtTemplateTitle_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (this._ProjectItem == null)
                return;

            ProjectFileInfo file = this._ProjectItem as ProjectFileInfo;
            if (file == null)
                return;

            file.TemplateTitle = String.IsNullOrEmpty(this.txtTemplateTitle.Text) ? null : this.txtTemplateTitle.Text.Trim();
        }

        private void txtTemplateFormat_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (this._ProjectItem == null)
                return;

            ProjectFolderInfo folder = this._ProjectItem as ProjectFolderInfo;
            if (folder == null)
                return;

            folder.TemplateFormat = this.txtTemplateFormat.Text.PlainXml();
        }

        private void txtSchemaNames_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (this._ProjectItem == null)
                return;

            ProjectFileInfo file = this._ProjectItem as ProjectFileInfo;
            if (file == null)
                return;

            file.SchemaNames = new List<string>(this.txtSchemaNames.Text.Split(';', ',', '\n', '\r'));
        }

        private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbRoles.SelectedValue == null)
                return;

            ProjectFolderRole role = (ProjectFolderRole) this.cbRoles.SelectedValue;
            if (this._ProjectItem.IsFolder)
            {
                ProjectFolderInfo folder = (ProjectFolderInfo)this._ProjectItem;
                folder.ProjectFolderRole = role;
                this.SetChildProperties(folder);
            }

            this.SetVisibility(role);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this._AllTree = Service.GetFileTree(this._TopFolder, this.RootPath);
            this.CurrentProjectFolder = Service.GetSelectedFolderFromTree(this._AllTree);

            if (this.CurrentProjectFolder != null && this.lstTridionFolders.SelectedIndex >= 0)
            {
                this.CurrentProjectFolder.ProjectFolderRole = (ProjectFolderRole) this.cbRoles.SelectedValue;
                this.CurrentProjectFolder.TcmId = this.TridionFolders.Where(x => x.TridionRole == this._TridionRole).ToList()[this.lstTridionFolders.SelectedIndex].TcmId;

                if (this.CurrentProjectFolder.ChildItems != null && this.CurrentProjectFolder.ChildItems.All(x => x.Checked == true))
                {
                    this.CurrentProjectFolder.Checked = true;
                }
                else if (this.CurrentProjectFolder.ChildItems != null && this.CurrentProjectFolder.ChildItems.All(x => x.Checked == false))
                {
                    this.CurrentProjectFolder.Checked = false;
                }
                else
                {
                    this.CurrentProjectFolder.Checked = null;
                }

                if (this.CurrentProjectFolder.ChildItems != null && this.CurrentProjectFolder.ChildItems.All(x => x.SyncTemplate == true))
                {
                    this.CurrentProjectFolder.SyncTemplate = true;
                }
                else if (this.CurrentProjectFolder.ChildItems != null && this.CurrentProjectFolder.ChildItems.All(x => x.SyncTemplate == false))
                {
                    this.CurrentProjectFolder.SyncTemplate = false;
                }
                else
                {
                    this.CurrentProjectFolder.SyncTemplate = null;
                }

                this.SetChildRoles((ProjectFolderInfo)this._ProjectItem);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            ProjectFileInfo file = this._ProjectItem as ProjectFileInfo;
            if (file == null)
                return;

            SelectTridionDebugDialogWindow dialog = new SelectTridionDebugDialogWindow();
            dialog.TbbTcmId = file.TcmId;
            dialog.TestItemTcmId = file.TestItemTcmId;
            dialog.TestTemplateTcmId = file.TestTemplateTcmId;
            dialog.CurrentMapping = this.CurrentMapping;

            bool res = dialog.ShowDialog() == true;
            if (res)
            {
                file.TestItemTcmId = dialog.TestItemTcmId;
                file.TestTemplateTcmId = dialog.TestTemplateTcmId;

                this.btnDebug.Foreground = new SolidColorBrush(Colors.Green);
            }
        }
    }
}