using System.Data;
using Dapper;

namespace MicroOrm.Dapper.Repositories.Extensions
{
    /// <summary>
    /// Currently it only add output parameter with type DbType.Int32. 
    /// It should actually check the prima key type then mapping it to different DbType such as DbType.Int64, etc.
    /// If GUID support is added, it should also be added here.
    /// </summary>
    internal static class OracleDynamicParametersExtensions
    {
        public static void AddOracleOutputParameterForId(this DynamicParameters param)
        {
            param.Add(name: "newId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        }
    }
}
