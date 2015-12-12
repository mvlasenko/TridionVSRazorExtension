using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using TcmDebugger.Mediators;
using TcmDebugger.Misc;
using Tridion.Configuration;
using Tridion.ContentManager.Data.AdoNet.Sql.Configuration;
using Tridion.ContentManager.Data.Configuration;
using Tridion.ContentManager.Security.Configuration;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Configuration;

namespace TcmDebugger.Engines
{
	public abstract class Engine : MarshalByRefObject
	{
		public Engine()
		{
			DebuggerConfig.ApplyConfiguration();
		}

        /// <summary>
        /// Execute the template transformation with the given parameters
        /// </summary>
        /// <param name="itemUri">Tridion item URI.</param>
        /// <param name="templateUri">Tridion template URI.</param>
        /// <param name="publicationTargetUri">Tridion publication target URI.</param>
        /// <param name="logLevel">Log level.</param>
        /// <returns>Package "Output" item as <see cref="T:System.String" /></returns>
		public virtual String Execute(String itemUri, String templateUri, String publicationTargetUri = null)
		{
			return String.Empty;
		}

		/// <summary>
		/// Derived <see cref="Engine" /> classes will call this to signal for debugging attachement
		/// </summary>
		protected void DebuggerHook()
		{
			if (DebuggerConfig.Instance.Debugging.EnableBreakpoint)
			{
				// Forced breakpoint before template execution
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine("{0} [!] {1}", DateTime.Now.ToString("HH:mm:ss"), "Ready to execute template, please attach your debugger now.");
				Console.ForegroundColor = ConsoleColor.Gray;

			    if (System.Diagnostics.Debugger.IsAttached)
			    {
                    //disabling this makes possible debugging from Visual Studio "Run"
                    //System.Diagnostics.Debugger.Break();
                }

                else
			        Console.ReadLine();
			}			
		}
	}
}
