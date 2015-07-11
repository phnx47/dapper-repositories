# MicroOrm.Dapper.Repositories

If you like your code runs fast, probably you know about Micro ORMs.
They are simple and one of their main goals is be the fastest way to execute your SQL sentences on your data repositories.
However, for some of them you need to write your own SQL sentences. This is the case of the most popular Micro ORM [Dapper](https://github.com/StackExchange/dapper-dot-net)

The idea of this tool is to abstract the generation of the SQL sentence for CRUD operations based on each POCO class "metadata".
We know there are plugins for both Micro ORMs to implement the execution of this kind of tasks,
but that's exactly the difference of this tool. The "SQL Generator" is a generic component
that generates all the CRUD sentences for a POCO class based on its definition with the possibility to override the way the SQL generator builds each sentence.

I tested this with MSSQL, PostgreSQL(partially) and MySQL .

Available on [nuget](https://www.nuget.org/packages/MicroOrm.Dapper.Repositories/)

	PM> Install-Package MicroOrm.Dapper.Repositories

Goals
-----
*  Avoid writing SQL.
*  Avoid possible overwhelming of your application by using Reflection on each CRUD operation execution. The best idea about this is handling SQL Generators as singletons.
*  Abstract the SQL generation process and reuse the same implementation with both Micro ORMs [Dapper](https://github.com/StackExchange/dapper-dot-net) or even other kind of tools rather than Micro ORMs

Metadata attributes
-------------------

###	[Key]
From System.ComponentModel.DataAnnotations

###	[Table]
From System.ComponentModel.DataAnnotations.Schema - By default the database table name will match the model name but it can be overridden with this.

### [Column]
From System.ComponentModel.DataAnnotations.Schema - By default the column name will match the property name but it can be overridden with this.

### [NotMapped]
From System.ComponentModel.DataAnnotations.Schema - For "logical" properties that does not have a corresponding column and have to be ignored by the SQL Generator.

###	[Status]
For tables that implements "logical deletes" instead of physical deletes. This attribute can decorate only `enum` properties and one of the possible values for that enumeration has to be decorated with the "Deleted" attribute

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

Lets see some SQL sentences examples that this tool will create. "Users" POCO:

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
    
Simple as that, we have defined a fully functional data repository for the "User" POCO. Because the inheritance pattern we are doing here, both repository contract and repository implementation contains this functions:

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
    
    var allUsers = await userRepository.FindAllAsync(x => x.AccountId == 3 && x.Status != UserStatus.Deleted); // all users for 3 account and not deleted
