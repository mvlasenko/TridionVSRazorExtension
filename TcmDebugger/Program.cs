#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: TcmDebugger
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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using CommandLine;
using TcmDebugger.Engines;
using TcmDebugger.Legacy;
using TcmDebugger.Misc;
using TcmDebugger.Service;
using TcmDebugger.Service.Tridion2011;
using Tridion.ContentManager;

//TcmDebugger -m Debugger -i tcm:4042-29578 -t tcm:5057-30730-32

namespace TcmDebugger
{
    class Program
    {
		private static void InitializeConsole()
		{
			Console.Title = AssemblyInfo.Title;
			Console.WindowWidth = 150;
			Console.BufferHeight = 8192;

			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(AssemblyInfo.Description);

			String version = String.Format("(Version: {0})", AssemblyInfo.VersionFull);

			Console.CursorLeft = Console.WindowWidth - version.Length;
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(version);
			Console.ForegroundColor = ConsoleColor.Gray;
		}

        private static void Main(string[] args)
        {
			// Default execution options
			Options options = new Options()
			{
				Mode = EngineMode.Debugger,
				ItemURI = null,
				TemplateURI = null,
				PublicationTargetURI = null
			};

			InitializeConsole();

			if (Parser.Default.ParseArguments(args, options))
			{
				LegacyInterface legacyInterface = null;

				try
				{
					using (new PreviewServer())
					{
						// Initialize TcmDebugger.COM
						legacyInterface = new LegacyInterface();
						legacyInterface.SetProvider(new LegacyProvider());

						switch (options.Mode)
						{
							case EngineMode.LocalServer:
								using (new CompoundTemplateService2011())
								{
									Logger.Log(System.Diagnostics.TraceEventType.Information, "Press any key to exit.");
									Console.ReadKey();
								}
								break;
							case EngineMode.Debugger:
							case EngineMode.Publisher:
								if (String.IsNullOrEmpty(options.ItemURI))
									throw new Exception("ItemUri is required for Debugger or Publisher engines");

								if (!TcmUri.IsValid(options.ItemURI))
									throw new Exception(String.Format("Invalid tcm uri {0}", options.ItemURI));

								TcmUri tcmItemUri = new TcmUri(options.ItemURI);

								if (tcmItemUri.ItemType != ItemType.Page || options.Mode != EngineMode.Publisher)
								{
									if (String.IsNullOrEmpty(options.TemplateURI))
										throw new Exception("Template tcm uri is required for non-page type objects or debug engine");

									if (!TcmUri.IsValid(options.TemplateURI))
										throw new Exception(String.Format("Invalid template tcm uri {0}", options.TemplateURI));
								}

								if (!String.IsNullOrEmpty(options.PublicationTargetURI) && !TcmUri.IsValid(options.PublicationTargetURI))
									throw new Exception(String.Format("Invalid publication target tcm uri {0}", options.TemplateURI));

								Engine engine = null;

								switch (options.Mode)
								{
									case EngineMode.Debugger:
										engine = new Engines.DebugEngine();
										break;
									case EngineMode.Publisher:
										engine = new RenderEngine();
										break;
								}

								String output = engine.Execute(options.ItemURI, options.TemplateURI, options.PublicationTargetURI);
								output += "";

								Console.WriteLine();
								Console.WriteLine("{0} [!] {1}", DateTime.Now.ToString("HH:mm:ss"), "Execution finished. Press any key to exit.");
								Console.ReadKey();
								break;
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Log(System.Diagnostics.TraceEventType.Error, ex.Message);
				}
				finally
				{
					if (legacyInterface != null)
						Marshal.ReleaseComObject(legacyInterface);
				}
			}
        }
    }
}
