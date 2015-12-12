#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Template Rendering Engine
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
using System.IO;
using TcmDebugger.Misc;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.Publishing;
using Tridion.ContentManager.Publishing.Rendering;
using Tridion.ContentManager.Publishing.Resolving;

namespace TcmDebugger.Engines
{
    /// <summary>
	/// <see cref="RenderEngine" /> wraps around the Tridion <see cref="T:Tridion.ContentManager.Templating.TemplateRenderer" />
    /// </summary>
	public class RenderEngine : Engine
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="RenderEngine"/> class.
		/// </summary>
		public RenderEngine(): base()
		{
		}

        /// <summary>
        /// Execute the template transformation with the given parameters
        /// </summary>
        /// <param name="itemUri">Tridion item URI.</param>
        /// <param name="templateUri">Tridion template URI.</param>
        /// <param name="publicationTargetUri">Tridion publication target URI.</param>
        /// <returns>Package "Output" item as <see cref="T:System.String" /></returns>
        /// <exception cref="System.ArgumentNullException">
        /// itemUri cannot be null or empty.
        /// or
        /// templateUri cannot be null or empty for itemtype
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// itemUri is not a valid Tridion URI.
        /// or
        /// templateUri is not a valid Tridion URI.
        /// </exception>
        public override String Execute(String itemUri, String templateUri = null, String publicationTargetUri = null)
        {
            Session session = null;

            try
            {
                session = DebuggerConfig.Instance.Templating.EnableImpersonation && !String.IsNullOrEmpty(DebuggerConfig.Instance.Templating.ImpersonationIdentity) ? 
                        new Session(DebuggerConfig.Instance.Templating.ImpersonationIdentity) : new Session();

                IdentifiableObject item = session.GetObject(itemUri);

                Template template = null;

                if (String.IsNullOrEmpty(templateUri))
                {
                    Page page = item as Page;

                    if (page != null)
                        template = page.PageTemplate;
                }
                else                                
                    template = session.GetObject(templateUri) as Template;

                if (template != null)
                {
                    ResolvedItem resolvedItem = new ResolvedItem(item, template);

                    PublishInstruction instruction = new PublishInstruction(session)
                    {
                        RenderInstruction = new RenderInstruction(session)
                        {
                            RenderMode = RenderMode.PreviewDynamic
                        },
                        ResolveInstruction = new ResolveInstruction(session)
                        {
                            IncludeChildPublications = false,
                            IncludeComponentLinks = true,
                            IncludeWorkflow = false,
                            OnlyPublishedItems = false,
                            Purpose = ResolvePurpose.Publish,
                            StructureResolveOption = StructureResolveOption.ItemsAndStructure
                        }
                    };

					DebuggerHook();

                    RenderedItem renderedItem = global::Tridion.ContentManager.Publishing.Rendering.RenderEngine.Render(resolvedItem, 
                        instruction,
                        !String.IsNullOrEmpty(publicationTargetUri) ? new PublicationTarget(new TcmUri(publicationTargetUri), session) : null);

                    using (StreamReader sr = new StreamReader(renderedItem.Content))
                    {
                        renderedItem.Content.Seek(0, SeekOrigin.Begin);
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(TraceEventType.Error, "Exception while executing TemplateEngine for item {0}, template {1}: {2}", itemUri, templateUri, ex.Message);
            }
            finally
            {
                if (session != null)
                    session.Dispose();
            }

            return String.Empty;
        }
    }
}
