using Dapper;
using MicroOrm.Dapper.Repositories.Extensions;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        /// Dummy type for excluding from multi-map
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Local
        private class DontMapClass
        {
        }

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
            return Connection.QueryFirstOrDefault<TEntity>(queryResult.GetSql(), queryResult.Param);
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
            return (await ExecuteJoinQueryAsync<TChild1, DontMapClass, DontMapClass, DontMapClass, DontMapClass, DontMapClass>(queryResult, transaction, tChild1)).FirstOrDefault();
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1);
            return (await ExecuteJoinQueryAsync<TChild1, DontMapClass, DontMapClass, DontMapClass, DontMapClass, DontMapClass>(queryResult, transaction, tChild1)).FirstOrDefault();
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
            return Connection.Query<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        private IEnumerable<TEntity> FindAll(SqlQuery sqlQuery, IDbTransaction transaction = null)
        {
            return Connection.Query<TEntity>(sqlQuery.GetSql(), sqlQuery.Param, transaction);
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
                if (SqlGenerator.KeySqlProperties.Length > 1)
                    throw new Exception("Joining with CompositeKeys isn't supported");

                var keyPropertyMeta = SqlGenerator.KeySqlProperties.FirstOrDefault();
                if (keyPropertyMeta == null)
                    throw new Exception("key not found");

                var keyProperty = keyPropertyMeta.PropertyInfo;

                Connection.Query<TEntity, TChild1, TEntity>(sqlQuery.GetSql(), (entity, j1) =>
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
                result = Connection.Query<TEntity, TChild1, TEntity>(sqlQuery.GetSql(), (entity, j1) =>
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
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(IDbTransaction transaction = null)
        {
            return FindAllAsync(predicate: null, transaction: transaction);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate);
            return FindAllAsync(queryResult, transaction);
        }

        /// <summary>
        ///
        /// </summary>
        private Task<IEnumerable<TEntity>> FindAllAsync(SqlQuery sqlQuery, IDbTransaction transaction = null)
        {
            return Connection.QueryAsync<TEntity>(sqlQuery.GetSql(), sqlQuery.Param, transaction);
        }

        /// <summary>
        /// Get all objects with join objects
        /// </summary>
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            return FindAllAsync<TChild1>(null, tChild1, transaction);
        }

        /// <summary>
        /// Get all objects with join objects
        /// </summary>
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate, tChild1);
            return ExecuteJoinQueryAsync<TChild1, DontMapClass, DontMapClass, DontMapClass, DontMapClass, DontMapClass>(queryResult, transaction, tChild1);
        }


        /// <summary>
        /// Get all objects with join objects
        /// </summary>
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2>(
           Expression<Func<TEntity, object>> tChild1,
           Expression<Func<TEntity, object>> tChild2,
           IDbTransaction transaction = null)
        {
            return FindAllAsync<TChild1, TChild2>(null, tChild1, tChild2, transaction);
        }

        /// <summary>
        /// Get all objects with join objects
        /// </summary>
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2>(
           Expression<Func<TEntity, bool>> predicate,
           Expression<Func<TEntity, object>> tChild1,
           Expression<Func<TEntity, object>> tChild2,
           IDbTransaction transaction = null)
        {
            var sqlQuery = new SqlGenerator<TEntity>().GetSelectAll(predicate, tChild1, tChild2);
            return ExecuteJoinQueryAsync<TChild1, TChild2, DontMapClass, DontMapClass, DontMapClass, DontMapClass>(sqlQuery, transaction, tChild1, tChild2);
        }


        /// <summary>
        /// Get all objects with join objects
        /// </summary>
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3>(
           Expression<Func<TEntity, object>> tChild1,
           Expression<Func<TEntity, object>> tChild2,
           Expression<Func<TEntity, object>> tChild3,
           IDbTransaction transaction = null)
        {
            return FindAllAsync<TChild1, TChild2, TChild3>(null, tChild1, tChild2, tChild3, transaction);
        }

        /// <summary>
        /// Get all objects with join objects
        /// </summary>
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3>(
           Expression<Func<TEntity, bool>> predicate,
           Expression<Func<TEntity, object>> tChild1,
           Expression<Func<TEntity, object>> tChild2,
           Expression<Func<TEntity, object>> tChild3,
           IDbTransaction transaction = null)
        {
            var sqlQuery = new SqlGenerator<TEntity>().GetSelectAll(predicate, tChild1, tChild2, tChild3);
            return ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, DontMapClass, DontMapClass, DontMapClass>(sqlQuery, transaction, tChild1, tChild2, tChild3);
        }

        private async Task<IEnumerable<TEntity>> ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(
             SqlQuery sqlQuery,
             IDbTransaction transaction,
             params Expression<Func<TEntity, object>>[] includes)
        {
            var type = typeof(TEntity);

            var childPropertyNames = includes.Select(ExpressionHelper.GetPropertyName).ToList();
            var childProperties = childPropertyNames.Select(p => type.GetProperty(p)).ToList();

            if (SqlGenerator.KeySqlProperties.Length > 1)
                throw new Exception("Joining with CompositeKeys isn't supported");

            var keyPropertyMeta = SqlGenerator.KeySqlProperties.FirstOrDefault();
            if (keyPropertyMeta == null)
                throw new Exception("key not found");

            var keyProperty = keyPropertyMeta.PropertyInfo;
            var childKeyProperties = new List<PropertyInfo>();

            foreach (var property in childProperties)
            {
                var childType = property.PropertyType.IsGenericType() ? property.PropertyType.GenericTypeArguments[0] : property.PropertyType;
                var properties = childType.GetProperties().Where(ExpressionHelper.GetPrimitivePropertiesPredicate());
                childKeyProperties.Add(properties.First(p => p.GetCustomAttributes<KeyAttribute>().Any()));
            }

            var lookup = new Dictionary<object, TEntity>();
            const bool buffered = true;

            var spiltOn = "Id";
            var childKeyNames = childKeyProperties.Select(p => p.Name).ToList();
            if (childKeyNames.Any(p => p != spiltOn))
            {
                spiltOn = string.Join(",", childKeyNames);
            }

            switch (includes.Length)
            {
                case 1:
                    await Connection.QueryAsync<TEntity, TChild1, TEntity>(sqlQuery.GetSql(), (entity, child1) => EntityMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperty, childKeyProperties, childProperties, childPropertyNames, type, entity, child1), sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 2:
                    await Connection.QueryAsync<TEntity, TChild1, TChild2, TEntity>(sqlQuery.GetSql(), (entity, child1, child2) => EntityMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperty, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2), sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 3:
                    await Connection.QueryAsync<TEntity, TChild1, TChild2, TChild3, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3) => EntityMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperty, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3), sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 4:
                    await Connection.QueryAsync<TEntity, TChild1, TChild2, TChild3, TChild4, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3, child4) => EntityMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperty, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3, child4), sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 5:
                    await Connection.QueryAsync<TEntity, TChild1, TChild2, TChild3, TChild4, TChild5, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3, child4, child5) => EntityMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperty, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3, child4, child5), sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 6:
                    await Connection.QueryAsync<TEntity, TChild1, TChild2, TChild3, TChild4, TChild5, TChild6, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3, child4, child5, child6) => EntityMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperty, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3, child4, child5, child6), sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                default:
                    throw new NotSupportedException();
            }

            return lookup.Values;
        }

        private static TEntity EntityMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(IDictionary<object, TEntity> lookup, PropertyInfo keyProperty, IList<PropertyInfo> childKeyProperties, IList<PropertyInfo> childProperties, IList<string> propertyNames, Type entityType, TEntity entity, params object[] childs)
        {
            var key = keyProperty.GetValue(entity);

            TEntity target;
            if (!lookup.TryGetValue(key, out target))
            {
                lookup.Add(key, target = entity);
            }

            for (var i = 0; i < childs.Length; i++)
            {
                var child = childs[i];
                var childProperty = childProperties[i];
                var propertyName = propertyNames[i];
                var childKeyProperty = childKeyProperties[i];

                if (childProperty.PropertyType.IsGenericType())
                {
                    var list = (IList)childProperty.GetValue(target);
                    if (list == null)
                    {
                        switch (i)
                        {
                            case 0:
                                list = new List<TChild1>();
                                break;

                            case 1:
                                list = new List<TChild2>();
                                break;

                            case 2:
                                list = new List<TChild3>();
                                break;

                            case 3:
                                list = new List<TChild4>();
                                break;

                            case 4:
                                list = new List<TChild5>();
                                break;

                            case 5:
                                list = new List<TChild6>();
                                break;

                            default:
                                throw new NotSupportedException();
                        }

                        childProperty.SetValue(target, list);
                    }

                    if (child != null)
                    {
                        var childKey = childKeyProperty.GetValue(child);
                        var exist = (from object item in list select childKeyProperty.GetValue(item)).Contains(childKey);
                        if (!exist)
                        {
                            list.Add(child);
                        }
                    }
                }
                else
                {
                    entityType.GetProperty(propertyName).SetValue(target, child);
                }
            }

            return target;
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
                var newId = Connection.Query<long>(queryResult.GetSql(), queryResult.Param, transaction).FirstOrDefault();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentitySqlProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentitySqlProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                added = Connection.Execute(queryResult.GetSql(), instance, transaction) > 0;
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
                var newId = (await Connection.QueryAsync<long>(queryResult.GetSql(), queryResult.Param, transaction)).FirstOrDefault();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentitySqlProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentitySqlProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                added = Connection.Execute(queryResult.GetSql(), instance, transaction) > 0;
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
            var deleted = Connection.Execute(queryResult.GetSql(), queryResult.Param, transaction) > 0;
            return deleted;
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<bool> DeleteAsync(TEntity instance, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetDelete(instance);
            var deleted = await Connection.ExecuteAsync(queryResult.GetSql(), queryResult.Param, transaction) > 0;
            return deleted;
        }

        #endregion Delete

        #region Update

        /// <summary>
        ///
        /// </summary>
        public virtual bool Update(TEntity instance, IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetUpdate(instance);
            var updated = Connection.Execute(sqlQuery.GetSql(), instance, transaction) > 0;
            return updated;
        }

        /// <summary>
        ///
        /// </summary>
        public virtual async Task<bool> UpdateAsync(TEntity instance, IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetUpdate(instance);
            var updated = (await Connection.ExecuteAsync(sqlQuery.GetSql(), instance, transaction)) > 0;
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
        public IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> predicate = null, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, btwField, predicate);
            var data = Connection.Query<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
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
            var data = await Connection.QueryAsync<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
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