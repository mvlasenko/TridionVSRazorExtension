#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Legacy Provider Interface for TcmDebugger.COM
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
using TcmDebugger.COM.cm_defines;
using TcmDebugger.COM.cm_sys;

namespace TcmDebugger.Legacy
{
	/// <summary>
	/// <see cref="ILegacyProvider" /> provides the means to implement Tridion legacy functionality
	/// </summary>
	/// <param name="legacyProvider"><see cref="ILegacyProvider" /></param>
	[ComImport, Guid("44C1A000-AD24-46C9-B870-16D797E770DE")]
	public interface ILegacyProvider
	{
		[DispId(1)]
		Boolean HasUsingItems([In, MarshalAs(UnmanagedType.Interface)] UserContext userContext,
							  [In, MarshalAs(UnmanagedType.BStr)] String URI,
							  [In, MarshalAs(UnmanagedType.BStr)] String rowFilter);

		[return: MarshalAs(UnmanagedType.BStr)]
		[DispId(2)]
		String GetUsingItems([In, MarshalAs(UnmanagedType.Interface)] UserContext userContext,
							 [In, MarshalAs(UnmanagedType.BStr)] String URI,
							 [In] ListColumnFilter columnFilter,
							 [In, MarshalAs(UnmanagedType.BStr)] String rowFilter);

		[return: MarshalAs(UnmanagedType.BStr)]
		[DispId(3)]
		String GetUsedItems([In, MarshalAs(UnmanagedType.Interface)] UserContext userContext,
							[In, MarshalAs(UnmanagedType.BStr)] String URI,
							[In] ListColumnFilter columnFilter,
							[In, MarshalAs(UnmanagedType.BStr)] String rowFilter);
	}
}
