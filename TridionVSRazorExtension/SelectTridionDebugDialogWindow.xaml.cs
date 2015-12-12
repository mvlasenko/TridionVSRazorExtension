using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SDL.TridionVSRazorExtension.Common.Configuration;
using SDL.TridionVSRazorExtension.Misc;
using SDL.TridionVSRazorExtension.Tridion;
using Tridion.ContentManager.CoreService.Client;

namespace SDL.TridionVSRazorExtension
{
    public partial class SelectTridionDebugDialogWindow
    {
        public SelectTridionDebugDialogWindow()
        {
            InitializeComponent();
        }

        public MappingInfo CurrentMapping { get; set; }
        public string TbbTcmId { private get; set; }
        public string TestItemTcmId { get; set; }
        public string TestTemplateTcmId { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //set related templates

            List<string> usingItems = MainService.GetUsingCurrentItems(this.CurrentMapping, this.TbbTcmId);
            List<ItemInfo> templates = new List<ItemInfo>();
            foreach (string currTcmId in usingItems)
            {
                IdentifiableObjectData dataItem = MainService.ReadItem(this.CurrentMapping, currTcmId);
                ItemInfo item = dataItem.ToItem();
                if (item != null && (item.ItemType == ItemType.ComponentTemplate || item.ItemType == ItemType.PageTemplate))
                {
                    item.WebDav = ((RepositoryLocalObjectData) dataItem).GetWebDav();
                }
                templates.Add(item);
            }

            if(string.IsNullOrEmpty(this.TestTemplateTcmId))
                this.TestTemplateTcmId = Common.IsolatedStorage.Service.GetFromIsolatedStorage(Common.IsolatedStorage.Service.GetId("DebugTemplate", this.TbbTcmId));

            this.lstTemplates.ItemsSource = templates;
            this.lstTemplates.DisplayMemberPath = "NamedPathCut";
            this.lstTemplates.SelectedIndex = templates.FindIndex(x => x.TcmId == this.TestTemplateTcmId);
            if (this.lstTemplates.SelectedIndex < 0) this.lstTemplates.SelectedIndex = 0;
        }

        private void lstTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemInfo template = this.lstTemplates.SelectedItem as ItemInfo;
            if (template == null)
                return;

            this.TestTemplateTcmId = template.TcmId;
            Common.IsolatedStorage.Service.SaveToIsolatedStorage(Common.IsolatedStorage.Service.GetId("DebugTemplate", this.TbbTcmId), this.TestTemplateTcmId);

            //set test components tree

            if (string.IsNullOrEmpty(this.TestItemTcmId))
                this.TestItemTcmId = Common.IsolatedStorage.Service.GetFromIsolatedStorage(Common.IsolatedStorage.Service.GetId("DebugItem", this.TbbTcmId, this.TestTemplateTcmId));

            string strTcmIdPath = Common.IsolatedStorage.Service.GetFromIsolatedStorage(Common.IsolatedStorage.Service.GetId("DebugItemPath", this.TbbTcmId, this.TestTemplateTcmId));
            List<string> tcmIdPath = string.IsNullOrEmpty(strTcmIdPath) ? null : strTcmIdPath.Split('|').ToList();
            if (tcmIdPath == null)
                tcmIdPath = MainService.GetIdPath(this.CurrentMapping, this.TestItemTcmId);

            ItemType templateType = MainService.GetItemType(this.TestTemplateTcmId);
            TridionSelectorMode treeMode = templateType == ItemType.PageTemplate ? TridionSelectorMode.StructureGroup | TridionSelectorMode.Page : TridionSelectorMode.Folder | TridionSelectorMode.Component;

            this.treeTridionItem.ItemsSource = MainService.GetPublications(this.CurrentMapping).Expand(this.CurrentMapping, treeMode, tcmIdPath, this.TestItemTcmId).MakeExpandable();
            this.treeTridionItem.IsEnabled = true;
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            ItemInfo item = ((TreeViewItem)e.OriginalSource).DataContext as ItemInfo;
            if (item == null)
                return;

            ItemType templateType = MainService.GetItemType(this.TestTemplateTcmId);
            TridionSelectorMode treeMode = templateType == ItemType.PageTemplate ? TridionSelectorMode.StructureGroup | TridionSelectorMode.Page : TridionSelectorMode.Folder | TridionSelectorMode.Component;

            MainService.OnItemExpanded(item, this.CurrentMapping, treeMode);
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            ItemInfo item = ((TreeViewItem) e.OriginalSource).DataContext as ItemInfo;
            if (item == null)
                return;

            //check if item is valid

            ItemType itemType = MainService.GetItemType(item.TcmId);
            ItemType templateType = MainService.GetItemType(this.TestTemplateTcmId);

            if (itemType == ItemType.Component && templateType == ItemType.ComponentTemplate)
            {
                ComponentData component = MainService.GetComponent(this.CurrentMapping, item.TcmId);
                ComponentTemplateData template = MainService.ReadItem(this.CurrentMapping, this.TestTemplateTcmId) as ComponentTemplateData;

                if (component == null || template == null)
                {
                    ((TreeViewItem) e.OriginalSource).IsEnabled = false;
                    MessageBox.Show("Selected component is invalid", "Test item", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }

                if (template.RelatedSchemas.All(x => MainService.GetId(x.IdRef) != MainService.GetId(component.Schema.IdRef)))
                {
                    ((TreeViewItem)e.OriginalSource).IsEnabled = false;
                    MessageBox.Show("Selected component is invalid", "Test item", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }
            }
            else if(itemType == ItemType.Page && templateType == ItemType.PageTemplate)
            {
                PageData page = MainService.ReadItem(this.CurrentMapping, item.TcmId) as PageData;
                PageTemplateData template = MainService.ReadItem(this.CurrentMapping, this.TestTemplateTcmId) as PageTemplateData;

                if (page == null || template == null)
                {
                    ((TreeViewItem)e.OriginalSource).IsEnabled = false;
                    MessageBox.Show("Selected page is invalid", "Test item", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }

                if (MainService.GetId(page.PageTemplate.IdRef) != MainService.GetId(template.Id))
                {
                    ((TreeViewItem)e.OriginalSource).IsEnabled = false;
                    MessageBox.Show("Selected page is invalid", "Test item", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }
            }
            else
            {
                return;
            }

            this.TestItemTcmId = item.TcmId;
            Common.IsolatedStorage.Service.SaveToIsolatedStorage(Common.IsolatedStorage.Service.GetId("DebugItem", this.TbbTcmId, this.TestTemplateTcmId), this.TestItemTcmId);

            List<ItemInfo> list = new List<ItemInfo>();
            MainService.AddPathItem(list, item);

            Common.IsolatedStorage.Service.SaveToIsolatedStorage(Common.IsolatedStorage.Service.GetId("DebugItemPath", this.TbbTcmId, this.TestTemplateTcmId), string.Join("|", list.Select(x => x.TcmId)));

            this.btnOk.IsEnabled = true;
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

    }
}