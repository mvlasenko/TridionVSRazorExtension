using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Web.Services.Protocols;
using TcmDebugger.Host;

namespace TcmDebugger.Service
{
	public abstract class CompoundTemplateServiceBase : IDisposable
	{
		private ServiceHost mServiceHost;
		private ConcurrentDictionary<String, RemoteDebugger> mDebugSessions;

		/// <summary>
		/// Gets the implemented contract for the <see cref="CompoundTemplateServiceBase" /> service host.
		/// </summary>
		/// <value>
		/// The implemented contract.
		/// </value>
		protected abstract Type ImplementedContract
		{
			get;
		}

		/// <summary>
		/// Gets the class for the <see cref="CompoundTemplateServiceBase" /> service host
		/// </summary>
		/// <value>
		/// The service class
		/// </value>
		protected abstract Type Service
		{
			get;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompoundTemplateServiceBase"/> class.
		/// </summary>
		public CompoundTemplateServiceBase()
		{
			mDebugSessions = new ConcurrentDictionary<String, RemoteDebugger>(StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Initializes the <see cref="T:System.ServiceModel.ServiceHost" /> 
		/// </summary>
		/// <param name="compoundTemplateWebService">Target CompoundTemplateWebService to proxy</param>
		protected void Initialize(SoapHttpClientProtocol compoundTemplateWebService)
		{
			Uri address = new Uri("http://localhost:9090/templating/compoundtemplatewebservice.asmx");

			ServiceHost mServiceHost = new ServiceHost(Service);

			BasicHttpBinding binding = new BasicHttpBinding()
			{
				HostNameComparisonMode = HostNameComparisonMode.Exact,
				OpenTimeout = TimeSpan.FromMinutes(5),
				CloseTimeout = TimeSpan.FromMinutes(5),
				SendTimeout = TimeSpan.FromMinutes(5),
				ReceiveTimeout = TimeSpan.FromMinutes(5),
				MaxReceivedMessageSize = 2097152 // 2 MB				 
			};

			binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
			binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

			mServiceHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
			mServiceHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CredentialsPassThrough(compoundTemplateWebService);

			mServiceHost.AddServiceEndpoint(ImplementedContract, binding, address);

			// Listen for requests
			mServiceHost.Open();
		}

		/// <summary>
		/// Instantiate a new remote <see cref="T:TcmDebugger.Host.Debugger" /> and return the unique session id.
		/// </summary>
		/// <returns>Remote <see cref="T:TcmDebugger.Host.Debugger" /> session id.</returns>
		protected Debugger StartNewDebugger()
		{
			RemoteDebugger remoteDebugger = new RemoteDebugger();
			mDebugSessions.TryAdd(remoteDebugger.SessionId, remoteDebugger);
			
			return remoteDebugger.Debugger;
		}

		protected Debugger GetDebugger(String sessionId)
		{
			RemoteDebugger remoteDebugger;

			if (mDebugSessions.TryGetValue(sessionId, out remoteDebugger))
				return remoteDebugger.Debugger;

			return null;
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (mServiceHost != null)
				{
					mServiceHost.Close();
					mServiceHost = null;
				}

				// Dispose any outstanding debug sessions
				foreach (RemoteDebugger remoteDebugger in mDebugSessions.Values)
				{
					remoteDebugger.Dispose();
				}
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
