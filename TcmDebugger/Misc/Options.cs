#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Command Line Options
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
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;

namespace TcmDebugger.Misc
{
	/// <summary>
	/// <see cref="EngineMode" />  determines which engine to execute for template debugging
	/// </summary>
	public enum EngineMode
	{
		Debugger,
		Publisher,
		LocalServer
	}

	public class Options
	{
		[Option('m', "mode", Required = true, HelpText = "Rendering engine to use")]
		public EngineMode Mode
		{
			get;
			set;
		}

		[Option('i', "item", Required = false, HelpText = "Item URI to transform")]
		public String ItemURI
		{
			get;
			set;
		}

		[Option('t', "template", Required = false, HelpText = "Template URI for transformation")]
		public String TemplateURI
		{
			get;
			set;
		}

		[Option('p', "target", Required = false, HelpText = "Publication target URI")]
		public String PublicationTargetURI
		{
			get;
			set;
		}

		[HelpOption]
		public String GetUsage()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("Usage:");
			sb.AppendLine("<program> -i <item> -t <template>");
			sb.AppendFormat("e.g. {0} -i tcm:5-2312-64 -t tcm:5-2279-128\n", Path.GetFileName(Assembly.GetExecutingAssembly().Location));
			sb.AppendLine();
			sb.AppendLine("Parameters:");
			sb.AppendLine("-m\t--mode\t\tEngine mode: \"Debugger\", \"LocalServer\" or \"Publisher\"");
			sb.AppendLine("-i\t--item\t\tItem URI");
			sb.AppendLine("-t\t--template\tTemplate URI");
			sb.AppendLine("-p\t--target\tPublication target URI");

			return sb.ToString();
		}
	}
}
