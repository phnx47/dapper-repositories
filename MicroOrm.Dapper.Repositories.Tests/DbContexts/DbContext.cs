using System;
using System.Data;
using MicroOrm.Dapper.Repositories.DbContext;

namespace MicroOrm.Dapper.Repositories.Tests.DbContexts
{
    public class DbContext : DapperDbContext
    {
        public DbContext(IDbConnection connection)
            :base(connection)
        {
            
        }
    }
}
