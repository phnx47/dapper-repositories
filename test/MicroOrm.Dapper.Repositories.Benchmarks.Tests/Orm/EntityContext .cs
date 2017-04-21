using MicroOrm.Dapper.Repositories.Benchmarks.Tests.Classes;
using Microsoft.EntityFrameworkCore;

namespace MicroOrm.Dapper.Repositories.Benchmarks.Tests.Orm
{
    public class EntityContext : Microsoft.EntityFrameworkCore.DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Consts.ConnectionString);
        }

        public DbSet<User> Users { get; set; }
    }
}
