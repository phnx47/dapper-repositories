using MicroOrm.Dapper.Repositories.DbContext;
using MicroOrm.Dapper.Repositories.Tests.Classes;

namespace MicroOrm.Dapper.Repositories.Tests.DbContexts
{
    public interface IDbContext : IDapperDbContext
    {
        IDapperRepository<Address> Address { get; }
        
        IDapperRepository<User> Users { get; }
        
        IDapperRepository<Car> Cars { get; }
        
        IDapperRepository<City> Cities { get; }
        
        IDapperRepository<Report> Reports { get; }
        
        IDapperRepository<Phone> Phones { get; }
    }
}