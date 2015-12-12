using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Web.Services.Protocols;
using TcmDebugger.Engines;
using TcmDebugger.Extensions;
using TcmDebugger.Misc;

namespace TcmDebugger.Service
{
	public abstract class CompoundTemplateServiceBase<T> : IDisposable
	{
		private ServiceHost mServiceHost;
		private ConcurrentDictionary<String, RemoteDebugger> mDebugSessions;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompoundTemplateServiceBase"/> class.
		/// </summary>
		protected CompoundTemplateServiceBase()
		{
			mDebugSessions = new ConcurrentDictionary<String, RemoteDebugger>(StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Initializes the <see cref="T:System.ServiceModel.ServiceHost" /> 
		/// </summary>
		/// <param name="compoundTemplateWebService">Target CompoundTemplateWebService to proxy</param>
		protected void Initialize(SoapHttpClientProtocol compoundTemplateWebService)
		{
			try
			{
				Uri address = new Uri(DebuggerConfig.WebServiceLocalUrl);

				ServiceHost mServiceHost = new ServiceHost(this);

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

				mServiceHost.AddServiceEndpoint(typeof(T), binding, address);

				mServiceHost.Open();

				Logger.Log(System.Diagnostics.TraceEventType.Information, "Compound Templating Webservice listening on \"{0}\".", DebuggerConfig.WebServiceLocalUrl);
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}			
		}

		/// <summary>
		/// Instantiate a new remote <see cref="T:TcmDebugger.Host.Debugger" /> and return the unique session id.
		/// </summary>
		/// <returns>Remote <see cref="T:TcmDebugger.Host.Debugger" /> session id.</returns>
		protected DebugEngineServer StartNewDebugger()
		{
			RemoteDebugger remoteDebugger = new RemoteDebugger();
			mDebugSessions.TryAdd(remoteDebugger.SessionId, remoteDebugger);
			
			return remoteDebugger.Debugger;
		}

		protected DebugEngineServer GetDebugger(String sessionId)
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
