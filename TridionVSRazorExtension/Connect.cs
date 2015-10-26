using System;
using System.IO;
using System.Linq;
using System.Windows;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace SDL.TridionVSRazorExtension
{
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        private DTE2 _applicationObject;
        private AddIn _addInInstance;
        private Solution2 _solution;
        private Project _project;

        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect()
        {
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;

            if (_solution == null)
            {
                _solution = _applicationObject.Solution as Solution2;
            }
            if (_project == null && _solution != null && _solution.Projects != null && _solution.Projects.Count > 0)
            {
                _project = Functions.GetProject(_applicationObject, _solution);
            }

            Functions.Project = _project;

            if (connectMode == ext_ConnectMode.ext_cm_UISetup || connectMode == ext_ConnectMode.ext_cm_Startup || connectMode == ext_ConnectMode.ext_cm_AfterStartup)
            {
                object[] contextGUIDS = { };
                Commands2 commands = (Commands2)_applicationObject.Commands;
                const string toolsMenuName = "Tools";

                CommandBar menuBarCommandBar = ((CommandBars)_applicationObject.CommandBars)["MenuBar"];

                CommandBar contextCommandBar = ((CommandBars)_applicationObject.CommandBars)["Item"];

                CommandBar contextFolderCommandBar = ((CommandBars)_applicationObject.CommandBars)["Folder"];

                //Find the Tools command bar on the MenuBar command bar:
                CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
                CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

                //This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
                //  just make sure you also update the QueryStatus/Exec method to include the new command names.
                try
                {
                    //Add a command to the Commands collection:
                    Command command = commands.AddNamedCommand2(_addInInstance, "TridionVSRazorExtension", "Tridion Razor Extension", "Opens dialog window", true, 548, ref contextGUIDS);

                    //Add a control for the command to the tools menu:
                    if((command != null) && (toolsPopup != null))
                    {
                        command.AddControl(toolsPopup.CommandBar);
                    }

                    //RAZOR item context menu
                    Command contextCommand = commands.AddNamedCommand2(_addInInstance, "TridionVSRazorExtensionContext", "Sync with Tridion", "Starts Tridion synchronization", true, 548, ref contextGUIDS);

                    if (contextCommand != null && contextCommandBar != null)
                    {
                        contextCommand.AddControl(contextCommandBar, 3);
                    }

                    //RAZOR folder context menu
                    Command contextFolderCommand = commands.AddNamedCommand2(_addInInstance, "TridionVSRazorExtensionFolderContext", "Sync with Tridion", "Starts Tridion synchronization", true, 548, ref contextGUIDS);

                    if (contextFolderCommand != null && contextFolderCommandBar != null)
                    {
                        contextFolderCommand.AddControl(contextFolderCommandBar, 2);
                    }

                    //RAZOR item context menu - delete file
                    Command contextCommandDelete = commands.AddNamedCommand2(_addInInstance, "TridionVSRazorExtensionContextDelete", "Delete VS and Tridion", "Deletes on both Visual Strudion and Tridion side", true, 1088, ref contextGUIDS);

                    if (contextCommandDelete != null && contextCommandBar != null)
                    {
                        contextCommandDelete.AddControl(contextCommandBar, 4);
                    }

                }
                catch(ArgumentException)
                {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can
                    //  safely ignore the exception.
                }

            }
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        /// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
        /// <param term='commandName'>The name of the command to determine state for.</param>
        /// <param term='neededText'>Text that is needed for the command.</param>
        /// <param term='status'>The state of the command in the user interface.</param>
        /// <param term='commandText'>Text requested by the neededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if (commandName == "SDL.TridionVSRazorExtension.Connect.TridionVSRazorExtension")
                {
                    status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                }
                if (commandName == "SDL.TridionVSRazorExtension.Connect.TridionVSRazorExtensionContext")
                {
                    if (_applicationObject.SelectedItems.Cast<SelectedItem>().All(item => item.Name.EndsWith(".cshtml") || Functions.IsAllowedMimeType(item.Name)))
                    {
                        status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    }
                }
                if (commandName == "SDL.TridionVSRazorExtension.Connect.TridionVSRazorExtensionFolderContext")
                {
                    var folders = _applicationObject.SelectedItems.Cast<SelectedItem>().Select(item => item.ProjectItem);
                    if (folders.Any(folder => folder.ProjectItems.Cast<ProjectItem>().Any(file => file.Name.EndsWith(".cshtml") || Functions.IsAllowedMimeType(file.Name))))
                    {
                        status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    }
                }
                if (commandName == "SDL.TridionVSRazorExtension.Connect.TridionVSRazorExtensionContextDelete")
                {
                    if (_applicationObject.SelectedItems.Cast<SelectedItem>().All(item => item.Name.EndsWith(".cshtml") || Functions.IsAllowedMimeType(item.Name)))
                    {
                        status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    }
                }

            }
        }

        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param term='commandName'>The name of the command to execute.</param>
        /// <param term='executeOption'>Describes how the command should be run.</param>
        /// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
        /// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
        /// <param term='handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {
            if (_solution == null)
            {
                _solution = _applicationObject.Solution as Solution2;
            }
            if (_project == null && _solution != null && _solution.Projects != null && _solution.Projects.Count > 0)
            {
                _project = Functions.GetProject(_applicationObject, _solution);
            }

            //check project is open
            if (_project == null)
            {
                MessageBox.Show("Project is not open");
                return;
            }

            //check valid project
            bool validproj = false;
            try
            {
                validproj = _project.Properties.Cast<Property>().Any(property => property.Name == "WebApplication.BrowseURL" && property.Value != null && property.Value.ToString() != String.Empty);
            }
            catch (Exception)
            {
            }
            if (!validproj)
            {
                MessageBox.Show("Project is not ASP.NET or ASP.NET MVC");
                return;
            }

            Functions.Project = _project;

            handled = false;
            if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {

                if(commandName == "SDL.TridionVSRazorExtension.Connect.TridionVSRazorExtension")
                {
                    if (_solution != null && _project != null)
                    {
                        MappingWindow window = new MappingWindow();
                        window.RootPath = Path.GetDirectoryName(_project.FileName);

                        handled = window.ShowDialog() == true;
                    }
                }

                if (commandName == "SDL.TridionVSRazorExtension.Connect.TridionVSRazorExtensionContext")
                {
                    if (_solution != null && _project != null && _applicationObject.SelectedItems != null)
                    {
                        foreach (SelectedItem item in _applicationObject.SelectedItems)
                        {
                            if (!item.Name.EndsWith(".cshtml") && !Functions.IsAllowedMimeType(item.Name))
                            {
                                MessageBox.Show("Item '" + item.ProjectItem.FileNames[0] + "' is not supported.", "Wrong Operation", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }
                        }

                        var files = _applicationObject.SelectedItems.Cast<SelectedItem>().Where(item => item.Name.EndsWith(".cshtml") || Functions.IsAllowedMimeType(item.Name)).Select(item => item.ProjectItem.FileNames[0]);
                        Functions.ProcessFiles(files.ToArray(), _project);
                    }
                }

                if (commandName == "SDL.TridionVSRazorExtension.Connect.TridionVSRazorExtensionFolderContext")
                {
                    if (_solution != null && _project != null && _applicationObject.SelectedItems != null)
                    {
                        foreach (ProjectItem folder in _applicationObject.SelectedItems.Cast<SelectedItem>().Select(item => item.ProjectItem))
                        {
                            bool folderValid = folder.ProjectItems.Cast<ProjectItem>().Any(file => file.Name.EndsWith(".cshtml") || Functions.IsAllowedMimeType(file.Name));
                            if (!folderValid)
                            {
                                MessageBox.Show("Folder '" + folder.Name + "' is not supported.", "Wrong Operation", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }
                        }

                        var folders = _applicationObject.SelectedItems.Cast<SelectedItem>().Select(item => item.ProjectItem.FileNames[0].Trim('\\'));
                        Functions.ProcessFolders(folders.ToArray(), _project);
                    }
                }

                if (commandName == "SDL.TridionVSRazorExtension.Connect.TridionVSRazorExtensionContextDelete")
                {
                    if (_solution != null && _project != null && _applicationObject.SelectedItems != null)
                    {
                        foreach (SelectedItem item in _applicationObject.SelectedItems)
                        {
                            if (!item.Name.EndsWith(".cshtml") && !Functions.IsAllowedMimeType(item.Name))
                            {
                                MessageBox.Show("Item '" + item.ProjectItem.FileNames[0] + "' is not supported.", "Wrong Operation", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }
                        }

                        var files = _applicationObject.SelectedItems.Cast<SelectedItem>().Where(item => item.Name.EndsWith(".cshtml") || Functions.IsAllowedMimeType(item.Name)).Select(item => item.ProjectItem.FileNames[0]);
                        Functions.DeleteFiles(files.ToArray(), _project);
                    }
                }

            }
        }
    }
}