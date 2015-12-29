using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using SDL.TridionVSRazorExtension.Common.Misc;

namespace SDL.TridionVSRazorExtension.Command
{
    internal sealed class FolderContextCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0140;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("375f73ad-0a47-4241-b7ba-e505a41a80c3");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderContextCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private FolderContextCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this._package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static FolderContextCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this._package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new FolderContextCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            TridionVSRazorExtensionPackage package = ((TridionVSRazorExtensionPackage)this.ServiceProvider);
            package.InitApplication();

            DTE applicationObject = package.ApplicationObject;
            Solution solution = package.Solution;
            Project project = package.Project;
            
            if (solution != null && project != null && applicationObject.SelectedItems != null)
            {
                foreach (ProjectItem folder in applicationObject.SelectedItems.Cast<SelectedItem>().Select(item => item.ProjectItem))
                {
                    if (folder.ProjectItems.Cast<ProjectItem>().Any(file => file.Name.EndsWith(".cshtml") || file.Name.IsAllowedMimeType()))
                        continue;

                    if (folder.FileNames[0].Contains("Views\\Shared"))
                        continue;

                    MessageBox.Show("Folder '" + folder.Name + "' is not supported.", "Wrong Operation", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var folders = applicationObject.SelectedItems.Cast<SelectedItem>().Select(item => item.ProjectItem.FileNames[0].Trim('\\'));
                MainService.ProcessFolders(folders.ToArray(), project);
            }
        }
    }
}
