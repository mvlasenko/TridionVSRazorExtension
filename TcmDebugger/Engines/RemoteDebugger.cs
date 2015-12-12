using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using TcmDebugger.Misc;

namespace TcmDebugger.Engines
{
	public class RemoteDebugger : IDisposable
	{
		private String mSessionId;
		private AppDomain mAppDomain;
		private DebugEngineServer mDebuggerHost;

		public String SessionId
		{
			get
			{
				return mSessionId;
			}
		}

		public DebugEngineServer Debugger
		{
			get
			{
				return mDebuggerHost;
			}
		}

		public RemoteDebugger()
		{
			// Create a new debugger session
			mSessionId = Guid.NewGuid().ToString();

			Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
			AppDomainSetup appDomainSetup = AppDomain.CurrentDomain.SetupInformation;

			mAppDomain = AppDomain.CreateDomain(String.Format("Debugger-{0}", mSessionId), evidence, appDomainSetup);

/*
			Type assemblyLoaderType = typeof(AssemblyLoader);
			AssemblyLoader loader = mAppDomain.CreateInstanceAndUnwrap(assemblyLoaderType.Assembly.GetName().Name, assemblyLoaderType.FullName) as AssemblyLoader;

			foreach (String assemblyPath in Directory.GetFiles(DebuggerConfig.ApplicationPath, "Tridion*.dll"))
			{
				loader.LoadAssembly(assemblyPath);
			}
*/
			Type debuggerHostType = typeof(DebugEngineServer);

			mDebuggerHost = mAppDomain.CreateInstanceAndUnwrap(
				debuggerHostType.Assembly.GetName().Name,
				debuggerHostType.FullName,
				true,
				BindingFlags.Default,
				null,
				new Object[] { mSessionId },
				null,
				null) as DebugEngineServer;			
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (mAppDomain != null)
				{
					AppDomain.Unload(mAppDomain);
					mAppDomain = null;
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
