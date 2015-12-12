using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using SDL.TridionVSRazorExtension.Command;

namespace SDL.TridionVSRazorExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(TridionVSRazorExtensionPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad("b7b232e7-2d51-4912-adac-57acb1c390b9")]
    public sealed class TridionVSRazorExtensionPackage : Package
    {
        /// <summary>
        /// TridionVSRazorExtensionPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "b7b232e7-2d51-4912-adac-57acb1c390b9";

        private DTE _applicationObject;
        private Solution _solution;
        private Project _project;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingWindowCommand"/> class.
        /// </summary>
        public TridionVSRazorExtensionPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            MappingWindowCommand.Initialize(this);
            ItemContextCommand.Initialize(this);
            FolderContextCommand.Initialize(this);
            ItemContextDeleteCommand.Initialize(this);
            ItemContextDebugCommand.Initialize(this);
        }

        public void InitApplication()
        {
            if (_applicationObject == null)
            {
                _applicationObject = this.GetService(typeof(DTE)) as DTE;
            }
            if (_solution == null && _applicationObject != null)
            {
                _solution = _applicationObject.Solution;
            }
            if (_project == null && _solution != null && _solution.Projects != null && _solution.Projects.Count > 0)
            {
                _project = VisualStudio.Service.GetProject(_applicationObject, _solution);
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

            MainService.Project = _project;
        }

        public DTE ApplicationObject
        {
            get
            {
                return _applicationObject;
            }
        }

        public Solution Solution
        {
            get
            {
                return _solution;
            }
        }

        public Project Project
        {
            get
            {
                return _project;
            }
        }

        #endregion
    }
}
