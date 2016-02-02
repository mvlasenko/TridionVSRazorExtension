using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SDL.TridionVSRazorExtension.Common.Configuration;
using SDL.TridionVSRazorExtension.Common.Misc;
using SDL.TridionVSRazorExtension.Misc;
using SDL.TridionVSRazorExtension.Tridion;

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
            List<ItemInfo> publications = MainService.GetPublications(this.CurrentMapping);

            //get folder from history
            if (string.IsNullOrEmpty(this.TridionFolder.TcmId))
            {
                string strTcmIdPath = Common.IsolatedStorage.Service.GetFromIsolatedStorage("LastUsedPath");
                if (!string.IsNullOrEmpty(strTcmIdPath))
                {
                    this.TridionFolder.TcmIdPath = strTcmIdPath.Split('|').ToList();
                    this.TridionFolder.TcmId = this.TridionFolder.TcmIdPath.First();
                }
            }

            //smart folder detect
            if (string.IsNullOrEmpty(this.TridionFolder.TcmId))
            {
                string itemId = GetFirstLayoutItem(publications);
                if (itemId != null)
                {
                    this.TridionFolder.TcmId = MainService.GetItemFolder(this.CurrentMapping, itemId);
                    this.TridionFolder.TcmIdPath = MainService.GetIdPath(this.CurrentMapping, this.TridionFolder.TcmId);
                }
            }

            //todo: this is slow in Web 8, make it faster
            this.treeTridionFolder.ItemsSource = publications.Expand(this.CurrentMapping, this.TridionSelectorMode, this.TridionFolder.TcmIdPath, this.TridionFolder.TcmId).MakeExpandable();

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

            MainService.OnItemExpanded(item, this.CurrentMapping, this.TridionSelectorMode);
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            ItemInfo item = ((TreeViewItem) e.OriginalSource).DataContext as ItemInfo;
            if (item == null)
                return;

            this.TridionFolder.TcmId = item.TcmId;

            List<ItemInfo> list = new List<ItemInfo>();
            MainService.AddPathItem(list, item);
            
            this.TridionFolder.TcmIdPath = list.Select(x => x.TcmId).ToList();
            list.Reverse();
            this.TridionFolder.NamedPath = string.Join("/", list.Select(x => x.Title));

            this.txtPath.Text = this.TridionFolder.NamedPath.CutPath("/", 74, true);

            Common.IsolatedStorage.Service.SaveToIsolatedStorage("LastUsedPath", string.Join("|", this.TridionFolder.TcmIdPath));
        }

        private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbRoles.SelectedValue == null)
                return;

            this.TridionFolder.TridionRole = (TridionRole)this.cbRoles.SelectedValue;
            this.TridionSelectorMode = TridionSelectorMode.Folder;
            this.treeTridionFolder.ItemsSource = MainService.GetPublications(this.CurrentMapping).Expand(this.CurrentMapping, this.TridionSelectorMode, this.TridionFolder.TcmIdPath, this.TridionFolder.TcmId).MakeExpandable();
        }

        private void chkScanForItems_OnClick(object sender, RoutedEventArgs e)
        {
            this.TridionFolder.ScanForItems = this.chkScanForItems.IsChecked == true;
        }

        private string GetFirstLayoutItem(List<ItemInfo> publications)
        {
            foreach (ItemInfo publication in publications)
            {
                foreach (ItemInfo folder in MainService.GetFoldersByPublication(this.CurrentMapping, publication.TcmId))
                {
                    List<ItemInfo> items = MainService.GetLayoutsByParentFolder(this.CurrentMapping, folder.TcmId);

                    if (items.Any())
                        return MainService.GetBluePrintTopTcmId(this.CurrentMapping, items.First().TcmId);
                }
            }

            return null;
        }

    }
}