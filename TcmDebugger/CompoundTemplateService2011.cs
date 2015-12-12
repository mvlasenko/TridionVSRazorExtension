using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Xml;
using TcmDebugger.Host;
using Tridion.ContentManager.Templating.CompoundTemplates.DomainModel.Proxy;

namespace TcmDebugger.Service.Tridion2011
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class CompoundTemplateStub : CompoundTemplateServiceBase, ICompoundTemplateService
	{
		private static String mTargetHost;
		private static CompoundTemplateWebService mCompoundService;
		
		/// <summary>
		/// Gets the implemented contract for thie <see cref="CompoundTemplateServiceBase" /> service host.
		/// </summary>
		/// <value>
		/// The implemented contract.
		/// </value>
		protected override Type ImplementedContract
		{
			get 
			{ 
				return typeof(ICompoundTemplateService);
			}
		}

		protected override Type Service
		{
			get 
			{ 
				return typeof(CompoundTemplateStub);
			}
		}

		public CompoundTemplateStub()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompoundTemplateStub"/> class.
		/// </summary>
		/// <param name="targetHost">SDL Tridion target host.</param>
		public CompoundTemplateStub(String targetHost)
		{
			mTargetHost = targetHost;

			// Initialize the CompoundTemplateWebService, we use this to retrieve ItemXml from Tridion CMS for DebugSession initialization
			mCompoundService = new CompoundTemplateWebService()
			{
				Url = VirtualPathUtility.AppendTrailingSlash(mTargetHost) + "templating/compoundtemplatewebservice.asmx",
				UseDefaultCredentials = true
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
			Debugger debugger = StartNewDebugger();
			return debugger.StartDebuggingWithItemUri(compoundTemplateId, compoundTemplateXml, itemId, includeSystemLog, logLevel);

			//return mCompoundService.StartDebuggingWithItemUri(compoundTemplateId, compoundTemplateXml, itemId, includeSystemLog, logLevel);
		}

		public DebuggingStatusResult StartDebuggingWithPackage(String compoundTemplateId, String compoundTemplateXml, String packageXml, bool includeSystemLog, System.Diagnostics.TraceEventType logLevel)
		{
			Debugger debugger = StartNewDebugger();
			return debugger.StartDebuggingWithPackage(compoundTemplateId, compoundTemplateXml, packageXml, includeSystemLog, logLevel);

			//return mCompoundService.StartDebuggingWithPackage(compoundTemplateId, compoundTemplateXml, packageXml, includeSystemLog, logLevel);
		}

		public DebuggingStatusResult GetDebuggingState(String debuggerSessionId, String lastLogMessageId)
		{
			Debugger debugger = GetDebugger(debuggerSessionId);
			return debugger.GetDebuggingState(debuggerSessionId, lastLogMessageId);

			//return mCompoundService.GetDebuggingState(debuggerSessionId, lastLogMessageId);
		}

		public DebuggingStatusResult StopDebugging(String debuggerSessionId, String lastLogMessage)
		{
			Debugger debugger = GetDebugger(debuggerSessionId);
			return debugger.StopDebugging(debuggerSessionId, lastLogMessage);

			//return mCompoundService.StopDebugging(debuggerSessionId, lastLogMessage);
		}

		public void ResetDebugging(String debuggerSessionId)
		{
			Debugger debugger = GetDebugger(debuggerSessionId);
			debugger.ResetDebugging(debuggerSessionId);

			//mCompoundService.ResetDebugging(debuggerSessionId);
		}

		public XmlElement GetPackage(String debuggerSessionId, String packageId)
		{
			Debugger debugger = GetDebugger(debuggerSessionId);
			return debugger.GetPackage(debuggerSessionId, packageId);

			//return mCompoundService.GetPackage(debuggerSessionId, packageId);
		}

		public XmlElement GetPackageItem(String debuggerSessionId, String packageItemId)
		{
			Debugger debugger = GetDebugger(debuggerSessionId);
			return debugger.GetPackageItem(debuggerSessionId, packageItemId);

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
