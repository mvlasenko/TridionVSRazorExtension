using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Xml;
using TcmDebugger.Engines;
using TcmDebugger.Extensions;
using TcmDebugger.Misc;
using Tridion.ContentManager.Templating.CompoundTemplates.DomainModel.Proxy;

namespace TcmDebugger.Service.Tridion2011
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class CompoundTemplateService2011 : CompoundTemplateServiceBase<ICompoundTemplateService>, ICompoundTemplateService
	{
		private static CompoundTemplateWebService mCompoundService;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompoundTemplateStub"/> class.
		/// </summary>
		/// <param name="targetHost">SDL Tridion target host.</param>
		public CompoundTemplateService2011(): base()
		{
			// Initialize the CompoundTemplateWebService, we use this to retrieve ItemXml from Tridion CMS for DebugSession initialization
			mCompoundService = new CompoundTemplateWebService()
			{
				Url = DebuggerConfig.WebServiceUrl,
				UseDefaultCredentials = false,
				EnableDecompression = true,
				PreAuthenticate = true
			};

			Initialize(mCompoundService);
		}

		public LoginResult Login(bool initData)
		{
			return mCompoundService.Login(true);
		}

		public void Logout()
		{
			mCompoundService.Logout();
		}

		public XmlElement GetPublications()
		{
			return mCompoundService.GetPublications();
		}

		public XmlElement GetList(String locationId, GetListOptions listOptions)
		{
			return mCompoundService.GetList(locationId, listOptions);
		}

		public XmlElement CreateCompoundTemplate(String location, String title, int itemType)
		{
			return mCompoundService.CreateCompoundTemplate(location, title, itemType);
		}

		public XmlElement CreateCompoundTemplateWithContent(String location, String title, int itemType, String contentXml)
		{
			return mCompoundService.CreateCompoundTemplateWithContent(location, title, itemType, contentXml);
		}

		public XmlElement SaveTemplate(String itemId, String contentXml, bool doneEditing)
		{
			return mCompoundService.SaveTemplate(itemId, contentXml, doneEditing);
		}

		public XmlElement CheckInTemplate(String itemId)
		{
			return mCompoundService.CheckInTemplate(itemId);
		}

		public XmlElement UndoCheckOutTemplate(String itemId)
		{
			return mCompoundService.UndoCheckOutTemplate(itemId);
		}

		public XmlElement GetTemplateGroups(String publicationId)
		{
			return mCompoundService.GetTemplateGroups(publicationId);
		}

		public XmlElement ReadItem(String itemId, EnumOpenMode openMode, int readFilter)
		{
			return mCompoundService.ReadItem(itemId, openMode, readFilter);
		}

		public DebuggingStatusResult StartDebuggingWithItemUri(String compoundTemplateId, String compoundTemplateXml, String itemId, bool includeSystemLog, System.Diagnostics.TraceEventType logLevel)
		{
			try
			{
				DebugEngineServer debugger = StartNewDebugger();

				if (debugger != null)
					return debugger.StartDebuggingWithItemUri(compoundTemplateId, compoundTemplateXml, itemId, includeSystemLog, logLevel);
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}

			return null;

			//return mCompoundService.StartDebuggingWithItemUri(compoundTemplateId, compoundTemplateXml, itemId, includeSystemLog, TraceEventType.Information);
		}

		public DebuggingStatusResult StartDebuggingWithPackage(String compoundTemplateId, String compoundTemplateXml, String packageXml, bool includeSystemLog, System.Diagnostics.TraceEventType logLevel)
		{
			try
			{
				DebugEngineServer debugger = StartNewDebugger();

				if (debugger != null)
					return debugger.StartDebuggingWithPackage(compoundTemplateId, compoundTemplateXml, packageXml, includeSystemLog, logLevel);
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}

			return null;

			//return mCompoundService.StartDebuggingWithPackage(compoundTemplateId, compoundTemplateXml, packageXml, includeSystemLog, logLevel);
		}

		public DebuggingStatusResult GetDebuggingState(String debuggerSessionId, String lastLogMessageId)
		{
			try
			{
				DebugEngineServer debugger = GetDebugger(debuggerSessionId);

				if (debugger != null)
					return debugger.GetDebuggingState(debuggerSessionId, lastLogMessageId);
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}

			return null;

			//return mCompoundService.GetDebuggingState(debuggerSessionId, lastLogMessageId);
		}

		public DebuggingStatusResult StopDebugging(String debuggerSessionId, String lastLogMessage)
		{
			try
			{
				DebugEngineServer debugger = GetDebugger(debuggerSessionId);

				if (debugger != null)
					return debugger.StopDebugging(debuggerSessionId, lastLogMessage);
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}

			return null;

			//return mCompoundService.StopDebugging(debuggerSessionId, lastLogMessage);
		}

		public void ResetDebugging(String debuggerSessionId)
		{
			try
			{
				DebugEngineServer debugger = GetDebugger(debuggerSessionId);

				if (debugger != null)
					debugger.ResetDebugging(debuggerSessionId);
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}

			//mCompoundService.ResetDebugging(debuggerSessionId);
		}

		public PackageResponse GetPackage(String debuggerSessionId, String packageId)
		{
			try
			{
				DebugEngineServer debugger = GetDebugger(debuggerSessionId);

				if (debugger != null)
					return debugger.GetPackage(debuggerSessionId, packageId);
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}

			return null;

			//return mCompoundService.GetPackage(debuggerSessionId, packageId);
		}

		public PackageResponse GetPackageItem(String debuggerSessionId, String packageItemId)
		{
			try
			{
				DebugEngineServer debugger = GetDebugger(debuggerSessionId);

				if (debugger != null)
					return debugger.GetPackageItem(debuggerSessionId, packageItemId);
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}

			return null;

			//return mCompoundService.GetPackageItem(debuggerSessionId, packageItemId);
		}

		public void CreateDefaultTemplates(String locationUri)
		{
			mCompoundService.CreateDefaultTemplates(locationUri);
		}

		public String GetCMEWebRoot()
		{
			return mCompoundService.GetCMEWebRoot();
		}
	}
}
