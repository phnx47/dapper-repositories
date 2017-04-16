using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Dapper;
using Dapper.FastCrud;
using MicroOrm.Dapper.Repositories.Benchmarks.Tests.Classes;
using MicroOrm.Dapper.Repositories.Benchmarks.Tests.Configs;
using MicroOrm.Dapper.Repositories.Benchmarks.Tests.Orm;
using Microsoft.EntityFrameworkCore;

namespace MicroOrm.Dapper.Repositories.Benchmarks.Tests.Benchmarks
{
    [Config(typeof(BenchmarkDotNetConfig))]
    public class Benchmark_Repository_FindAll
    {
        private const int CountQueries = 300;

        [Setup]
        public void Setup()
        {
            DatabaseCreator.CreateDb();
        }

        [Benchmark]
        public void FindAll_Dapper()
        {
            using (IDbConnection connection = new SqlConnection(Consts.ConnectionString))
            {
                for (int i = 0; i < CountQueries; i++)
                {
                    var users = connection.Query<User>("SELECT Id, Name FROM Users");
                }
            }
        }

        [Benchmark]
        public void FindAll_MicroOrm()
        {
            using (IDbConnection connection = new SqlConnection(Consts.ConnectionString))
            {
                var repository = new DapperRepository<User>(connection);
                for (int i = 0; i < CountQueries; i++)
                {
                    var users = repository.FindAll();
                }
            }
        }

        [Benchmark]
        public void FindAll_Entity()
        {
            using (var entityContext = new EntityContext())
            {
                for (int i = 0; i < CountQueries; i++)
                {
                    var users = entityContext.Users.AsNoTracking().ToArray();
                }
            }


        }


        [Benchmark]
        public void FindAll_FastCrud()
        {
            using (IDbConnection connection = new SqlConnection(Consts.ConnectionString))
            {
                for (int i = 0; i < CountQueries; i++)
                {
                    var users = connection.Find<User>();
                }
            }
        }

        [Benchmark]
        public void FindWhereOperator_Dapper()
        {
            using (IDbConnection connection = new SqlConnection(Consts.ConnectionString))
            {
                for (int i = 0; i < CountQueries; i++)
                {
                    var user = connection.Query<User>("SELECT Id, Name FROM Users WHERE Name = @value", new { value = "name50" }).FirstOrDefault();
                }
            }
        }

        [Benchmark]
        public void FindWhereOperator_MicroOrm()
        {
            using (IDbConnection connection = new SqlConnection(Consts.ConnectionString))
            {
                var repository = new DapperRepository<User>(connection);
                for (int i = 0; i < CountQueries; i++)
                {
                    var user = repository.Find(q => q.Name == "name50");
                }
            }
        }

        [Benchmark]
        public void FindWhereOperator_Entity()
        {
            using (var entityContext = new EntityContext())
            {
                for (int i = 0; i < CountQueries; i++)
                {
                    var user = entityContext.Users.AsNoTracking().FirstOrDefault(q => q.Name == "name50");
                }
            }


        }

        [Benchmark]
        public void FindWhereOperator_FastCrud()
        {
            using (IDbConnection connection = new SqlConnection(Consts.ConnectionString))
            {
                for (int i = 0; i < CountQueries; i++)
                {
                    var user = connection.Find<User>(statement => statement
                        .Where($"{nameof(User.Name):C} = @Name")
                        .WithParameters(new { Name = "name50" })).FirstOrDefault();
                }
            }
        }
    }
}