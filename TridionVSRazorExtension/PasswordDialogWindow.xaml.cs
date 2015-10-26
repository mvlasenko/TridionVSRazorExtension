using System;
using System.Windows;

namespace SDL.TridionVSRazorExtension
{
    public partial class PasswordDialogWindow
    {
        public PasswordDialogWindow()
        {
            InitializeComponent();
        }

        public MappingInfo Mapping { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtUsername.Text = this.Mapping.Username;
            this.txtPassword.Password = this.Mapping.Password;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Mapping.Username = this.txtUsername.Text;
            this.Mapping.Password = this.txtPassword.Password;

            if (String.IsNullOrEmpty(this.Mapping.Username) || String.IsNullOrEmpty(this.Mapping.Password))
                return;

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