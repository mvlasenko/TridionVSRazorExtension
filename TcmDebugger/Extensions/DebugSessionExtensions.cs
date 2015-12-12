#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Debug Session Extensions
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
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using TcmDebugger.Misc;
using Tridion.ContentManager.Templating.Debugging;

namespace TcmDebugger.Extensions
{
    public static class DebugSessionExtensions
    {
        private static readonly XNamespace templateDebuggingNamespace = "http://www.tridion.com/ContentManager/5.3/TemplateDebugging";

        public static int WriteLogEntries(this DebugSession debugSession, int lastLogMessageId = -1)
        {
            int startId = -1;

            if (lastLogMessageId != -1)
                startId = lastLogMessageId + 1;

            if (debugSession != null)
            {
                XElement logXml = XElement.Parse(debugSession.GetLogMessagesXml(-1, startId, -1));

                foreach (XElement logEntry in logXml.Descendants(templateDebuggingNamespace + "log"))
                {
                    startId = int.Parse(logEntry.Attribute("id").Value);

					TraceEventType logType;
					Enum.TryParse<TraceEventType>(logEntry.Attribute("type").Value, true, out logType);
					                    
                    Logger.Log(logType, logEntry.Value);
                }
            }

            return startId;
        }

        public static String GetPostPackageName(this DebugSession debugSession)
        {
            if (debugSession != null)
            {
                XElement statusXml = XElement.Parse(debugSession.GetExecutionStatusXml());

                XElement lastInvocationXml = statusXml.Descendants(templateDebuggingNamespace + "Invocation").LastOrDefault();

                if (lastInvocationXml != null)
                {
                    XAttribute postPackageAttribute = lastInvocationXml.Attribute("PostPackage");

                    if (postPackageAttribute != null)
                        return postPackageAttribute.Value;
                }
            }

            return String.Empty;
        }

        public static String GetPackageItemDataId(this DebugSession debugSession, String packageName, String itemName)
        {
            if (debugSession != null)
            {
                XElement packageXml = XElement.Parse(debugSession.GetPackageXml(packageName));

                XElement itemXml = packageXml.Descendants(templateDebuggingNamespace + "Item").FirstOrDefault(i => String.Equals(i.Attribute("Name").Value, itemName, StringComparison.OrdinalIgnoreCase));

                if (itemXml != null)
                {
                    return itemXml.Attribute("ItemDataId").Value;
                }
            }

            return String.Empty;
        }

        public static String GetPackageItemString(this DebugSession debugSession, String itemDataId)
        {
            if (debugSession != null)
            {
                XElement itemXml = XElement.Parse(debugSession.GetPackageItemXml(itemDataId));
                return HttpUtility.UrlDecode(itemXml.Value);
            }

            return String.Empty;
        }
    }
}
