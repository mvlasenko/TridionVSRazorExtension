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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TcmDebugger.Legacy
{
	/// <summary>
	/// <see cref="LegacyInterface" /> implements <see cref="I:ILegacyInterface" /> in order to configure the <see cref="ILegacyProvider" />
	/// </summary>
	[ComImport, Guid("C1C34A56-61D2-4BDB-A8AB-C273F5EAA165")]
	public class LegacyInterface : ILegacyInterface
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)]
		public virtual extern void SetProvider(ILegacyProvider legacyProvider);
	}
}
