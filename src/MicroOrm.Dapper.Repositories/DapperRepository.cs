using Dapper;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MicroOrm.Dapper.Repositories
{
    public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity : class
    {
        #region Constructors

        protected DapperRepository(IDbConnection connection)
        {
            Connection = connection;
            SqlGenerator = new SqlGenerator<TEntity>(ESqlConnector.MSSQL);
        }

        protected DapperRepository(IDbConnection connection, ESqlConnector sqlConnector)
        {
            Connection = connection;
            SqlGenerator = new SqlGenerator<TEntity>(sqlConnector);
        }

        protected DapperRepository(IDbConnection connection, ISqlGenerator<TEntity> sqlGenerator)
        {
            Connection = connection;
            SqlGenerator = sqlGenerator;
        }

        #endregion Constructors

        #region Properties

        public IDbConnection Connection { get; }
        public ISqlGenerator<TEntity> SqlGenerator { get; }

        #endregion Properties

        #region Find


        public virtual TEntity Find(Expression<Func<TEntity, bool>> expression)
        {
            return FindAll(expression).FirstOrDefault();
        }

        public virtual TEntity Find<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1)
        {
            return FindAll<TChild1>(expression, tChild1).FirstOrDefault();
        }

        public virtual IEnumerable<TEntity> FindAll()
        {
            return FindAll(null);
        }

        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelect(expression);
            return Connection.Query<TEntity>(queryResult.Sql, queryResult.Param);
        }

        public virtual IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, object>> tChild1)
        {
            return FindAll<TChild1>(null, tChild1);
        }

        public virtual IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1)
        {
            var queryResult = SqlGenerator.GetSelect(expression, tChild1);

            var type = typeof(TEntity);
            var propertyName = ExpressionHelper.GetPropertyName(tChild1);

            IEnumerable<TEntity> result = null;
            var tj1Property = type.GetProperty(propertyName);
            if (tj1Property.PropertyType.IsGenericType)
            {
                var lookup = new Dictionary<object, TEntity>();

                var keyPropertyMeta = SqlGenerator.KeyProperties.FirstOrDefault();
                if (keyPropertyMeta == null)
                    throw new Exception("key not found");

                var keyProperty = keyPropertyMeta.PropertyInfo;

                Connection.Query<TEntity, TChild1, TEntity>(queryResult.Sql, (entity, j1) =>
                {
                    var key = keyProperty.GetValue(entity);

                    TEntity en;
                    if (!lookup.TryGetValue(key, out en))
                    {
                        lookup.Add(key, en = entity);
                    }

                    var list = (List<TChild1>)tj1Property.GetValue(en) ?? new List<TChild1>();
                    if (j1 != null)
                        list.Add(j1);

                    tj1Property.SetValue(en, list);

                    return en;
                }, queryResult.Param);

                result = lookup.Values;
            }
            else
            {
                result = Connection.Query<TEntity, TChild1, TEntity>(queryResult.Sql, (entity, j1) =>
               {
                   type.GetProperty(propertyName).SetValue(entity, j1);
                   return entity;
               }, queryResult.Param);
            }

            return result;
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsync()
        {
            return await FindAllAsync(null);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelect(expression);
            return await Connection.QueryAsync<TEntity>(queryResult.Sql, queryResult.Param);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, object>> tChild1)
        {
            return await FindAllAsync<TChild1>(null, tChild1);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1)
        {
            var queryResult = SqlGenerator.GetSelect(expression, tChild1);

            var type = typeof(TEntity);
            var propertyName = ExpressionHelper.GetPropertyName(tChild1);

            IEnumerable<TEntity> result = null;
            var tj1Property = type.GetProperty(propertyName);
            if (tj1Property.PropertyType.IsGenericType)
            {
                var lookup = new Dictionary<object, TEntity>();

                var keyPropertyMeta = SqlGenerator.KeyProperties.FirstOrDefault();
                if (keyPropertyMeta == null)
                    throw new Exception("key not found");

                var keyProperty = keyPropertyMeta.PropertyInfo;

                await Connection.QueryAsync<TEntity, TChild1, TEntity>(queryResult.Sql, (entity, j1) =>
                {
                    var key = keyProperty.GetValue(entity);

                    TEntity en;
                    if (!lookup.TryGetValue(key, out en))
                    {
                        lookup.Add(key, en = entity);
                    }

                    var list = (List<TChild1>)tj1Property.GetValue(en) ?? new List<TChild1>();
                    if (j1 != null)
                        list.Add(j1);

                    tj1Property.SetValue(en, list);

                    return en;
                }, queryResult.Param);

                result = lookup.Values;
            }
            else
            {
                result = await Connection.QueryAsync<TEntity, TChild1, TEntity>(queryResult.Sql, (entity, j1) =>
                {
                    type.GetProperty(propertyName).SetValue(entity, j1);
                    return entity;
                }, queryResult.Param);
            }

            return result;

        }

        public virtual async Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1)
        {
            return (await FindAllAsync<TChild1>(expression, tChild1)).FirstOrDefault();
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
                var newId = Connection.Query<long>(queryResult.Sql, queryResult.Param).FirstOrDefault();
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
            bool added = false;

            var queryResult = SqlGenerator.GetInsert(instance);

            if (SqlGenerator.IsIdentity)
            {
                //hack: https://github.com/StackExchange/dapper-dot-net/pull/338
                var res = (await Connection.QueryAsync<dynamic>(queryResult.Sql, queryResult.Param)).FirstOrDefault();
                var dictionary = (IDictionary<string, object>)res;
                if (dictionary != null && dictionary.Keys.Any())
                {
                    var newId = Convert.ToInt64(dictionary[dictionary.Keys.First()]);
                    added = newId > 0;

                    if (added)
                    {
                        var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentityProperty.PropertyInfo.PropertyType);
                        SqlGenerator.IdentityProperty.PropertyInfo.SetValue(instance, newParsedId);
                    }
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

        public IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwFiled)
        {
            return FindAllBetween(from, to, btwFiled, null);
        }

        public IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, btwFiled, expression);
            var data = Connection.Query<TEntity>(queryResult.Sql, queryResult.Param);
            return data;
        }

        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwFiled)
        {
            return await FindAllBetweenAsync(from, to, btwFiled, null);
        }
        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, btwFiled, expression);
            var data = await Connection.QueryAsync<TEntity>(queryResult.Sql, queryResult.Param);
            return data;
        }

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwFiled)
        {
            return FindAllBetween(from, to, btwFiled, null);
        }

        public IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> expression)
        {
            var fromString = from.ToString(DateTimeFormat);
            var toString = to.ToString(DateTimeFormat);
            return FindAllBetween(fromString, toString, btwFiled, expression);
        }

        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwFiled)
        {
            return await FindAllBetweenAsync(from, to, btwFiled, null);
        }
        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> expression)
        {
            var fromString = from.ToString(DateTimeFormat);
            var toString = to.ToString(DateTimeFormat);
            return await FindAllBetweenAsync(fromString, toString, btwFiled, expression);
        }

        #endregion Beetwen
    }
}