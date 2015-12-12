#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Debugger Configuration
// ---------------------------------------------------------------------------------
//	Date Created	: November 16, 2013
//	Author			: Rob van Oostenrijk
// ---------------------------------------------------------------------------------
// 	Change History
//	Date Modified       : Dec 07, 2015
//	Changed By          : Mark Vlasenko
//	Change Description  : WrappedRazorMediator used as a default mediator
//
////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using TcmDebugger.Mediators;
using Tridion.ContentManager.Data.AdoNet.Sql.Configuration;
using Tridion.ContentManager.Data.Configuration;
using Tridion.ContentManager.Security.Configuration;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Configuration;
using TridionConfig = Tridion.Configuration;

namespace TcmDebugger.Misc
{
    public class DebuggerConfig : ConfigurationSection
    {
		private const String COMPOUND_SERVICE_PATH = "templating/compoundtemplatewebservice.asmx";

		private static DebuggerConfig mDebuggerConfig = null;
		
        public static DebuggerConfig Instance
        {
            get
            {
				if (mDebuggerConfig == null)
					mDebuggerConfig = ConfigurationManager.GetSection("TcmDebugger") as DebuggerConfig;

				return mDebuggerConfig;
            }
        }

		public static String ApplicationPath
		{
			get
			{
				return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			}
		}

		public static void ApplyConfiguration()
		{
			// Apply database configuration
			ConfigurationSection sqlDbConfiguration = TridionConfig.ConfigurationManager.TryGetSection(DbConfiguration.SectionName);

			// Use reflection because SDL 2011 and SDL 2013 have different configuration structures
			Action<String, Object> setProperty = (Action<String, Object>)Delegate.CreateDelegate(typeof(Action<String, Object>),
												sqlDbConfiguration,
												typeof(ConfigurationElement).GetProperty("Item", BindingFlags.Instance | BindingFlags.NonPublic, null, typeof(Object), new Type[] { typeof(String) }, new ParameterModifier[] { })
												.GetSetMethod(true));

			if (sqlDbConfiguration != null)
			{
				setProperty("name", DebuggerConfig.Instance.Database.Name);
				setProperty("server", DebuggerConfig.Instance.Database.Server);
				setProperty("username", DebuggerConfig.Instance.Database.Username);
				setProperty("password", DebuggerConfig.Instance.Database.Password);
			}

			// Configure TRIDION_CM_HOME for the SchemaCache to find the required XSD files
			String rendererFolder = Path.GetFullPath(Path.Combine(ApplicationPath, DebuggerConfig.Instance.Templating.SchemaCache));
			Environment.SetEnvironmentVariable("TRIDION_CM_HOME", rendererFolder);

			// Enable impersonation for the current windows user?
			if (DebuggerConfig.Instance.Templating.EnableImpersonation)
			{
				ContentManagerSecurityConfiguration section = TridionConfig.ConfigurationManager.TryGetSection(ContentManagerSecurityConfiguration.SectionName) as ContentManagerSecurityConfiguration;

				if (section != null)
				{
					section.ImpersonationUsers.Add(
						new ImpersonationUserSettings()
						{
							Name = WindowsIdentity.GetCurrent().Name,
							ImpersonationType = ImpersonationType.Windows
						});
				}
			}

			// Enable templating settings and load the WrappedMediator
			TemplatingSettings templatingSettings = TemplateUtilities.GetTemplatingSettings();

			if (templatingSettings != null)
			{
				templatingSettings.DebuggingElement.PdbLocation = ApplicationPath;

                //MediatorElement mediatorElement = templatingSettings.MediatorCollection
                //									.Cast<MediatorElement>()
                //									.FirstOrDefault(me => String.Equals(me.MatchMimeType, "text/x-tcm-csharpfragment"));

                //if (mediatorElement != null)
                //{
                //	// Ensure the wrapped mediator is initialized (this loads the configured debug libraries)
                //	WrappedCSharpMediator.Initialize();
                //	mediatorElement.ClassName = typeof(WrappedCSharpMediator).AssemblyQualifiedName;
                //}

                MediatorElement mediatorElement = templatingSettings.MediatorCollection
                                    .Cast<MediatorElement>()
                                    .FirstOrDefault(me => String.Equals(me.MatchMimeType, "text/x-tcm-cshtml"));

                if (mediatorElement != null)
                {
                    WrappedRazorMediator.Initialize();
                    mediatorElement.ClassName = typeof(WrappedRazorMediator).AssemblyQualifiedName;
                }

            }
        }

		[ConfigurationProperty("database", IsRequired = true, DefaultValue = null)]
		public DatabaseElement Database
		{
			get
			{
				return base["database"] as DatabaseElement;
			}
		}

