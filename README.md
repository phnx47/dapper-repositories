# MicroOrm.Dapper.Repositories

[![NuGet](https://img.shields.io/nuget/v/MicroOrm.Dapper.Repositories.svg)](https://www.nuget.org/packages/MicroOrm.Dapper.Repositories) [![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) [![Build status](https://ci.appveyor.com/api/projects/status/5v68lbhwc9d4948g?svg=true)](https://ci.appveyor.com/project/phnx47/microorm-dapper-repositories) [![Code Climate](https://img.shields.io/codeclimate/coverage/github/triAGENS/ashikawa-core.svg?maxAge=2592000)]()

If you like your code to run fast, you probably know about Micro ORMs.
They are simple and one of their main goals is to be the fastest execution of your SQL sentences in you data repository.
For some Micro ORM's you need to write your own SQL sentences and this is the case of the most popular Micro ORM [Dapper](https://github.com/StackExchange/dapper-dot-net)

This tool abstracts the generation of the SQL sentence for CRUD operations based on each C# POCO class "metadata".
We know there are plugins for both Micro ORMs that implement the execution of these tasks, but that's exactly where this tool is different. The "SQL Generator" is a generic component
that generates all the CRUD sentences for a POCO class based on its definition and the possibility to override the SQL generatorand the way it builds each sentence.

I tested this with MSSQL, PostgreSQL and MySQL.

	PM> Install-Package MicroOrm.Dapper.Repositories

Goals
-----
*  Avoid writing SQL.
*  Avoid  the possible of overwhelming your application using Reflection on each CRUD operation execution.
*  Abstract the SQL generation process and reuse the same implementation with both Micro ORMs [Dapper](https://github.com/StackExchange/dapper-dot-net)

Metadata attributes
-------------------

###	[Key]
From System.ComponentModel.DataAnnotations

###	[Table]
From System.ComponentModel.DataAnnotations.Schema - By default the database table name will match the model name but it can be overridden with this.

### [Column]
From System.ComponentModel.DataAnnotations.Schema - By default the column name will match the property name but it can be overridden with this.

### [NotMapped]
From System.ComponentModel.DataAnnotations.Schema - For "logical" properties that do not have a corresponding column and have to be ignored by the SQL Generator.

###	[Status]
For tables that implement "logical deletes" instead of physical deletes. This attribute can decorate only `enum` properties and one of the possible values for that enumeration has to be decorated with the "Deleted" attribute.

###	[Deleted]
Brother of the previous attribute. Use this to decorate the enum value that specifies the logical delete value for the status property.	

### [Identity]
Use for identity key.

Some notes
----------

*  By default the SQL Generator is going to map the POCO name with the table name, and each public property to a column.
*  If the `Status` is used on a certain POCO, the "delete" sentence will be an update instead of a delete.
*  Complex primary keys are supported.

SQL Sentences
=============

Let's see some SQL sentences examples that this tool will create. "Users" POCO:

	[Table("Users")]
	public class User
	{
		[Key]
		[Identity]
		public int Id { get; set; }
		
		public string Login { get; set;}
		
		[Column("FName")]
		public string FirstName { get; set; }
		
		[Column("LName")]
		public string LastName { get; set; }
		
		public string Email { get; set; }
		
		[Status]
		public UserStatus Status { get; set; }
		
		[NotMapped]
		public string FullName
		{
			get
			{
				return string.Format("{0} {1}", FirstName, LastName);
			}
		}
	}
	
	public enum UserStatus
	{
	    Active = 0,
	
	    Inactive = 1,
	
	    [Deleted]
	    Deleted = 2
	}
	
Implements the repository:

    public class UserRepository : DapperRepository<User>
    {

        public UserRepository(IDbConnection connection, ISqlGenerator<User> sqlGenerator)
            : base(connection, sqlGenerator)
        {


        }
    }
    
Simple as that, we have defined a fully functional data repository for the "User" POCO. Because of the inheritance pattern here, both contract and implementation repositories contain this function:

        bool Delete(TEntity instance);
        
        Task<bool> DeleteAsync(TEntity instance);
        
        TEntity Find(Expression<Func<TEntity, bool>> expression);
        
        IEnumerable<TEntity> FindAll();
        
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression);
        
        Task<IEnumerable<TEntity>> FindAllAsync();
        
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> expression);
        
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression);
        
        bool Insert(TEntity instance);
        
        Task<bool> InsertAsync(TEntity instance);
        
        bool Update(TEntity instance);
        
        Task<bool> UpdateAsync(TEntity instance);

Example:

    var user = await userRepository.FindAsync(x => x.Id == 5);
    
    var allUsers = await userRepository.FindAllAsync(x => x.AccountId == 3 && x.Status != UserStatus.Deleted); // all users for account id 3 and not deleted
