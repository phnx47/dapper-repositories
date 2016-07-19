using System;
using System.Data;

namespace MicroOrm.Dapper.Repositories.DbContext
{

    /// <summary>
    /// Class is helper for use and close IDbConnection
    /// </summary>
    public interface IDapperDbContext : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        IDbConnection Connection { get; }

   }
}
