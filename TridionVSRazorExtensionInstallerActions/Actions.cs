using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using Microsoft.Win32;

namespace SDL.TridionVSRazorExtension.Installer
{
    [RunInstaller(true)]
    public class Actions : System.Configuration.Install.Installer
    {
        public Actions()
        {
            this.AfterInstall += SetupVSRegistration;
            this.BeforeRollback += RemoveVSRegistration;
            this.BeforeUninstall += RemoveVSRegistration;
        }

        private void SetupVSRegistration(object sender, InstallEventArgs e)
        {
            string extensionPath = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);

            SetValue("Software\\Microsoft\\VisualStudio\\10.0\\AutomationOptions\\LookInFolders", extensionPath, "");
            SetValue("Software\\Microsoft\\VisualStudio\\11.0\\AutomationOptions\\LookInFolders", extensionPath, "");
            SetValue("Software\\Microsoft\\VisualStudio\\12.0\\AutomationOptions\\LookInFolders", extensionPath, "");
            SetValue("Software\\Microsoft\\VisualStudio\\13.0\\AutomationOptions\\LookInFolders", extensionPath, "");
        }

        private void RemoveVSRegistration(object sender, InstallEventArgs e)
        {
            string extensionPath = this.Context.Parameters["targetdir"];

            RemoveValue("Software\\Microsoft\\VisualStudio\\10.0\\AutomationOptions\\LookInFolders", extensionPath);
            RemoveValue("Software\\Microsoft\\VisualStudio\\11.0\\AutomationOptions\\LookInFolders", extensionPath);
            RemoveValue("Software\\Microsoft\\VisualStudio\\12.0\\AutomationOptions\\LookInFolders", extensionPath);
            RemoveValue("Software\\Microsoft\\VisualStudio\\13.0\\AutomationOptions\\LookInFolders", extensionPath);
        }

        private static void SetValue(string path, string key, string value)
        {
            RegistryKey hklm = Registry.CurrentUser;
            hklm = hklm.CreateSubKey(path);
            
            hklm.SetValue(key, value);
        }

        private static void RemoveValue(string path, string key)
        {
            RegistryKey hklm = Registry.CurrentUser;
            hklm = hklm.OpenSubKey(path);

            hklm.DeleteValue(key);
        }
    }
}