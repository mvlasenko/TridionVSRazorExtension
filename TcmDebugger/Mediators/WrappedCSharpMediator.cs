#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Mediator Wrapper Helper
// ---------------------------------------------------------------------------------
//	Date Created	: November 16, 2013
//	Author			: Rob van Oostenrijk
// ---------------------------------------------------------------------------------
// 	Change History
//	Date Modified       : 
//	Changed By          : 
//	Change Description  : 
//
////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TcmDebugger.Extensions;
using TcmDebugger.Misc;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Assembly;
using Tridion.ContentManager.Templating.Configuration;

namespace TcmDebugger.Mediators
{
    /// <summary>
    /// <see cref="WrappedCSharpMediator" /> wraps around <see cref="T:Tridion.ContentManager.Templating.Assembly.CSharpSourceCodeMediator" /> in order
    /// to load template classes from local debug assemblies
    /// </summary>
	public class WrappedCSharpMediator : IMediator
	{
        private const String RUNTEMPLATE = "<%RunTemplate";
		private delegate void ProcessRunTemplateCalls(ref String templateCode);

		private CSharpSourceCodeMediator mOriginalMediator;
        private static readonly String mBasePath;
		private static ProcessRunTemplateCalls mProcessRunTemplateCalls;		
        private static TemplatingLogger mLogger;
        private static List<Assembly> mDebugAssemblies;

		/// <summary>
		/// Load the <see cref="T:System.Reflection.Assembly" /> specified by the filename
		/// </summary>
		/// <param name="fileName">Assembly filename</param>
		/// <returns>Loaded <see cref="T:System.Reflection.Assembly" /> or null</returns>
		/// <remarks>Additionally loads the assembly symbols (.pdb) if available</remarks>
		private static Assembly LoadAssembly(String fileName)
		{
			String file = Path.GetFileNameWithoutExtension(fileName);
			String filePath = Path.Combine(mBasePath, file + ".dll");
			String pdbPath = Path.Combine(mBasePath, file + ".pdb");

			// Verify if a full filename was given
			if (File.Exists(fileName))
			{
				filePath = fileName;
				pdbPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(fileName) + ".pdb");
			}

			if (File.Exists(filePath))
			{
				Byte[] assemblyContent = File.ReadAllBytes(filePath);

				if (File.Exists(pdbPath))
				{
					Byte[] symbolsContent = File.ReadAllBytes(pdbPath);

					mLogger.Info("Loading templating debug assembly {0}.", file + ".dll", file + ".pdb");
					return Assembly.Load(assemblyContent, symbolsContent);
				}
				else
				{
					mLogger.Info("Loading templating debug assembly {0}.", file + ".dll");
					return Assembly.Load(assemblyContent);
				}
			}

			return null;
		}

