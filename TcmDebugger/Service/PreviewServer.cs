using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcmDebugger.Extensions;
using TcmDebugger.Misc;

namespace TcmDebugger.Service
{
	public class PreviewServer : IDisposable
	{
		private const String PREVIEW_PREFIX =  "/Preview/";
		private CancellationTokenSource mCancellationTokenSource;

		public static String PreviewUrl
		{
			get
			{
				return String.Format("http://localhost:{0}{1}", DebuggerConfig.Instance.CMS.LocalPort, PREVIEW_PREFIX);
			}
		}

		public PreviewServer()
		{
			// Ensure the preview directory is created
			Directory.CreateDirectory(DebuggerConfig.Instance.CMS.PreviewDirectory);

			mCancellationTokenSource = new CancellationTokenSource();

			Task.Factory.StartNew(() =>
			{
				Listen(mCancellationTokenSource.Token);

			}, TaskCreationOptions.LongRunning);
		}

		private void Listen(CancellationToken cancel)
		{
			try
			{
				HttpListener httpListener = new HttpListener();
				httpListener.Prefixes.Add(PreviewUrl);

				httpListener.Start();

				Logger.Log(System.Diagnostics.TraceEventType.Information, "Preview webserver listening on \"{0}\".", PreviewUrl);

				while (!cancel.IsCancellationRequested)
				{
					IAsyncResult result = httpListener.BeginGetContext(callback =>
					{
						HttpListener localHttpListener = callback.AsyncState as HttpListener;

						if (localHttpListener == null || !localHttpListener.IsListening)
							return;

						ProcessRequest(localHttpListener.EndGetContext(callback));
						// handle the request in httpContext, some requests can take some time to complete
					}, httpListener);

					while (result.IsCompleted == false)
					{
						if (cancel.IsCancellationRequested)
							break;

						Thread.Sleep(100);
					}
				}

				httpListener.Stop();
			}
			catch (Exception ex)
			{
				Logger.Log(System.Diagnostics.TraceEventType.Error, LoggerExtensions.TraceException(ex));
			}
		}

		private void ProcessRequest(HttpListenerContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			HttpListenerRequest request = context.Request;
			HttpListenerResponse response = context.Response;

			if (!request.Url.AbsolutePath.StartsWith(PREVIEW_PREFIX, StringComparison.OrdinalIgnoreCase))
			{
				response.ContentType = MediaTypeNames.Text.Plain;
				response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

				using (StreamWriter sw = new StreamWriter(response.OutputStream))
				{
					sw.WriteLine("Only urls starting with \"Preview\" are served.");
				}

				response.Close();
				return;
			}

			String physicalPath = request.Url.AbsolutePath.Substring(PREVIEW_PREFIX.Length).Replace('/', '\\');
			physicalPath = Path.Combine(Path.GetFullPath(DebuggerConfig.Instance.CMS.PreviewDirectory), physicalPath);

			if (!File.Exists(physicalPath))
			{
				response.ContentType = MediaTypeNames.Text.Plain;
				response.StatusCode = (int)HttpStatusCode.NotFound;

				using (StreamWriter sw = new StreamWriter(response.OutputStream))
				{
					sw.WriteLine("Requested file \"{0}\" was not found.", request.Url.AbsolutePath);
				}

				response.Close();
				return;				
			}

			using (FileStream fileStream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
			{
				response.StatusCode = (int)HttpStatusCode.OK;
				response.ContentLength64 = fileStream.Length;
				response.ContentType = MimeMapping.GetMimeMapping(Path.GetFileName(physicalPath));

				fileStream.CopyTo(response.OutputStream);
				response.Close();
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

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				mCancellationTokenSource.Cancel();
		}
	}
}
