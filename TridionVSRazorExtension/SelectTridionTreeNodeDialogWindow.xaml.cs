using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tridion.ContentManager.CoreService.Client;

namespace SDL.TridionVSRazorExtension
{
    public partial class SelectTridionTreeNodeDialogWindow
    {
        public SelectTridionTreeNodeDialogWindow()
        {
            InitializeComponent();
        }

        public MappingInfo CurrentMapping { get; set; }
        public TridionFolderInfo TridionFolder { get; set; }
        public TridionSelectorMode TridionSelectorMode { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.treeTridionFolder.ItemsSource = Functions.GetPublications(this.CurrentMapping).Expand(this.CurrentMapping, this.TridionSelectorMode, this.TridionFolder.TcmIdPath, this.TridionFolder.TcmId).MakeExpandable();

            this.TridionFolder.FillNamedPath(this.CurrentMapping);
            this.txtPath.Text = this.TridionFolder.NamedPath;

            this.cbRoles.ItemsSource = Enum.GetValues(typeof(TridionRole)).Cast<TridionRole>();
            this.cbRoles.SelectedValue = this.TridionFolder.TridionRole;

            this.chkScanForItems.IsChecked = this.TridionFolder.ScanForItems;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            ItemInfo item = ((TreeViewItem)e.OriginalSource).DataContext as ItemInfo;
            if (item == null)
                return;

            if (item.ChildItems != null && item.ChildItems.All(x => x.Title != "Loading..."))
                return;

            if (String.IsNullOrEmpty(item.TcmId))
                return;

            if (item.ItemType == ItemType.Publication)
            {
                if (this.TridionSelectorMode == TridionSelectorMode.Folder)
                {
                    item.ChildItems = Functions.GetFoldersByPublication(this.CurrentMapping, item.TcmId).MakeExpandable().SetParent(item);
                }
                if (this.TridionSelectorMode == TridionSelectorMode.StructureGroup)
                {
                    item.ChildItems = Functions.GetStructureGroupsByPublication(this.CurrentMapping, item.TcmId).MakeExpandable().SetParent(item);
                }
                if (this.TridionSelectorMode == TridionSelectorMode.FolderAndStructureGroup)
                {
                    item.ChildItems = Functions.GetFoldersAndStructureGroupsByPublication(this.CurrentMapping, item.TcmId).MakeExpandable().SetParent(item);
                }
            }
            if (item.ItemType == ItemType.Folder)
            {
                item.ChildItems = Functions.GetFoldersByParentFolder(this.CurrentMapping, item.TcmId).MakeExpandable().SetParent(item);
            }
            if (item.ItemType == ItemType.StructureGroup)
            {
                item.ChildItems = Functions.GetStructureGroupsByParentStructureGroup(this.CurrentMapping, item.TcmId).MakeExpandable().SetParent(item);
            }
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            ItemInfo item = ((TreeViewItem) e.OriginalSource).DataContext as ItemInfo;
            if (item == null)
                return;

            this.TridionFolder.TcmId = item.TcmId;

            List<ItemInfo> list = new List<ItemInfo>();
            this.AddPathItem(list, item);
            
            this.TridionFolder.TcmIdPath = list.Select(x => x.TcmId).ToList();
            list.Reverse();
            this.TridionFolder.NamedPath = string.Join("/", list.Select(x => x.Title));

            this.txtPath.Text = this.TridionFolder.NamedPath.CutPath("/", 74, true);
        }

        private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbRoles.SelectedValue == null)
                return;

            this.TridionFolder.TridionRole = (TridionRole)this.cbRoles.SelectedValue;
            this.TridionSelectorMode = TridionSelectorMode.Folder;
            this.treeTridionFolder.ItemsSource = Functions.GetPublications(this.CurrentMapping).Expand(this.CurrentMapping, this.TridionSelectorMode, this.TridionFolder.TcmIdPath, this.TridionFolder.TcmId).MakeExpandable();
        }

        private void chkScanForItems_OnClick(object sender, RoutedEventArgs e)
        {
            this.TridionFolder.ScanForItems = this.chkScanForItems.IsChecked == true;
        }

        private void AddPathItem(List<ItemInfo> list, ItemInfo item)
        {
            if (item == null)
                return;

            list.Add(item);

            if (item.Parent != null)
                this.AddPathItem(list, item.Parent);
        }
    }
}