#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Enforce References
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

namespace TcmDebugger.Misc
{
    /// <summary>
    /// <see cref="T:EnforceReferences" /> forces additional references to be deployed with the project when building
    /// </summary>
    public class EnforceReferences
    {
        global::Tridion.ContentManager.Data.AdoNet.Sql.SqlDatabaseUtilities sqlDatabaseUtilities = null;        
    }
}
