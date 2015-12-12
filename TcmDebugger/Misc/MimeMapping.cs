using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace TcmDebugger.Misc
{
	public static class MimeMapping
	{
		private static readonly Func<String, String> _getMimeMappingMethod = null;

		static MimeMapping()
		{
			// Load hidden mime mapping class and method from System.Web
			Assembly assembly = Assembly.GetAssembly(typeof(HttpApplication));
			Type mimeMappingType = assembly.GetType("System.Web.MimeMapping");

			_getMimeMappingMethod = (Func<String, String>)Delegate.CreateDelegate(typeof(Func<String, String>), mimeMappingType.GetMethod("GetMimeMapping",
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));
		}

		public static String GetMimeMapping(String fileName)
		{
			return _getMimeMappingMethod(fileName);
		}
	}
}
