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
using System.Runtime.InteropServices;

namespace TcmDebugger.Legacy
{
	/// <summary>
	/// <see cref="ILegacyInterface" /> provides a means to configure the <see cref="I:ILegacyProvider" />
	/// </summary>
	[ComImport, Guid("15ED8BD9-90CA-47B6-8825-46282E17F0A1")]
	public interface ILegacyInterface
	{
		/// <summary>
		/// Configures a <see cref="ILegacyProvider" /> to use to implement Tridion legacy functionality
		/// </summary>
		/// <param name="legacyProvider"><see cref="ILegacyProvider" /></param>
		[DispId(1)]
		void SetProvider([In, MarshalAs(UnmanagedType.Interface)] ILegacyProvider legacyProvider);
	}
}