		/// <summary>
		/// Obtain the <see cref="I:Tridion.ContentManager.Templating.Assembly.ITemplate" /> for the requested className from the specified assemblies
		/// </summary>
		/// <param name="className">Name of the class to load</param>
		/// <returns><see cref="I:Tridion.ContentManager.Templating.Assembly.ITemplate" /> or null if not found</returns>
		private static ITemplate GetTemplate(IEnumerable<Assembly> assemblies, String className)
		{
			foreach (Assembly assembly in assemblies)
			{
				Type templateType = assembly.GetType(className, false, true);

				if (templateType != null)
				{
					// Verify the templateType implements <see cref="I:Tridion.ContentManager.Templating.Assembly.ITemplate" />
					if (templateType.GetInterface(typeof(ITemplate).FullName) != null)
					{
						ITemplate template = Activator.CreateInstance(templateType) as ITemplate;

						if (template != null)
							return template;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Allow for resolving of dependent assemblies in the path of configured debug assembly libraries
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="T:System.ResolveEventArgs"/> instance containing the event data.</param>
		/// <returns></returns>
		private static Assembly AssemblyResolve(Object sender, ResolveEventArgs args)
		{
			AssemblyName assemblyName = new AssemblyName(args.Name);

			foreach (DebugAssembly debugAssemblyConfig in DebuggerConfig.Instance.Debugging.DebugAssemblies)
			{
				String directory = Path.GetDirectoryName(debugAssemblyConfig.Name);

				if (Directory.Exists(directory))
				{
					String assemblyPath = Path.Combine(directory, assemblyName.Name + ".dll");
					return LoadAssembly(assemblyPath);
				}
			}

			return null;
		}

	    /// <summary>
        /// Initializes the <see cref="WrappedCSharpMediator"/> class.
        /// </summary>
		static WrappedCSharpMediator()
		{
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

            mBasePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			
			mProcessRunTemplateCalls = (ProcessRunTemplateCalls)Delegate.CreateDelegate(typeof(ProcessRunTemplateCalls),
											typeof(CSharpSourceCodeMediator).GetMethod("ProcessRunTemplateCalls",
											BindingFlags.Static | BindingFlags.NonPublic));

            mLogger = TemplatingLogger.GetLogger(typeof(WrappedCSharpMediator));

            mDebugAssemblies = new List<Assembly>();
            
            foreach (DebugAssembly debugAssemblyConfig in DebuggerConfig.Instance.Debugging.DebugAssemblies)
            {
                Assembly debugAssembly = LoadAssembly(debugAssemblyConfig.Name + ".dll");

				if (debugAssembly != null)
					mDebugAssemblies.Add(debugAssembly);
				else
					mLogger.Error("Failed to load configured debug assembly: \"{0}\".", debugAssemblyConfig.Name);
            }
		}

		/// <summary>
		/// Initializes the <see cref="WrappedCSharpMediator" /> by executing the static constructor
		/// </summary>
		public static void Initialize()
		{
			//
		}
        
        /// <summary>
		/// Initializes a new instance of the <see cref="WrappedCSharpMediator"/> class.
        /// </summary>
		public WrappedCSharpMediator()
		{
			mOriginalMediator = new CSharpSourceCodeMediator();
		}

        /// <summary>
        /// Configures the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
		public void Configure(MediatorElement configuration)
		{
			mOriginalMediator.Configure(configuration);
		}

        /// <summary>
        /// Wraps around the original RunTemplate in order to load debug classes when required
        /// </summary>
        /// <param name="engine"><see cref="T:Tridion.ContentManager.Templating.Engine" /></param>
        /// <param name="package"><see cref="T:Tridion.ContentManager.Templating.Package"/></param>
        /// <param name="templateUri"><see cref="T:Tridion.ContentManager.CommunicationManagement.Template"/></param>
        /// <param name="className">Class to execute</param>
        /// <remarks>
        /// The wrapper tries to load the specified class from available assemblies in the following order:
        /// 1) Assemblies configured as debug assemblies in the application configuration
        /// 2) Assemblies loaded in the application domain
        /// 3) Assembly with the same name as the template present in the application directory
        /// 4) Assembly stored in the Tridion database (default behavior of Tridion)
        /// </remarks>
		public static void RunTemplateWrapper(Engine engine, Package package, String templateUri, String className)
		{
            ITemplate template = null;

            // Try and obtain the requested template from the loaded debug assemblies
            template = GetTemplate(mDebugAssemblies, className);

            if (template != null)
            {
                mLogger.Info("Loaded {0} from debug assembly {1}.dll", className, template.GetType().Assembly.GetName().Name);
                template.Transform(engine, package);
                return;
            }

            // Try and obtain the requested template from the assemblies already loaded in the application domain
            template = GetTemplate(AppDomain.CurrentDomain.GetAssemblies(), className);

            if (template != null)
            {
                mLogger.Info("Loaded {0} from appdomain assembly {1}.dll", className, template.GetType().Assembly.GetName().Name);
                template.Transform(engine, package);
                return;
            }

            // Try and load the template specified assembly from local disk
            Template tridionTemplate = engine.GetObject(templateUri) as Template;

            if (tridionTemplate != null && tridionTemplate.TemplateType == "AssemblyTemplate" && tridionTemplate.BinaryContent != null)
            {
                Assembly assembly = LoadAssembly(tridionTemplate.BinaryContent.Filename);

                if (assembly != null)
                {
                    template = GetTemplate(new Assembly[] { assembly }, className);

                    if (template != null)
                    {
                        mLogger.Info("Loaded {0} from local assembly {1}.dll", className, template.GetType().Assembly.GetName().Name);
                        template.Transform(engine, package);
                        return;
                    }
                }
            }
                
            // Run the original Tridion defined RunTemplate functionality
            CSharpSourceCodeMediator.RunTemplate(engine, package, templateUri, className);
		}

        /// <summary>
        /// Executes a Tridion template transformation
        /// </summary>
        /// <param name="engine"><see cref="T:Tridion.ContentManager.Templating.Engine" /></param>
        /// <param name="template"><see cref="T:Tridion.ContentManager.CommunicationManagement.Template"/></param>
        /// <param name="package"><see cref="T:Tridion.ContentManager.Templating.Package"/></param>
		public void Transform(Engine engine, Template template, Package package)
		{
			String templateCode = template.Content;

            if (templateCode.Contains(RUNTEMPLATE))
            {
                // Call the original ProcessRunTemplateCalls to process any "RunTemplate" executions
				mProcessRunTemplateCalls(ref templateCode);

			    // Add our own assembly as an additional using reference for compilation
				templateCode = String.Format("<%@Import Namespace=\"{0}\"%>\n<%@Assembly Name=\"{1}\"%>\n",
											this.GetType().Namespace,
											this.GetType().Assembly.FullName) + templateCode;
                                
                // Wrap any RunTemplate() calls through our RunTemplateWrapper()			
                template.Content = templateCode.Replace("RunTemplate(", this.GetType().Name + ".RunTemplateWrapper(");
            }
			
            // Execute the original mediator
			mOriginalMediator.Transform(engine, template, package);
		}
	}
}
