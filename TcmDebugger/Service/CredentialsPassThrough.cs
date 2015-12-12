using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services.Protocols;

namespace TcmDebugger.Service
{
	/// <summary>
	/// <see cref="CredentialsPassThrough" /> captures the users credentials in order to pass them on to the SDL Tridion Compound Template webservice
	/// </summary>
	public class CredentialsPassThrough : UserNamePasswordValidator
	{
		private SoapHttpClientProtocol mCompoundTemplateWebService;

		/// <summary>
		/// Initializes a new instance of the <see cref="CredentialsPassThrough"/> class.
		/// </summary>
		/// <param name="compoundTemplateWebService">Target CompoundTemplateWebService to proxy for.</param>
		public CredentialsPassThrough(SoapHttpClientProtocol compoundTemplateWebService): base()
		{
			mCompoundTemplateWebService = compoundTemplateWebService;	
		}

		/// <summary>
		/// When overridden in a derived class, validates the specified username and password.
		/// </summary>
		/// <param name="userName">The username to validate.</param>
		/// <param name="password">The password to validate.</param>
		public override void Validate(String userName, String password)
		{
			mCompoundTemplateWebService.Credentials = new NetworkCredential(userName, password);
		}
	}
}
