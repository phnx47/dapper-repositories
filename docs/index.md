# MicroOrm.Dapper.Repositories

[![MyGet](https://img.shields.io/myget/phnx47-beta/vpre/MicroOrm.Dapper.Repositories.svg)](https://www.myget.org/feed/phnx47-beta/package/nuget/MicroOrm.Dapper.Repositories)
[![NuGet](https://img.shields.io/nuget/v/MicroOrm.Dapper.Repositories.svg)](https://www.nuget.org/packages/MicroOrm.Dapper.Repositories)
[![NuGet](https://img.shields.io/nuget/dt/MicroOrm.Dapper.Repositories.svg)](https://www.nuget.org/packages/MicroOrm.Dapper.Repositories)

[![Build status](https://ci.appveyor.com/api/projects/status/v4105ivo9emm3qus/branch/master?svg=true)](https://ci.appveyor.com/project/phnx47/microorm-dapper-repositories/branch/master)
[![AppVeyor tests](https://img.shields.io/appveyor/tests/phnx47/microorm-dapper-repositories.svg)](https://ci.appveyor.com/project/phnx47/microorm-dapper-repositories/build/tests)
[![GitHub contributors](https://img.shields.io/github/contributors/phnx47/MicroOrm.Dapper.Repositories.svg)](https://github.com/phnx47/MicroOrm.Dapper.Repositories/graphs/contributors)
[![CodeFactor](https://www.codefactor.io/repository/github/microorm-dotnet/microorm.dapper.repositories/badge)](https://www.codefactor.io/repository/github/microorm-dotnet/microorm.dapper.repositories)
[![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)

[![GitHub stars](https://img.shields.io/github/stars/phnx47/MicroOrm.Dapper.Repositories.svg?style=social&label=Star)](https://github.com/phnx47/MicroOrm.Dapper.Repositories)
[![GitHub forks](https://img.shields.io/github/forks/phnx47/MicroOrm.Dapper.Repositories.svg?style=social&label=Fork)](https://github.com/phnx47/MicroOrm.Dapper.Repositories)
[![GitHub watchers](https://img.shields.io/github/watchers/phnx47/MicroOrm.Dapper.Repositories.svg?style=social&label=Watch)](https://github.com/phnx47/MicroOrm.Dapper.Repositories)

## Description

If you like your code to run fast, you probably know about Micro ORMs.
They are simple and one of their main goals is to be the fastest execution of your SQL sentences in you data repository.
For some Micro ORM's you need to write your own SQL sentences and this is the case of the most popular Micro ORM [Dapper](https://github.com/StackExchange/dapper-dot-net)

This tool abstracts the generation of the SQL sentence for CRUD operations based on each C# POCO class "metadata".
We know there are plugins for both Micro ORMs that implement the execution of these tasks, but that's exactly where this tool is different. The "SQL Generator" is a generic component
that generates all the CRUD sentences for a POCO class based on its definition and the possibility to override the SQL generator and the way it builds each sentence.

The original idea was taken from [Yoinbol](https://github.com/Yoinbol/MicroOrm.Pocos.SqlGenerator).

All tests with MSSQL, PostgreSQL and MySQL has passed, SQLite tests are being developed.

## Installation

    dotnet add package MicroOrm.Dapper.Repositories

## Metadata attributes

**[Key]**  
From `System.ComponentModel.DataAnnotations` - Use for primary key.

**[Identity]**  
Use for identity key.

**[Table]**  
From `System.ComponentModel.DataAnnotations.Schema` - By default the database table name will match the model name but it can be overridden with this.

**[Column]**  
From `System.ComponentModel.DataAnnotations.Schema` - By default the column name will match the property name but it can be overridden with this.

**[NotMapped]**  
From `System.ComponentModel.DataAnnotations.Schema` - For "logical" properties that do not have a corresponding column and have to be ignored by the SQL Generator.

**[Deleted], [Status]**  
For tables that implement "logical deletes" instead of physical deletes. Use this to decorate the `bool` or `enum`.

**[LeftJoin]**  
Left join for property: Collection and object are supported.

**[InnerJoin]**  
Inner join for property: Collection and object are supported.

**[RightJoin]**  
Right join for property: Collection and object are supported.

**[CrossJoin] - SQLite only**  
Cross join for property: Collection and object are supported.

**[UpdatedAt]**  
Automatically set DataTime.UtcNow (You can use local date or define offset) for Insert and Update.

### Notes

* By default the SQL Generator is going to map the POCO name with the table name, and each public property to a column.
* If the [Deleted] is used on a certain POCO, the sentence will be an update instead of a delete.
* Supports complex primary keys.
* Supports simple Joins.
* For this moment, with MSSQL you can only use limit with offset if you call OrderBy first, otherwise limit will be ignored.

## Examples

"Users" POCO:

    [Table("Users")]
    public class User
    {
        [Key, Identity]
        public int Id { get; set; }

        public string ReadOnly => "test"; // because don't have set

        public string Name { get; set; }

        public int AddressId { get; set; }

        [LeftJoin("Cars", "Id", "UserId")]
        public List<Car> Cars { get; set; }

        [LeftJoin("Addresses", "AddressId", "Id")]
        public Address Address { get; set; }

        [Status, Deleted]
        public bool Deleted { get; set; }

        [UpdatedAt]
        public DateTime? UpdatedAt { get; set; }
    }

"Cars" POCO:

    [Table("Cars")]
    public class Car
    {
        [Key, Identity]
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] Data { get; set; }

        public int UserId { get; set; }

        [LeftJoin("Users", "UserId", "Id")]
        public User User { get; set; }

        [Status]
        public StatusCar Status { get; set; }
    }

    public enum StatusCar
    {
        Inactive = 0,

        Active = 1,

        [Deleted]
        Deleted = -1
    }

Implements the repository:

    public class UserRepository : DapperRepository<User>
    {

        public UserRepository(IDbConnection connection, ISqlGenerator<User> sqlGenerator)
            : base(connection, sqlGenerator)
        {

        }
    }

### Queries

Find by ID:

    var user = await userRepository.FindAsync(x => x.Id == 5);
    
Query with limit:

    var limit = 10u;
    var users = await userRepository.SetLimit(limit).FindAllAsync();


Query with limit and offset:

    var limit = 10u;
    var offset = 5u;
    var users = await userRepository.SetLimit(limit, offset).FindAllAsync();


Query with OrderBy:
    
    var users = await userRepository.SetOrderBy(OrderInfo.SortDirection.DESC, x => x.CreatedAt).FindAllAsync();

Query with SetSelect:
    
    var users = await userRepository.SetSelect(x => new {x.Id, x.Name}).FindAllAsync();

Find all users for AccountId equals to 3 and not logical deleted:

    var allUsers = await userRepository.FindAllAsync(x => x.AccountId == 3 && x.Deleted != false);

Update with join:

    userRepository.Update();
### Example with Asp.Net Core and D.I
- ConfigureServices:


    //Your DB Provider
    MicroOrmConfig.SqlProvider = SqlProvider.MySQL;
    //Not required
    MicroOrmConfig.TablePrefix = "db1_";
    //Add generic SqlGenerator as singleton
    services.AddSingleton(typeof(ISqlGenerator<>), typeof(SqlGenerator<>));
    //Your db factory
    services.AddSingleton<IDbConnectionFactory, DbFactory>(x => new DbFactory(appSettings.DbConnectionString));

- Example Repository:


    public class BaseRepository<T> : DapperRepository<T> where T : class
    {
        private readonly IDbConnectionFactory _factory;
        public BaseRepository(IDbConnectionFactory factory, ISqlGenerator<T> generator)
            : base(factory.OpenDbConnection(), generator)
        {
            _factory = factory;
        }
        
        protected IDbConnection GetConnection()
        {
            return _factory.OpenDbConnection();
        }
    }
