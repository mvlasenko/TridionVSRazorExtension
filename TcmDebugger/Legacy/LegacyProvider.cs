#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Legacy Interface for TcmDebugger.COM
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
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using TcmDebugger.Misc;
using Tridion.ContentManager.Interop.cm_sys;
using Tridion.ContentManager.Interop.TDSDefines;
using core = Tridion.ContentManager.CoreService.Client;

namespace TcmDebugger.Legacy
{
	public class LegacyProvider : ILegacyProvider
	{
		private static core.ICoreService mCoreServiceClient;

		static LegacyProvider()		
		{	
			WSHttpBinding wsHttpBinding = new WSHttpBinding()
			{
				Security = new WSHttpSecurity()
				{
					Mode = SecurityMode.Message,
					Message =
					{
						ClientCredentialType = MessageCredentialType.Windows
					}
				}
			};

			EndpointAddress endpoint = new EndpointAddress(VirtualPathUtility.AppendTrailingSlash(DebuggerConfig.Instance.CMS.Url) + "webservices/CoreService2011.svc/wsHttp");
			ChannelFactory<core.ICoreService> clientFactory = new ChannelFactory<core.ICoreService>(wsHttpBinding, endpoint)
			{
				Credentials =
				{
					Windows =
					{
						ClientCredential = CredentialCache.DefaultNetworkCredentials
					}
				}
			};

			// Initialize the CoreServiceClient
			mCoreServiceClient = clientFactory.CreateChannel();
		}

		private String ToXml(XElement element)
		{
			using (XmlReader reader = element.CreateReader())
			{
				reader.MoveToContent();
				return reader.ReadOuterXml();
			}
		}

		private core.ListBaseColumns ParseColumnFilter(ListColumnFilter columnFilter)
		{
			if ((columnFilter & ListColumnFilter.XMLListID) == ListColumnFilter.XMLListID)
				return core.ListBaseColumns.Id;

			if ((columnFilter & ListColumnFilter.XMLListIDAndTitle) == ListColumnFilter.XMLListIDAndTitle)
				return core.ListBaseColumns.IdAndTitle;

			if ((columnFilter & ListColumnFilter.XMLListDefault) == ListColumnFilter.XMLListDefault)
				return core.ListBaseColumns.Default;

			if ((columnFilter & ListColumnFilter.XMLListExtended) == ListColumnFilter.XMLListExtended)
				return core.ListBaseColumns.Extended;

			return core.ListBaseColumns.Default;
		}
		
		private static Dictionary<String, Object> ParseRowFilterXml(String rowFilterXml)
		{
			Dictionary<String, Object> result = new Dictionary<String, Object>();

			if (!String.IsNullOrEmpty(rowFilterXml))
			{
				XmlDocument document = new XmlDocument();
				document.LoadXml(rowFilterXml);

				foreach (XmlNode node in document.DocumentElement.ChildNodes)
				{
					String localName = node.LocalName;
					String[] values = node.InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

					if (!String.IsNullOrEmpty(localName))
					{
						if (values.Length > 1)
							result.Add(localName, values);
						else if (values.Length > 0 && !String.IsNullOrEmpty(values[0]))
						{
							if (String.Equals(localName, "ItemType"))
								result.Add(localName, values[0]);								
							else
							{
								int num;

								if (!int.TryParse(values[0], out num) || num != 0x3FFFF)
									result.Add(localName, values[0]);
							}
						}
					}
				}
			}

			return result;
		}

		private core.ItemType[] ToItemTypes(String values)
		{
			int value;

			if (int.TryParse(values, out value))
				return new core.ItemType[] { (core.ItemType)value };

			return new core.ItemType[] { };
		}
		
		public Boolean HasUsingItems(UserContext userContext, String URI, String rowFilter)
		{
			Dictionary<String, Object> filterValues = ParseRowFilterXml(rowFilter);

			core.UsingItemsFilterData filter = new core.UsingItemsFilterData()
			{
				BaseColumns = core.ListBaseColumns.Default,
				IncludeLocalCopies = String.Equals(filterValues["InclLocalCopies"] as String, "true", StringComparison.OrdinalIgnoreCase),
				ExcludeTaxonomyRelations = String.Equals(filterValues["ExcludeTaxonomyRelations"] as String, "true", StringComparison.OrdinalIgnoreCase),
				IncludedVersions = String.Equals(filterValues["OnlyLatestVersions"] as String, "true", StringComparison.OrdinalIgnoreCase) ? core.VersionCondition.OnlyLatestVersions : core.VersionCondition.AllVersions,
				ItemTypes = ToItemTypes(filterValues["ItemType"] as String)
			};

			XElement result = mCoreServiceClient.GetListXml(URI, filter);

			return result.Descendants().Any();
		}

		public String GetUsingItems(UserContext userContext, String URI, ListColumnFilter columnFilter, String rowFilter)
		{
			Dictionary<String, Object> filterValues = ParseRowFilterXml(rowFilter);

			core.UsingItemsFilterData filter = new core.UsingItemsFilterData()
			{
				BaseColumns = ParseColumnFilter(columnFilter),
				IncludeLocalCopies = String.Equals(filterValues["InclLocalCopies"] as String, "true", StringComparison.OrdinalIgnoreCase),
				ExcludeTaxonomyRelations = String.Equals(filterValues["ExcludeTaxonomyRelations"] as String, "true", StringComparison.OrdinalIgnoreCase),
				IncludedVersions = String.Equals(filterValues["OnlyLatestVersions"] as String, "true", StringComparison.OrdinalIgnoreCase) ? core.VersionCondition.OnlyLatestVersions : core.VersionCondition.AllVersions,
				ItemTypes = ToItemTypes(filterValues["ItemType"] as String)
			};

			XElement result = mCoreServiceClient.GetListXml(URI, filter);

			return ToXml(result);
		}

		public String GetUsedItems(UserContext userContext, String URI, ListColumnFilter columnFilter, String rowFilter)
		{
			Dictionary<String, Object> filterValues = ParseRowFilterXml(rowFilter);

			core.UsedItemsFilterData filter = new core.UsedItemsFilterData()
			{
				BaseColumns = ParseColumnFilter(columnFilter)
			};

			XElement result = mCoreServiceClient.GetListXml(URI, filter);

			return ToXml(result);
		}
	}
}
