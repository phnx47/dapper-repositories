[![MyGet](https://img.shields.io/myget/phnx47-beta/vpre/MicroOrm.Dapper.Repositories.svg)](https://www.myget.org/feed/phnx47-beta/package/nuget/MicroOrm.Dapper.Repositories)
[![NuGet](https://img.shields.io/nuget/v/MicroOrm.Dapper.Repositories.svg)](https://www.nuget.org/packages/MicroOrm.Dapper.Repositories)
[![NuGet](https://img.shields.io/nuget/dt/MicroOrm.Dapper.Repositories.svg)](https://www.nuget.org/packages/MicroOrm.Dapper.Repositories)

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/3f47d1be6c484a89a7204a38d6f67b6a)](https://www.codacy.com/app/phnx47/MicroOrm.Dapper.Repositories?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=phnx47/MicroOrm.Dapper.Repositories&amp;utm_campaign=Badge_Grade)
[![GitHub contributors](https://img.shields.io/github/contributors/phnx47/MicroOrm.Dapper.Repositories.svg)](https://github.com/phnx47/MicroOrm.Dapper.Repositories/graphs/contributors)
[![Build status](https://ci.appveyor.com/api/projects/status/5v68lbhwc9d4948g?svg=true)](https://ci.appveyor.com/project/phnx47/microorm-dapper-repositories)
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
that generates all the CRUD sentences for a POCO class based on its definition and the possibility to override the SQL generatorand the way it builds each sentence.

The original idea was taken from [Yoinbol](https://github.com/Yoinbol/MicroOrm.Pocos.SqlGenerator).

I tested this with MSSQL, PostgreSQL and MySQL.

    PM> Install-Package MicroOrm.Dapper.Repositories


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

**[InnerJoin]**  

**[RightJoin]**  

**[UpdatedAt]**  

#### Notes

*  By default the SQL Generator is going to map the POCO name with the table name, and each public property to a column.
*  If the [Deleted] is used on a certain POCO, the sentence will be an update instead of a delete.
*  Supports complex primary keys.
*  Supports simple Joins.

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

Query:

    var user = await userRepository.FindAsync(x => x.Id == 5);
    
    var allUsers = await userRepository.FindAllAsync(x => x.AccountId == 3 && x.Deleted != false); // all users for account id 3 and not deleted
