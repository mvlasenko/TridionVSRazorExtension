using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Xml;
using Tridion.ContentManager.Templating.Debugging;
using TcmDebugger.Extensions;
using TcmDebugger.Misc;
using System.Reflection;
using System.Xml.Linq;
using TcmDebugger.Service;

namespace TcmDebugger.Engines
{
	/// <summary>
	/// <see cref="DebugEngineServer" /> is a hosting process for the Tridion templating debug session as a server
	/// </summary>
    public class DebugEngineServer : Engine
    {
		private static readonly XNamespace templateDebuggingNamespace = "http://www.tridion.com/ContentManager/5.3/TemplateDebugging";

		private String mSessionId;
		private DebugSession mDebugSession;

		private static String StripXmlDeclaration(String xml)
		{
			if (!String.IsNullOrEmpty(xml) && xml.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
				return xml.Substring(xml.IndexOf("?>", StringComparison.OrdinalIgnoreCase) + 2);

			return xml;
		}

		private static void WriteLog(String logMessages)
		{
			XElement logXml = XElement.Parse(logMessages);

			foreach (XElement logEntry in logXml.Descendants(templateDebuggingNamespace + "log"))
			{
				System.Diagnostics.TraceEventType logType;
				Enum.TryParse<System.Diagnostics.TraceEventType>(logEntry.Attribute("type").Value, true, out logType);

				Logger.Log(logType, logEntry.Value);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DebugEngineServer"/> class.
		/// </summary>
		/// <param name="sessionId">Debug session identifier.</param>
		public DebugEngineServer(String sessionId): base()
		{
			mSessionId = sessionId;
		}

		public DebuggingStatusResult StartDebuggingWithItemUri(String compoundTemplateId, String compoundTemplateXml, String itemId, bool includeSystemLog, System.Diagnostics.TraceEventType logLevel)
		{
			try
			{
				mDebugSession = new DebugSession(mSessionId,
					compoundTemplateId,
					compoundTemplateXml,
					itemId,
					null,
					true,
					"tcm:0-0-0",
					DebuggerConfig.Instance.Templating.EnableImpersonation && !String.IsNullOrEmpty(DebuggerConfig.Instance.Templating.ImpersonationIdentity) ?
						DebuggerConfig.Instance.Templating.ImpersonationIdentity : WindowsIdentity.GetCurrent().Name,
					includeSystemLog,
					logLevel);

				// Signal for any debuggers
				DebuggerHook();

				mDebugSession.SetPreviewLocations(
					DebuggerConfig.Instance.CMS.PreviewDirectory,
					PreviewServer.PreviewUrl);

				mDebugSession.Start();
				
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}

			String logMessages = mDebugSession.GetLogMessagesXml(-1, -1, -1);
			WriteLog(logMessages);

			return new DebuggingStatusResult()
			{
				SessionId = mSessionId,
				IsRunning = mDebugSession.IsRunning(),
				ExecutionStatus = StripXmlDeclaration(mDebugSession.GetExecutionStatusXml()),
				Log = StripXmlDeclaration(logMessages),
				Error = mDebugSession.GetLastException()
			};
		}

		public DebuggingStatusResult StartDebuggingWithPackage(String compoundTemplateId, String compoundTemplateXml, String packageXml, bool includeSystemLog, System.Diagnostics.TraceEventType logLevel)
		{
			try
			{
				mDebugSession = new DebugSession(mSessionId,
					compoundTemplateId,
					compoundTemplateXml,
					null,
					packageXml,
					true,
					"tcm:0-0-0",
					DebuggerConfig.Instance.Templating.EnableImpersonation && !String.IsNullOrEmpty(DebuggerConfig.Instance.Templating.ImpersonationIdentity) ?
						DebuggerConfig.Instance.Templating.ImpersonationIdentity : WindowsIdentity.GetCurrent().Name,
					DebuggerConfig.Instance.Logging.IncludeTridionClasses,
					logLevel);

				// Signal for any debuggers
				DebuggerHook();

				mDebugSession.SetPreviewLocations(
					DebuggerConfig.Instance.CMS.PreviewDirectory,
					PreviewServer.PreviewUrl);

				mDebugSession.Start();
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}

			String logMessages = mDebugSession.GetLogMessagesXml(-1, -1, -1);
			WriteLog(logMessages);

			return new DebuggingStatusResult()
			{
				SessionId = mSessionId,
				IsRunning = mDebugSession.IsRunning(),
				ExecutionStatus = StripXmlDeclaration(mDebugSession.GetExecutionStatusXml()),
				Log = StripXmlDeclaration(logMessages),
				Error = mDebugSession.GetLastException()
			};
		}

		public DebuggingStatusResult GetDebuggingState(String debuggerSessionId, String lastLogMessageId)
		{
			int logId = -1;
			
			if (!int.TryParse(lastLogMessageId, out logId))
				logId = -1;

			if (logId != -1)
				logId++;

			String logMessages = mDebugSession.GetLogMessagesXml(-1, logId, -1);
			WriteLog(logMessages);

			return new DebuggingStatusResult()
			{
				SessionId = mSessionId,
				IsRunning = mDebugSession.IsRunning(),
				ExecutionStatus = StripXmlDeclaration(mDebugSession.GetExecutionStatusXml()),
				Log = StripXmlDeclaration(logMessages),
				Error = mDebugSession.GetLastException()
			};
		}

		public DebuggingStatusResult StopDebugging(String debuggerSessionId, String lastLogMessage)
		{
			try
			{
				if (mDebugSession.IsRunning())
					mDebugSession.Stop();			
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}

			int logId = -1;

			if (!int.TryParse(lastLogMessage, out logId))
				logId = -1;

			if (logId != -1)
				logId++;

			String logMessages = mDebugSession.GetLogMessagesXml(-1, logId, -1);
			WriteLog(logMessages);

			return new DebuggingStatusResult()
			{
				SessionId = mSessionId,
				IsRunning = mDebugSession.IsRunning(),
				ExecutionStatus = StripXmlDeclaration(mDebugSession.GetExecutionStatusXml()),
				Log = StripXmlDeclaration(logMessages),
				Error = mDebugSession.GetLastException()
			};
	    }

		public void ResetDebugging(String debuggerSessionId)
		{
			//mDebugSession.
		}

		public PackageResponse GetPackage(String debuggerSessionId, String packageId)
		{
			try
			{
				return new PackageResponse()
				{
					Response = mDebugSession.GetPackageXml(packageId)
				};
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, ex.Message);
			}

			return null;
		}

		public PackageResponse GetPackageItem(String debuggerSessionId, String packageItemId)
		{
			try
			{
				return new PackageResponse()
				{
					Response = mDebugSession.GetPackageItemXml(packageItemId)
				};
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, ex.Message);
			}

			return null;
		}
    }
}
