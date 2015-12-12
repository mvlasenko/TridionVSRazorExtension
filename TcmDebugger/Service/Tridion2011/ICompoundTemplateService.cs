using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tridion.ContentManager.Templating.CompoundTemplates.DomainModel.Proxy;

namespace TcmDebugger.Service.Tridion2011
{
	[ServiceContract(Namespace = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService"), XmlSerializerFormat]
	public interface ICompoundTemplateService
	{
		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/Login")]
		LoginResult Login(bool initData);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/Logout")]
		void Logout();

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/GetPublications")]
		XmlElement GetPublications();

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/GetList")]
		XmlElement GetList(String locationId, GetListOptions listOptions);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/CreateCompoundTemplate")]
		XmlElement CreateCompoundTemplate(String location, String title, int itemType);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/CreateCompoundTemplateWithContent")]
		XmlElement CreateCompoundTemplateWithContent(String location, String title, int itemType, String contentXml);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/SaveTemplate")]
		XmlElement SaveTemplate(String itemId, String contentXml, bool doneEditing);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/CheckInTemplate")]
		XmlElement CheckInTemplate(String itemId);
		
		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/UndoCheckOutTemplate")]
		XmlElement UndoCheckOutTemplate(String itemId);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/GetTemplateGroups")]
		XmlElement GetTemplateGroups(String publicationId);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/ReadItem")]
		XmlElement ReadItem(String itemId, EnumOpenMode openMode, int readFilter);
		
		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/StartDebuggingWithItemUri")]
		DebuggingStatusResult StartDebuggingWithItemUri(String compoundTemplateId, String compoundTemplateXml, String itemId, bool includeSystemLog, System.Diagnostics.TraceEventType logLevel);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/StartDebuggingWithPackage")]
		DebuggingStatusResult StartDebuggingWithPackage(String compoundTemplateId, String compoundTemplateXml, String packageXml, bool includeSystemLog, System.Diagnostics.TraceEventType logLevel);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/GetDebuggingState")]
		DebuggingStatusResult GetDebuggingState(String debuggerSessionId, String lastLogMessageId);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/StopDebugging")]
		DebuggingStatusResult StopDebugging(String debuggerSessionId, String lastLogMessage);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/ResetDebugging")]
		void ResetDebugging(String debuggerSessionId);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/GetPackage")]
		PackageResponse GetPackage(String debuggerSessionId, String packageId);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/GetPackageItem")]
		PackageResponse GetPackageItem(String debuggerSessionId, String packageItemId);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/CreateDefaultTemplates")]
		void CreateDefaultTemplates(String locationUri);

		[OperationContract(Action = "http://www.tridion.com/ContentManager/5.3/TemplatingWebService/GetCMEWebRoot")]
		String GetCMEWebRoot();
	}
}
