#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Template Debugging Engine
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
using System.Security.Principal;
using System.Threading;
using System.Web;
using TcmDebugger.Extensions;
using TcmDebugger.Misc;
using TcmDebugger.Service;
using Tridion.Configuration;
using Tridion.ContentManager.Templating.CompoundTemplates.DomainModel.Proxy;
using Tridion.ContentManager.Templating.Configuration;
using Tridion.ContentManager.Templating.Debugging;

namespace TcmDebugger.Engines
{
    /// <summary>
    /// <see cref="DebugEngine" /> wraps around the Tridion Template debugging functionality and allows local debugging of compound templates.
    /// </summary>
    public class DebugEngine : Engine
    {
        /// <summary>
        /// Template Builder Compound Template Webservice
        /// </summary>
        private CompoundTemplateWebService mCompoundService;

        public DebugEngine(): base()
        {
            // Initialize the CompoundTemplateWebService, we use this to retrieve ItemXml from Tridion CMS for DebugSession initialization
            mCompoundService = new CompoundTemplateWebService()
            {
                Url = VirtualPathUtility.AppendTrailingSlash(DebuggerConfig.Instance.CMS.Url) + "templating/compoundtemplatewebservice.asmx",
                UseDefaultCredentials = true
            };
        }

        /// <summary>
        /// Execute the template transformation with the given parameters
        /// </summary>
        /// <param name="itemUri">Tridion item URI.</param>
        /// <param name="templateUri">Tridion template URI.</param>
        /// <param name="publicationTargetUri">Tridion publication target URI.</param>
        /// <param name="logLevel">Log level.</param>
        /// <returns>Package "Output" item as <see cref="T:System.String" /></returns>
        public override String Execute(String itemUri, String templateUri, String publicationTargetUri = null)
        {
            String sessionId = Guid.NewGuid().ToString();
            DebugSession debugSession = null;

            try
            {
                String templateXml = mCompoundService.GetCompoundTemplateXml(templateUri);
			
                debugSession = new DebugSession(sessionId,
                    templateUri,
                    templateXml,
                    itemUri,
                    null,
                    true,
                    publicationTargetUri,
                    DebuggerConfig.Instance.Templating.EnableImpersonation && !String.IsNullOrEmpty(DebuggerConfig.Instance.Templating.ImpersonationIdentity) ? 
                        DebuggerConfig.Instance.Templating.ImpersonationIdentity : WindowsIdentity.GetCurrent().Name,
                    DebuggerConfig.Instance.Logging.IncludeTridionClasses,
                    DebuggerConfig.Instance.Logging.Level);

                debugSession.SetPreviewLocations(
                    DebuggerConfig.Instance.CMS.PreviewDirectory,
					PreviewServer.PreviewUrl);

				// Signal for any debuggers
				DebuggerHook();

                debugSession.Start();

                int lastMessageId = -1;

                while (debugSession.IsRunning())
                {
                    Thread.Sleep(1000);

                    lastMessageId = debugSession.WriteLogEntries(lastMessageId);
                }
                
				String postPackage = debugSession.GetPostPackageName();
                String outputItemDataId = debugSession.GetPackageItemDataId(postPackage, "Output");

                return debugSession.GetPackageItemString(outputItemDataId);
            }
            catch (Exception ex)
            {
				Logger.Log(System.Diagnostics.TraceEventType.Error, "Exception while executing DebugEngine for item {0}, template {1}: {2}", itemUri, templateUri, LoggerExtensions.TraceException(ex));				
            }
            finally
            {
                if (debugSession != null)
                    debugSession.Stop();
            }

            return String.Empty;
        }
    }
}
