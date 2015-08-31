using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using MicroOrm.Dapper.Repositories.SqlGenerator.Interfaces;
using MicroOrm.Dapper.Repositories.SqlGenerator.Models;

namespace MicroOrm.Dapper.Repositories.Repositories
{
    public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity : new()
    {
        #region Constructors

        protected DapperRepository(IDbConnection connection, ISqlGenerator<TEntity> sqlGenerator)
        {
            Connection = connection;
            SqlGenerator = sqlGenerator;
        }

        #endregion Constructors

        #region Properties


        protected ISqlGenerator<TEntity> SqlGenerator { get; }

        protected IDbConnection Connection { get; }

        #endregion Properties

        #region Find

        public virtual IEnumerable<TEntity> FindAll()
        {
            var queryResult = SqlGenerator.GetSelect();
            return Connection.Query<TEntity>(queryResult.Sql);
        }

        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelect(expression);
            return Connection.Query<TEntity>(queryResult.Sql, queryResult.Param);
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> expression)
        {
            return FindAll(expression).FirstOrDefault();
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsync()
        {
            var queryResult = SqlGenerator.GetSelect();
            return await Connection.QueryAsync<TEntity>(queryResult.Sql);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelect(expression);
            return await Connection.QueryAsync<TEntity>(queryResult.Sql, queryResult.Param);
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression)
        {
            return (await FindAllAsync(expression)).FirstOrDefault();
        }

        #endregion Find

        #region Insert

        public virtual bool Insert(TEntity instance)
        {
            bool added;

            var queryResult = SqlGenerator.GetInsert(instance);

            if (SqlGenerator.IsIdentity)
            {
                var newId = Connection.Query<int>(queryResult.Sql, queryResult.Param).FirstOrDefault();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentityProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentityProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                added = Connection.Execute(queryResult.Sql, instance) > 0;
            }

            return added;
        }

        public virtual async Task<bool> InsertAsync(TEntity instance)
        {
            bool added;

            var queryResult = SqlGenerator.GetInsert(instance);

            if (SqlGenerator.IsIdentity)
            {
                //hack
                int newId;
                switch (SqlGenerator.SqlConnector)
                {
                    case ESqlConnector.MSSQL:
                        var decimalId = (await Connection.QueryAsync<decimal>(queryResult.Sql, queryResult.Param)).FirstOrDefault();
                        newId = Convert.ToInt32(decimalId);
                        break;
                    case ESqlConnector.MySQL:
                        var longId = (await Connection.QueryAsync<long>(queryResult.Sql, queryResult.Param)).FirstOrDefault();
                        newId = Convert.ToInt32(longId);
                        break;
                    default:
                        newId = (await Connection.QueryAsync<int>(queryResult.Sql, queryResult.Param)).FirstOrDefault();
                        break;

                }

                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentityProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentityProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                added = Connection.Execute(queryResult.Sql, instance) > 0;
            }

            return added;
        }

        #endregion Insert

        #region Delete

        public virtual bool Delete(TEntity instance)
        {
            var queryResult = SqlGenerator.GetDelete(instance);
            var deleted = Connection.Execute(queryResult.Sql, queryResult.Param) > 0;
            return deleted;
        }

        public virtual async Task<bool> DeleteAsync(TEntity instance)
        {
            var queryResult = SqlGenerator.GetDelete(instance);
            var deleted = (await Connection.ExecuteAsync(queryResult.Sql, queryResult.Param)) > 0;
            return deleted;
        }

        #endregion Delete

        #region Update

        public virtual bool Update(TEntity instance)
        {
            var query = SqlGenerator.GetUpdate(instance);
            var updated = Connection.Execute(query.Sql, instance) > 0;
            return updated;
        }

        public virtual async Task<bool> UpdateAsync(TEntity instance)
        {
            var query = SqlGenerator.GetUpdate(instance);
            var updated = (await Connection.ExecuteAsync(query.Sql, instance)) > 0;
            return updated;
        }

        #endregion Update

        #region Beetwen

        public IEnumerable<TEntity> FindAllBeetwen(object from, object to, Expression<Func<TEntity, object>> btwFiled)
        {
            return FindAllBeetwen(from, to, btwFiled, null);

        }

        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwFiled)
        {
            return await FindAllBetweenAsync(from, to, btwFiled, null);
        }

        public IEnumerable<TEntity> FindAllBeetwen(object from, object to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, btwFiled, expression);
            var data = Connection.Query<TEntity>(queryResult.Sql, queryResult.Param);
            return data;

        }

        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, btwFiled, expression);
            var data = await Connection.QueryAsync<TEntity>(queryResult.Sql, queryResult.Param);
            return data;
        }

        #endregion Beetwen

    }
}