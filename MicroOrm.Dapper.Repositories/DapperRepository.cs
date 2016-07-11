using Dapper;
using MicroOrm.Dapper.Repositories.Extensions;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    /// Base Repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DapperRepository(IDbConnection connection, IDbTransaction transaction = null)
        {
            Connection = connection;
            Transaction = transaction;
            SqlGenerator = new SqlGenerator<TEntity>(ESqlConnector.MSSQL);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlConnector"></param>
        /// <param name="transaction"></param>
        public DapperRepository(IDbConnection connection, ESqlConnector sqlConnector, IDbTransaction transaction = null)
        {
            Connection = connection;
            Transaction = transaction;
            SqlGenerator = new SqlGenerator<TEntity>(sqlConnector);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlGenerator"></param>
        /// <param name="transaction"></param>
        public DapperRepository(IDbConnection connection, ISqlGenerator<TEntity> sqlGenerator, IDbTransaction transaction = null)
        {
            Connection = connection;
            Transaction = transaction;
            SqlGenerator = sqlGenerator;
        }

        /// <summary>
        ///
        /// </summary>
        public IDbConnection Connection { get; }

        /// <summary>
        ///
        /// </summary>
        public IDbTransaction Transaction { get; }

        /// <summary>
        ///
        /// </summary>
        public ISqlGenerator<TEntity> SqlGenerator { get; }

        #region Find

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual TEntity Find()
        {
            return Find(null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual TEntity Find(Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelectFirst(expression);
            return Connection.QueryFirstOrDefault<TEntity>(queryResult.Sql, queryResult.Param);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="expression"></param>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        public virtual TEntity Find<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1)
        {
            var queryResult = SqlGenerator.GetSelectFirst(expression);
            return FindAll<TChild1>(queryResult, tChild1).FirstOrDefault();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, object>> tChild1)
        {
            var queryResult = SqlGenerator.GetSelectFirst(null, tChild1);
            return (await FindAllAsync<TChild1>(queryResult, tChild1)).FirstOrDefault();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="expression"></param>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1)
        {
            var queryResult = SqlGenerator.GetSelectFirst(expression, tChild1);
            return (await FindAllAsync<TChild1>(queryResult, tChild1)).FirstOrDefault();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelectFirst(expression);
            return (await FindAllAsync(queryResult)).FirstOrDefault();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual async Task<TEntity> FindAsync()
        {
            var queryResult = SqlGenerator.GetSelectFirst(null);
            return (await FindAllAsync(queryResult)).FirstOrDefault();
        }

        #endregion Find

        #region FindAll

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> FindAll()
        {
            return FindAll(expression: null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelectAll(expression);
            return Connection.Query<TEntity>(queryResult.Sql, queryResult.Param, Transaction);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        private IEnumerable<TEntity> FindAll(SqlQuery sqlQuery)
        {
            return Connection.Query<TEntity>(sqlQuery.Sql, sqlQuery.Param, Transaction);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, object>> tChild1)
        {
            var queryResult = SqlGenerator.GetSelectAll(null, tChild1);
            return FindAll<TChild1>(queryResult, tChild1);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="expression"></param>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1)
        {
            var queryResult = SqlGenerator.GetSelectAll(expression, tChild1);
            return FindAll<TChild1>(queryResult, tChild1);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="sqlQuery"></param>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        private IEnumerable<TEntity> FindAll<TChild1>(SqlQuery sqlQuery, Expression<Func<TEntity, object>> tChild1)
        {
            var type = typeof(TEntity);
            IEnumerable<TEntity> result;
            var propertyName = ExpressionHelper.GetPropertyName(tChild1);
            var tj1Property = type.GetProperty(propertyName);
            if (tj1Property.PropertyType.IsGenericType())
            {
                var lookup = new Dictionary<object, TEntity>();

                var keyPropertyMeta = SqlGenerator.KeySqlProperties.FirstOrDefault();
                if (keyPropertyMeta == null)
                    throw new Exception("key not found");

                var keyProperty = keyPropertyMeta.PropertyInfo;

                Connection.Query<TEntity, TChild1, TEntity>(sqlQuery.Sql, (entity, j1) =>
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
                }, sqlQuery.Param);

                result = lookup.Values;
            }
            else
            {
                result = Connection.Query<TEntity, TChild1, TEntity>(sqlQuery.Sql, (entity, j1) =>
                {
                    type.GetProperty(propertyName).SetValue(entity, j1);
                    return entity;
                }, sqlQuery.Param);
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> FindAllAsync()
        {
            return await FindAllAsync(expression: null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelectAll(expression);
            return await FindAllAsync(queryResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        private async Task<IEnumerable<TEntity>> FindAllAsync(SqlQuery sqlQuery)
        {
            return await Connection.QueryAsync<TEntity>(sqlQuery.Sql, sqlQuery.Param, Transaction);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, object>> tChild1)
        {
            var queryResult = SqlGenerator.GetSelectAll(null, tChild1);
            return await FindAllAsync<TChild1>(queryResult, tChild1);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="expression"></param>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1)
        {
            var queryResult = SqlGenerator.GetSelectAll(expression, tChild1);
            return await FindAllAsync<TChild1>(queryResult, tChild1);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="sqlQuery"></param>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        private async Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(SqlQuery sqlQuery, Expression<Func<TEntity, object>> tChild1)
        {
            var type = typeof(TEntity);
            var propertyName = ExpressionHelper.GetPropertyName(tChild1);

            IEnumerable<TEntity> result = null;
            var tj1Property = type.GetProperty(propertyName);
            if (tj1Property.PropertyType.IsGenericType())
            {
                var lookup = new Dictionary<object, TEntity>();

                var keyPropertyMeta = SqlGenerator.KeySqlProperties.FirstOrDefault();
                if (keyPropertyMeta == null)
                    throw new Exception("key not found");

                var keyProperty = keyPropertyMeta.PropertyInfo;

                await Connection.QueryAsync<TEntity, TChild1, TEntity>(sqlQuery.Sql, (entity, j1) =>
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
                }, sqlQuery.Param);

                result = lookup.Values;
            }
            else
            {
                result = await Connection.QueryAsync<TEntity, TChild1, TEntity>(sqlQuery.Sql, (entity, j1) =>
                {
                    type.GetProperty(propertyName).SetValue(entity, j1);
                    return entity;
                }, sqlQuery.Param);
            }

            return result;
        }

        #endregion FindAll

        #region Insert

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual bool Insert(TEntity instance)
        {
            bool added;

            var queryResult = SqlGenerator.GetInsert(instance);

            if (SqlGenerator.IsIdentity)
            {
                var newId = Connection.Query<long>(queryResult.Sql, queryResult.Param, Transaction).FirstOrDefault();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentitySqlProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentitySqlProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                added = Connection.Execute(queryResult.Sql, instance, Transaction) > 0;
            }

            return added;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<bool> InsertAsync(TEntity instance)
        {
            bool added;

            var queryResult = SqlGenerator.GetInsert(instance);

            if (SqlGenerator.IsIdentity)
            {
                var newId = (await Connection.QueryAsync<long>(queryResult.Sql, queryResult.Param, Transaction)).FirstOrDefault();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentitySqlProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentitySqlProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                added = Connection.Execute(queryResult.Sql, instance, Transaction) > 0;
            }

            return added;
        }

        #endregion Insert

        #region Delete

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual bool Delete(TEntity instance)
        {
            var queryResult = SqlGenerator.GetDelete(instance);
            var deleted = Connection.Execute(queryResult.Sql, queryResult.Param, Transaction) > 0;
            return deleted;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(TEntity instance)
        {
            var queryResult = SqlGenerator.GetDelete(instance);
            var deleted = (await Connection.ExecuteAsync(queryResult.Sql, queryResult.Param, Transaction)) > 0;
            return deleted;
        }

        #endregion Delete

        #region Update

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual bool Update(TEntity instance)
        {
            var query = SqlGenerator.GetUpdate(instance);
            var updated = Connection.Execute(query.Sql, instance, Transaction) > 0;
            return updated;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(TEntity instance)
        {
            var query = SqlGenerator.GetUpdate(instance);
            var updated = (await Connection.ExecuteAsync(query.Sql, instance, Transaction)) > 0;
            return updated;
        }

        #endregion Update

        #region Beetwen

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField)
        {
            return FindAllBetween(from, to, btwField, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, btwField, expression);
            var data = Connection.Query<TEntity>(queryResult.Sql, queryResult.Param, Transaction);
            return data;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField)
        {
            return FindAllBetween(from, to, btwField, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression)
        {
            var fromString = from.ToString(DateTimeFormat);
            var toString = to.ToString(DateTimeFormat);
            return FindAllBetween(fromString, toString, btwField, expression);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField)
        {
            return await FindAllBetweenAsync(from, to, btwField, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, btwField, expression);
            var data = await Connection.QueryAsync<TEntity>(queryResult.Sql, queryResult.Param, Transaction);
            return data;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField)
        {
            return await FindAllBetweenAsync(from, to, btwField, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression)
        {
            var fromString = from.ToString(DateTimeFormat);
            var toString = to.ToString(DateTimeFormat);
            return await FindAllBetweenAsync(fromString, toString, btwField, expression);
        }

        #endregion Beetwen
    }
}