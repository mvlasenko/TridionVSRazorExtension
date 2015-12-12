using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TcmDebugger.Misc
{
	public class AssemblyLoader : MarshalByRefObject
	{
		public Assembly LoadAssembly(String assemblyPath)
		{
			return Assembly.LoadFile(assemblyPath);
		}
	}
}
