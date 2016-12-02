using MicroOrm.Dapper.Repositories.DbContext;
using MicroOrm.Dapper.Repositories.Tests.Classes;

namespace MicroOrm.Dapper.Repositories.Tests.DbContexts
{
    public interface IDbContext
    {
        IDapperRepository<User> Users { get; }

        IDapperRepository<Car> Cars { get; }
    }
}