		[ConfigurationProperty("logging", IsRequired = true, DefaultValue = null)]
		public LoggingElement Logging
		{
			get
			{
				return base["logging"] as LoggingElement;
			}
		}

		[ConfigurationProperty("templating", IsRequired = true, DefaultValue = null)]
		public TemplatingElement Templating
		{
			get
			{
				return base["templating"] as TemplatingElement;
			}
		}

		[ConfigurationProperty("cms", IsRequired = true, DefaultValue = null)]
		public CMSElement CMS
		{
			get
			{
				return base["cms"] as CMSElement;
			}
		}

		[ConfigurationProperty("debugging", IsRequired = true, DefaultValue = null)]
		public DebuggingElement Debugging
		{
			get
			{
				return base["debugging"] as DebuggingElement;
			}
		}

		public static String WebServiceUrl
		{
			get
			{
				return VirtualPathUtility.AppendTrailingSlash(DebuggerConfig.Instance.CMS.Url) + COMPOUND_SERVICE_PATH;
			}
		}

		public static String WebServiceLocalUrl
		{
			get
			{
				return String.Format("http://localhost:{0}/{1}", DebuggerConfig.Instance.CMS.LocalPort, COMPOUND_SERVICE_PATH);
			}
		}


    }

	public class DatabaseElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true)]
		public String Name
		{
			get
			{
				return base["name"] as String;
			}
		}

		[ConfigurationProperty("server", IsRequired = true)]
		public String Server
		{
			get
			{
				return base["server"] as String;
			}
		}


		[ConfigurationProperty("username", IsRequired = true)]
		public String Username
		{
			get
			{
				return base["username"] as String;
			}
		}

		[ConfigurationProperty("password", IsRequired = true)]
		public String Password
		{
			get
			{
				return base["password"] as String;
			}
		}
	}

	public class LoggingElement : ConfigurationElement
	{
		[ConfigurationProperty("includeTridionClasses", DefaultValue = false, IsRequired = false)]
		public Boolean IncludeTridionClasses
		{
			get
			{
				return (Boolean)base["includeTridionClasses"];
			}
		}

		[ConfigurationProperty("level", DefaultValue = TraceEventType.Information, IsRequired = false)]
		public TraceEventType Level
		{
			get
			{
				return (TraceEventType)base["level"];
			}
		}
	}

	public class TemplatingElement : ConfigurationElement
	{
		[ConfigurationProperty("schemaCache", IsRequired = true)]
		public String SchemaCache
		{
			get
			{
				return base["schemaCache"] as String;
			}
		}
		
		[ConfigurationProperty("enableImpersonation", DefaultValue = false, IsRequired = false)]
		public bool EnableImpersonation
		{
			get
			{
				return (bool)base["enableImpersonation"];
			}
		}

		[ConfigurationProperty("impersonationIdentity", DefaultValue = null, IsRequired = false)]
		public String ImpersonationIdentity
		{
			get
			{
				return base["impersonationIdentity"] as String;
			}
		}
	}

	public class CMSElement : ConfigurationElement
	{
		[ConfigurationProperty("url", IsRequired = true)]
		public String Url
		{
			get
			{
				return base["url"] as String;
			}
		}

		[ConfigurationProperty("localPort", IsRequired = false, DefaultValue = (short)9090)]
		public short LocalPort
		{
			get
			{
				return (short)base["localPort"];
			}
		}

		[ConfigurationProperty("previewDirectory", DefaultValue = "D:\temp", IsRequired = false)]
		public String PreviewDirectory
		{
			get
			{
				return base["previewDirectory"] as String;
			}
		}
	}

	public class DebuggingElement : ConfigurationElement
	{
		[ConfigurationProperty("enableBreakpoint", DefaultValue = false, IsRequired = false)]
		public bool EnableBreakpoint
		{
			get
			{
				return (bool)base["enableBreakpoint"];
			}
		}

		[ConfigurationProperty("", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(DebugAssemblyCollection), AddItemName = "debugAssembly")]
		public DebugAssemblyCollection DebugAssemblies
		{
			get
			{
				return base[""] as DebugAssemblyCollection;
			}
		}
	}

	public class DebugAssemblyCollection : ConfigurationElementCollection, IEnumerable<DebugAssembly>
	{
		private readonly List<DebugAssembly> elements;

		public DebugAssemblyCollection()
		{
			this.elements = new List<DebugAssembly>();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			DebugAssembly element = new DebugAssembly();
			this.elements.Add(element);

			return element;
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((DebugAssembly)element).Name;
		}

		public new IEnumerator<DebugAssembly> GetEnumerator()
		{
			return this.elements.GetEnumerator();
		}
	}

	public class DebugAssembly : ConfigurationElement
	{
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public String Name
		{
			get
			{
				return (String)this["name"];
			}
		}
	}
}
