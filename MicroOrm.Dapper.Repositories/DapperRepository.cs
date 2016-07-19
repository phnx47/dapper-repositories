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
        public DapperRepository(IDbConnection connection)
        {
            Connection = connection;
            SqlGenerator = new SqlGenerator<TEntity>(ESqlConnector.MSSQL);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DapperRepository(IDbConnection connection, ESqlConnector sqlConnector)
        {
            Connection = connection;
            SqlGenerator = new SqlGenerator<TEntity>(sqlConnector);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DapperRepository(IDbConnection connection, ISqlGenerator<TEntity> sqlGenerator)
        {
            Connection = connection;
            SqlGenerator = sqlGenerator;
        }

        /// <summary>
        ///
        /// </summary>
        public IDbConnection Connection { get; }


        /// <summary>
        ///
        /// </summary>
        public ISqlGenerator<TEntity> SqlGenerator { get; }

        #region Find

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual TEntity Find(IDbTransaction transaction = null)
        {
            return Find(null, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate);
            return Connection.QueryFirstOrDefault<TEntity>(queryResult.Sql, queryResult.Param);
        }

        /// <summary>
        ///
        /// </summary>

        public virtual TEntity Find<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate);
            return FindAll<TChild1>(queryResult, tChild1).FirstOrDefault();
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(null, tChild1);
            return (await FindAllAsync<TChild1>(queryResult, tChild1, transaction)).FirstOrDefault();
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1);
            return (await FindAllAsync<TChild1>(queryResult, tChild1, transaction)).FirstOrDefault();
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate);
            return (await FindAllAsync(queryResult, transaction)).FirstOrDefault();
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<TEntity> FindAsync(IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(null);
            return (await FindAllAsync(queryResult, transaction)).FirstOrDefault();
        }

        #endregion Find

        #region FindAll

        /// <summary>
        ///
        /// </summary>
        public virtual IEnumerable<TEntity> FindAll(IDbTransaction transaction = null)
        {
            return FindAll(predicate: null, transaction: transaction);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate);
            return Connection.Query<TEntity>(queryResult.Sql, queryResult.Param, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        private IEnumerable<TEntity> FindAll(SqlQuery sqlQuery, IDbTransaction transaction = null)
        {
            return Connection.Query<TEntity>(sqlQuery.Sql, sqlQuery.Param, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(null, tChild1);
            return FindAll<TChild1>(queryResult, tChild1, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate, tChild1);
            return FindAll<TChild1>(queryResult, tChild1, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        private IEnumerable<TEntity> FindAll<TChild1>(SqlQuery sqlQuery, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
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
                }, sqlQuery.Param, transaction);

                result = lookup.Values;
            }
            else
            {
                result = Connection.Query<TEntity, TChild1, TEntity>(sqlQuery.Sql, (entity, j1) =>
                {
                    type.GetProperty(propertyName).SetValue(entity, j1);
                    return entity;
                }, sqlQuery.Param, transaction);
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> FindAllAsync(IDbTransaction transaction = null)
        {
            return await FindAllAsync(predicate: null, transaction: transaction);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate);
            return await FindAllAsync(queryResult, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        private async Task<IEnumerable<TEntity>> FindAllAsync(SqlQuery sqlQuery, IDbTransaction transaction = null)
        {
            return await Connection.QueryAsync<TEntity>(sqlQuery.Sql, sqlQuery.Param, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(null, tChild1);
            return await FindAllAsync<TChild1>(queryResult, tChild1, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate, tChild1);
            return await FindAllAsync<TChild1>(queryResult, tChild1, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        private async Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(SqlQuery sqlQuery, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
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
                }, sqlQuery.Param, transaction);

                result = lookup.Values;
            }
            else
            {
                result = await Connection.QueryAsync<TEntity, TChild1, TEntity>(sqlQuery.Sql, (entity, j1) =>
                {
                    type.GetProperty(propertyName).SetValue(entity, j1);
                    return entity;
                }, sqlQuery.Param, transaction);
            }

            return result;
        }

        #endregion FindAll

        #region Insert

        /// <summary>
        ///
        /// </summary>
        public virtual bool Insert(TEntity instance, IDbTransaction transaction = null)
        {
            bool added;

            var queryResult = SqlGenerator.GetInsert(instance);

            if (SqlGenerator.IsIdentity)
            {
                var newId = Connection.Query<long>(queryResult.Sql, queryResult.Param, transaction).FirstOrDefault();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentitySqlProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentitySqlProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                added = Connection.Execute(queryResult.Sql, instance, transaction) > 0;
            }

            return added;
        }

        /// <summary>
        ///
        /// </summary>>
        public virtual async Task<bool> InsertAsync(TEntity instance, IDbTransaction transaction = null)
        {
            bool added;

            var queryResult = SqlGenerator.GetInsert(instance);

            if (SqlGenerator.IsIdentity)
            {
                var newId = (await Connection.QueryAsync<long>(queryResult.Sql, queryResult.Param, transaction)).FirstOrDefault();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentitySqlProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentitySqlProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                added = Connection.Execute(queryResult.Sql, instance, transaction) > 0;
            }

            return added;
        }

        #endregion Insert

        #region Delete

        /// <summary>
        ///
        /// </summary>
        public virtual bool Delete(TEntity instance, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetDelete(instance);
            var deleted = Connection.Execute(queryResult.Sql, queryResult.Param, transaction) > 0;
            return deleted;
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<bool> DeleteAsync(TEntity instance, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetDelete(instance);
            var deleted = (await Connection.ExecuteAsync(queryResult.Sql, queryResult.Param, transaction)) > 0;
            return deleted;
        }

        #endregion Delete

        #region Update

        /// <summary>
        ///
        /// </summary>
        public virtual bool Update(TEntity instance, IDbTransaction transaction = null)
        {
            var query = SqlGenerator.GetUpdate(instance);
            var updated = Connection.Execute(query.Sql, instance, transaction) > 0;
            return updated;
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<bool> UpdateAsync(TEntity instance, IDbTransaction transaction = null)
        {
            var query = SqlGenerator.GetUpdate(instance);
            var updated = (await Connection.ExecuteAsync(query.Sql, instance, transaction)) > 0;
            return updated;
        }

        #endregion Update

        #region Beetwen

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction = null)
        {
            return FindAllBetween(from, to, btwField, null, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, btwField, predicate);
            var data = Connection.Query<TEntity>(queryResult.Sql, queryResult.Param, transaction);
            return data;
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction = null)
        {
            return FindAllBetween(from, to, btwField, null, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var fromString = from.ToString(DateTimeFormat);
            var toString = to.ToString(DateTimeFormat);
            return FindAllBetween(fromString, toString, btwField, predicate);
        }

        /// <summary>
        ///
        /// </summary>
        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction = null)
        {
            return await FindAllBetweenAsync(from, to, btwField, null, transaction);
        }

        /// <summary>
        ///
        /// </summary>>
        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, btwField, predicate);
            var data = await Connection.QueryAsync<TEntity>(queryResult.Sql, queryResult.Param, transaction);
            return data;
        }

        /// <summary>
        ///
        /// </summary>
        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction = null)
        {
            return await FindAllBetweenAsync(from, to, btwField, null, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        public async Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var fromString = from.ToString(DateTimeFormat);
            var toString = to.ToString(DateTimeFormat);
            return await FindAllBetweenAsync(fromString, toString, btwField, predicate, transaction);
        }

        #endregion Beetwen
    }
}