#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Compound Template Webservice Extensions
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
using System.Xml;
using Tridion;
using Tridion.ContentManager;
using Tridion.ContentManager.Templating.CompoundTemplates.DomainModel.Proxy;

namespace TcmDebugger.Extensions
{
    /// <summary>
    /// Extension functions for <see cref="T:Tridion.ContentManager.Templating.CompoundTemplates.DomainModel.Proxy.CompoundTemplateWebService" />
    /// </summary>
    public static class CompoundTemplateWebServiceExtensions
    {
        private static readonly XmlNamespaceManager namespaceManager;

        /// <summary>
        /// Initializes the <see cref="CompoundTemplateWebServiceExtensions"/> class.
        /// </summary>
        static CompoundTemplateWebServiceExtensions()
        {
            // Initialize Tridion XmlNamespaceManager
            namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace(Constants.XlinkPrefix, Constants.XlinkNamespace);
            namespaceManager.AddNamespace(Constants.XsdPrefix, Constants.XsdNamespace);
            namespaceManager.AddNamespace(Constants.TcmPrefix, Constants.TcmNamespace);
        }

        /// <summary>
        /// Retrieves the Compound Template invocation XML for a given page or component template Uri
        /// </summary>
        /// <param name="compoundTemplateWebService"><see cref="T:Tridion.ContentManager.Templating.CompoundTemplates.DomainModel.Proxy.CompoundTemplateWebService" /></param>
        /// <param name="templateUri">Tridion template URI.</param>
        /// <returns>Compound Template invocation XML as <see cref="T:String.String" /></returns>
        /// <exception cref="System.NotSupportedException">Unsupported template type</exception>
        public static String GetCompoundTemplateXml(this CompoundTemplateWebService compoundTemplateWebService, String templateUri)
        {
            if (compoundTemplateWebService != null)
            {
                XmlElement templateXml = compoundTemplateWebService.ReadItem(templateUri, EnumOpenMode.OpenModeView, 1919);

                String templateType = templateXml.SelectSingleNode("/tcm:Data/tcm:Type", namespaceManager).InnerText;

                if (!String.Equals(templateType, "CompoundTemplate", StringComparison.OrdinalIgnoreCase))
                    throw new NotSupportedException("Unsupported template type: " + templateType);

                return templateXml.SelectSingleNode("/tcm:Data/tcm:Content/tcm:PublisherScript", namespaceManager).InnerText;
            }

            return String.Empty;
        }

		/// <summary>
		/// Retrieves the Compound Template invocation XML for a given page or component template Uri
		/// </summary>
		/// <param name="compoundTemplateWebService"><see cref="T:Tridion.ContentManager.Templating.CompoundTemplates.DomainModel.Proxy.CompoundTemplateWebService" /></param>
		/// <param name="templateUri">Tridion template URI.</param>
		/// <returns>Compound Template invocation XML as <see cref="T:String.String" /></returns>
		/// <exception cref="System.NotSupportedException">Unsupported template type</exception>
		public static String GetAssemblyKey(this CompoundTemplateWebService compoundTemplateWebService, String templateUri)
		{
			if (compoundTemplateWebService != null)
			{
				XmlElement templateXml = compoundTemplateWebService.ReadItem(templateUri, EnumOpenMode.OpenModeView, 1919);

				String templateType = templateXml.SelectSingleNode("/tcm:Data/tcm:Type", namespaceManager).InnerText;

				if (!String.Equals(templateType, "CompoundTemplate", StringComparison.OrdinalIgnoreCase))
					throw new NotSupportedException("Unsupported template type: " + templateType);

				String owningPublication = templateXml.SelectSingleNode("/tcm:Info/tcm:BluePrintInfo/tcm:OwningPublication/@xlink:href", namespaceManager).InnerText;
				String version = templateXml.SelectSingleNode("/tcm:Info/tcm:VersionInfo/tcm:Version", namespaceManager).InnerText;
				String revision = templateXml.SelectSingleNode("/tcm:Info/tcm:VersionInfo/tcm:Revision", namespaceManager).InnerText;
				String content = templateXml.SelectSingleNode("/tcm:Data/tcm:Content/tcm:PublisherScript", namespaceManager).InnerText;

				content = content.Replace("\n", "\r\n");

				TcmUri tcmPublication = new TcmUri(owningPublication);
				TcmUri tcmTemplate = new TcmUri(templateUri);

				TcmUri tcmParent = new TcmUri(tcmTemplate.ItemId, tcmTemplate.ItemType, tcmPublication.ItemId);

				int hashCode = 0;

				if (!String.IsNullOrEmpty(content))
					hashCode = content.GetHashCode();

				return String.Format("{0}/{1}.{2}/{3:X}", tcmParent, version, revision, hashCode);
			}

			return String.Empty;
		}
    }
}
