using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using SDL.TridionVSRazorExtension.Common.Configuration;
using SDL.TridionVSRazorExtension.Tridion;

namespace SDL.TridionVSRazorExtension
{
    public partial class TridionDestinationDialogWindow
    {
        public TridionDestinationDialogWindow()
        {
            InitializeComponent();
        }

        public MappingInfo Mapping { private get; set; }
        public string FilterItemTcmId { private get; set; }
        public string PublicationTcmId { get; set; }
        public string LayoutTitle { get; set; }
        public string TemplateTitle { get; set; }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtLayoutTitle.Text = this.LayoutTitle.Replace(" Layout", "") + " Layout";

            if (string.IsNullOrEmpty(this.TemplateTitle))
            {
                this.txtTemplateTitle.IsEnabled = false;
            }
            else
            {
                this.txtTemplateTitle.IsEnabled = true;
                this.txtTemplateTitle.Text = this.TemplateTitle.Replace(" Layout", "");
            }

            this.LoadPublications();
        }

        private void LoadPublications()
        {
            List<ItemInfo> publications = String.IsNullOrEmpty(this.FilterItemTcmId) ? MainService.GetPublications(this.Mapping) : MainService.GetPublications(this.Mapping, this.FilterItemTcmId);
            this.cbPublication.ItemsSource = publications;
            this.cbPublication.DisplayMemberPath = "Title";

            this.cbPublication.SelectedItem = publications.FirstOrDefault(t => t.TcmId == this.PublicationTcmId);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.LayoutTitle = this.txtLayoutTitle.Text.Trim();
            this.TemplateTitle = this.txtTemplateTitle.Text.Trim();

            ItemInfo publication = this.cbPublication.SelectedItem as ItemInfo;
            if (publication != null)
                this.PublicationTcmId = publication.TcmId;
            
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