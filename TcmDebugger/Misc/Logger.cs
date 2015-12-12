#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Logger
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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using TcmDebugger.Extensions;

namespace TcmDebugger.Misc
{
    public static class Logger
    {
		private const int CONSOLE_PREFIX_WIDTH = 13;
		private readonly static Object mLock = new Object();

		public static IEnumerable<String> WrapString(String input, int wrapLength)
		{
			return Regex.Split(input, @"(.{1," + wrapLength + @"})(?:\s|$)")
						.Where(x => x.Length > 0)
						.Select(x => x.Trim());
		}

        public static void Log(TraceEventType logType, String message, params Object[] args)
        {
			// Synchronize access to the console log
			lock (mLock)
			{
				// Do not log above the configured log level
				if (logType > DebuggerConfig.Instance.Logging.Level)
					return;

				String outputType;

				switch (logType)
				{
					case TraceEventType.Critical:
						Console.ForegroundColor = ConsoleColor.DarkRed;
						outputType = "!";
						break;
					case TraceEventType.Error:
						Console.ForegroundColor = ConsoleColor.DarkRed;
						outputType = "E";
						break;
					case TraceEventType.Information:
						Console.ForegroundColor = ConsoleColor.DarkGreen;
						outputType = "I";
						break;
					case TraceEventType.Verbose:
						Console.ForegroundColor = ConsoleColor.Gray;
						outputType = "V";
						break;
					default:
						Console.ForegroundColor = ConsoleColor.Gray;
						outputType = "?";
						break;
				}

				String formattedMessage = args.Length > 0 ? String.Format(message, args) : message;
				//formattedMessage = LoggerExtensions.Formatter.Replace(formattedMessage, "\r" + new String(' ', 16));

				Console.Write("{0} [{1}] ", DateTime.Now.ToString("HH:mm:ss"), outputType);

				bool initialLine = true;

				foreach (String output in WrapString(formattedMessage, Console.WindowWidth - CONSOLE_PREFIX_WIDTH))
				{
					if (!initialLine)
						Console.Write(new String(' ', CONSOLE_PREFIX_WIDTH));

					Console.WriteLine(output);

					initialLine = false;
				}

				// Reset the console color
				Console.ForegroundColor = ConsoleColor.Gray;
			}
        }
    }
}
