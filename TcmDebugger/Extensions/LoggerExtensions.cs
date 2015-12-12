#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Logger Extensions
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
using System.Text;
using System.Text.RegularExpressions;
using Tridion.ContentManager.Templating;

namespace TcmDebugger.Extensions
{
	/// <summary>
	/// Extension functions for <see cref="T:Tridion.ContentManager.Templating.TemplatingLogger" />
	/// </summary>
	public static class LoggerExtensions
	{
		private static Regex mRegEx;

		public static Regex Formatter
		{
			get
			{
				if (mRegEx == null)
				{
					mRegEx = new Regex("\r\n|\r|\n", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
				}

				return mRegEx;
			}
		}

		/// <summary>
		/// Output a log trace for the given <see cref="T:System.Exception" />
		/// </summary>
		/// <param name="ex"><see cref="T:System.Exception" /></param>
		/// <returns><see cref="T:System.Exception" /> logtrace</returns>
		public static string TraceException(Exception ex)
		{
			StringBuilder sbMessage = new StringBuilder();
			int depth = 1;

			if (ex != null)
			{
				if (!String.IsNullOrEmpty(ex.Source))
					sbMessage.AppendFormat("{0} ({1})\n", ex.GetType().FullName, ex.Source);

				if (!String.IsNullOrEmpty(ex.Message))
					sbMessage.AppendLine(ex.Message);

				if (!String.IsNullOrEmpty(ex.StackTrace))
					sbMessage.AppendLine(ex.StackTrace);

				while (ex.InnerException != null)
				{
					string indent = new string('\t', depth);

					ex = ex.InnerException;

					if (!String.IsNullOrEmpty(ex.Source))
					{
						sbMessage.Append(indent);
						sbMessage.AppendFormat("{0} ({1})\n", ex.GetType().FullName, ex.Source);
					}

					if (!String.IsNullOrEmpty(ex.Message))
					{
						sbMessage.Append(indent);
						sbMessage.AppendLine(ex.Message);
					}

					if (!String.IsNullOrEmpty(ex.StackTrace))
					{
						sbMessage.Append(indent);
						sbMessage.AppendLine(Formatter.Replace(ex.StackTrace, "\r" + indent));
					}

					depth++;
				}
			}

			return sbMessage.ToString();
		}

		/// <summary>
		/// Log an error message
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="ex">Associated exception to output</param>
		public static void Error(this TemplatingLogger logger, String message, Exception ex)
		{
			if (logger != null)
				logger.Error(String.Format("{0}\n{1}", message, TraceException(ex)));
		}

		/// <summary>
		/// Log an error message
		/// </summary>
		/// <param name="format">Message format string</param>
		/// <param name="args">Format string parameters</param>
		public static void Error(this TemplatingLogger logger, String format, params Object[] args)
		{
			if (logger != null)
				logger.Error(String.Format(format, args));
		}

		/// <summary>
		/// Log an error message
		/// </summary>
		/// <param name="format">Message format string</param>
		/// <param name="ex">Associated exception to output</param>
		/// <param name="args">Format string parameters</param>
		public static void Error(this TemplatingLogger logger, String format, Exception ex, params Object[] args)
		{
			if (logger != null)
				logger.Error(String.Format("{0}\n{1}", String.Format(format, args), TraceException(ex)));
		}

		/// <summary>
		/// Log a warning message
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="ex">Associated exception to output</param>
		public static void Warning(this TemplatingLogger logger, String message, Exception ex)
		{
			if (logger != null)
				logger.Warning(String.Format("{0}\n{1}", message, TraceException(ex)));
		}

		/// <summary>
		/// Log a warning message
		/// </summary>
		/// <param name="format">Message / Message format string</param>
		/// <param name="args">Format string parameters</param>
		public static void Warning(this TemplatingLogger logger, String format, params Object[] args)
		{
			if (logger != null)
				logger.Warning(String.Format(format, args));
		}

		/// <summary>
		/// Log an warning message
		/// </summary>
		/// <param name="format">Message format string</param>
		/// <param name="ex">Associated exception to output</param>
		/// <param name="args">Format string parameters</param>
		public static void Warning(this TemplatingLogger logger, String format, Exception ex, params Object[] args)
		{
			if (logger != null)
				logger.Warning(String.Format("{0}\n{1}", String.Format(format, args), TraceException(ex)));
		}

		/// <summary>
		/// Log an information message
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="ex">Associated exception to output</param>
		public static void Info(this TemplatingLogger logger, String message, Exception ex)
		{
			if (logger != null)
				logger.Info(String.Format("{0}\n{1}", message, TraceException(ex)));
		}

		/// <summary>
		/// Log an information message
		/// </summary>
		/// <param name="format">Message / Message format string</param>
		/// <param name="args">Format string parameters</param>
		public static void Info(this TemplatingLogger logger, String format, params Object[] args)
		{
			if (logger != null)
				logger.Info(String.Format(format, args));
		}

		/// <summary>
		/// Log an information message
		/// </summary>
		/// <param name="format">Message format string</param>
		/// <param name="ex">Associated exception to output</param>
		/// <param name="args">Format string parameters</param>
		public static void Info(this TemplatingLogger logger, String format, Exception ex, params Object[] args)
		{
			if (logger != null)
				logger.Info(String.Format("{0}\n{1}", String.Format(format, args), TraceException(ex)));
		}

		/// <summary>
		/// Log a debug message
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="ex">Associated exception to output</param>
		public static void Debug(this TemplatingLogger logger, String message, Exception ex)
		{
			if (logger != null)
				logger.Debug(String.Format("{0}\n{1}", message, TraceException(ex)));
		}

		/// <summary>
		/// Log a debug message
		/// </summary>
		/// <param name="format">Message format string</param>
		/// <param name="args">Format string parameters</param>
		public static void Debug(this TemplatingLogger logger, String format, params Object[] args)
		{
			if (logger != null)
				logger.Debug(String.Format(format, args));
		}

		/// <summary>
		/// Log a debug message
		/// </summary>
		/// <param name="format">Message format string</param>
		/// <param name="ex">Associated exception to output</param>
		/// <param name="args">Format string parameters</param>
		public static void Debug(this TemplatingLogger logger, String format, Exception ex, params Object[] args)
		{
			if (logger != null)
				logger.Debug(String.Format("{0}\n{1}", String.Format(format, args), TraceException(ex)));
		}
	}
}