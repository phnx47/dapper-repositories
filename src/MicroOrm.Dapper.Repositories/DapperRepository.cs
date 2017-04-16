using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using MicroOrm.Dapper.Repositories.Extensions;
using MicroOrm.Dapper.Repositories.SqlGenerator;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     Base Repository
    /// </summary>
    public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity : class
    {
        /// <summary>
        ///     Dummy type for excluding from multi-map
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Local
        private class DontMap
        {
        }


        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <inheritdoc />
        public IDbConnection Connection { get; }

        /// <inheritdoc />
        public ISqlGenerator<TEntity> SqlGenerator { get; }


        /// <summary>
        ///     Constructor
        /// </summary>
        public DapperRepository(IDbConnection connection)
        {
            Connection = connection;
            SqlGenerator = new SqlGenerator<TEntity>(ESqlConnector.MSSQL);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public DapperRepository(IDbConnection connection, ESqlConnector sqlConnector)
        {
            Connection = connection;
            SqlGenerator = new SqlGenerator<TEntity>(sqlConnector);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public DapperRepository(IDbConnection connection, ISqlGenerator<TEntity> sqlGenerator)
        {
            Connection = connection;
            SqlGenerator = sqlGenerator;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public DapperRepository(IDbConnection connection, SqlGeneratorConfig config)
        {
            Connection = connection;
            SqlGenerator = new SqlGenerator<TEntity>(config);
        }

        /// <inheritdoc />
        public virtual TEntity Find(IDbTransaction transaction = null)
        {
            return Find(null, transaction);
        }

        /// <inheritdoc />
        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            return FindAll(predicate, transaction).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual TEntity Find<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1);
            return ExecuteJoinQuery<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual TEntity Find<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1, tChild2);
            return ExecuteJoinQuery<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual TEntity Find<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1, tChild2, tChild3);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3).FirstOrDefault();
        }


        /// <inheritdoc />
        public virtual TEntity Find<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1, tChild2, tChild3, tChild4);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual TEntity Find<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1, tChild2, tChild3, tChild4, tChild5);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, TChild5, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4, tChild5).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual TEntity Find<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, TChild5, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(null, tChild1);
            return (await ExecuteJoinQueryAsync<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1)).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate);
            return (await FindAllAsync(queryResult, transaction)).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FindAsync(IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(null);
            return (await FindAllAsync(queryResult, transaction)).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1);
            return (await ExecuteJoinQueryAsync<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1)).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1, tChild2);
            return (await ExecuteJoinQueryAsync<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2)).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1, tChild2, tChild3);
            return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3)).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1, tChild2, tChild3, tChild4);
            return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4)).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1, tChild2, tChild3, tChild4, tChild5);
            return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4, tChild5)).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
            return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6)).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll(IDbTransaction transaction = null)
        {
            return FindAll(predicate: null, transaction: transaction);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate);
            return Connection.Query<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate, tChild1);
            return ExecuteJoinQuery<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2);
            return ExecuteJoinQuery<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, DontMap, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4, TChild5>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4, tChild5);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, TChild5, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4, tChild5);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(IDbTransaction transaction = null)
        {
            return FindAllAsync(predicate: null, transaction: transaction);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate);
            return FindAllAsync(queryResult, transaction);
        }

        /// <inheritdoc />
        private Task<IEnumerable<TEntity>> FindAllAsync(SqlQuery sqlQuery, IDbTransaction transaction = null)
        {
            return Connection.QueryAsync<TEntity>(sqlQuery.GetSql(), sqlQuery.Param, transaction);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate, tChild1);
            return ExecuteJoinQueryAsync<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2);
            return ExecuteJoinQueryAsync<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3);
            return ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, DontMap, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4);
            return ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4, tChild5);
            return ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4, tChild5);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
            return ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
        }

        /// <summary>
        ///     Execute Join query
        /// </summary>
        protected virtual IEnumerable<TEntity> ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(
            SqlQuery sqlQuery,
            IDbTransaction transaction,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var type = typeof(TEntity);

            var childPropertyNames = includes.Select(ExpressionHelper.GetPropertyName).ToList();
            var childProperties = childPropertyNames.Select(p => type.GetProperty(p)).ToList();

            if (!SqlGenerator.KeySqlProperties.Any())
                throw new NotSupportedException("Join doesn't support without [Key] attribute");

            var keyProperties = SqlGenerator.KeySqlProperties.Select(q => q.PropertyInfo).ToArray();
            var childKeyProperties = new List<PropertyInfo>();

            foreach (var property in childProperties)
            {
                var childType = property.PropertyType.IsGenericType() ? property.PropertyType.GenericTypeArguments[0] : property.PropertyType;
                var properties = childType.FindClassProperties().Where(ExpressionHelper.GetPrimitivePropertiesPredicate());
                childKeyProperties.AddRange(properties.Where(p => p.GetCustomAttributes<KeyAttribute>().Any()));
            }

            if (!childKeyProperties.Any())
                throw new NotSupportedException("Join doesn't support without [Key] attribute");

            var lookup = new Dictionary<object, TEntity>();
            const bool buffered = true;

            var spiltOn = string.Join(",", childKeyProperties.Select(q => q.Name));

            switch (includes.Length)
            {
                case 1:
                    Connection.Query<TEntity, TChild1, TEntity>(sqlQuery.GetSql(), (entity, child1) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 2:
                    Connection.Query<TEntity, TChild1, TChild2, TEntity>(sqlQuery.GetSql(), (entity, child1, child2) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 3:
                    Connection.Query<TEntity, TChild1, TChild2, TChild3, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 4:
                    Connection.Query<TEntity, TChild1, TChild2, TChild3, TChild4, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3, child4) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3, child4),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 5:
                    Connection.Query<TEntity, TChild1, TChild2, TChild3, TChild4, TChild5, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3, child4, child5) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3, child4, child5),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 6:
                    Connection.Query<TEntity, TChild1, TChild2, TChild3, TChild4, TChild5, TChild6, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3, child4, child5, child6) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3, child4, child5, child6),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                default:
                    throw new NotSupportedException();
            }

            return lookup.Values;
        }


        /// <summary>
        ///     Execute Join query
        /// </summary>
        protected virtual async Task<IEnumerable<TEntity>> ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(
            SqlQuery sqlQuery,
            IDbTransaction transaction,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var type = typeof(TEntity);

            var childPropertyNames = includes.Select(ExpressionHelper.GetPropertyName).ToList();
            var childProperties = childPropertyNames.Select(p => type.GetProperty(p)).ToList();

            if (!SqlGenerator.KeySqlProperties.Any())
                throw new NotSupportedException("Join doesn't support without [Key] attribute");

            var keyProperties = SqlGenerator.KeySqlProperties.Select(q => q.PropertyInfo).ToArray();
            var childKeyProperties = new List<PropertyInfo>();

            foreach (var property in childProperties)
            {
                var childType = property.PropertyType.IsGenericType() ? property.PropertyType.GenericTypeArguments[0] : property.PropertyType;
                var properties = childType.FindClassProperties().Where(ExpressionHelper.GetPrimitivePropertiesPredicate());
                childKeyProperties.AddRange(properties.Where(p => p.GetCustomAttributes<KeyAttribute>().Any()));
            }

            if (!childKeyProperties.Any())
                throw new NotSupportedException("Join doesn't support without [Key] attribute");

            var lookup = new Dictionary<object, TEntity>();
            const bool buffered = true;

            var spiltOn = string.Join(",", childKeyProperties.Select(q => q.Name));
            var sql = sqlQuery.GetSql();
            switch (includes.Length)
            {
                case 1:
                    await Connection.QueryAsync<TEntity, TChild1, TEntity>(sqlQuery.GetSql(), (entity, child1) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 2:
                    await Connection.QueryAsync<TEntity, TChild1, TChild2, TEntity>(sqlQuery.GetSql(), (entity, child1, child2) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 3:
                    await Connection.QueryAsync<TEntity, TChild1, TChild2, TChild3, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 4:
                    await Connection.QueryAsync<TEntity, TChild1, TChild2, TChild3, TChild4, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3, child4) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3, child4),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 5:
                    await Connection.QueryAsync<TEntity, TChild1, TChild2, TChild3, TChild4, TChild5, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3, child4, child5) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3, child4, child5),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                case 6:
                    await Connection.QueryAsync<TEntity, TChild1, TChild2, TChild3, TChild4, TChild5, TChild6, TEntity>(sqlQuery.GetSql(), (entity, child1, child2, child3, child4, child5, child6) =>
                            EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(lookup, keyProperties, childKeyProperties, childProperties, childPropertyNames, type, entity, child1, child2, child3, child4, child5, child6),
                        sqlQuery.Param, transaction, buffered, spiltOn);
                    break;

                default:
                    throw new NotSupportedException();
            }

            return lookup.Values;
        }


        private static TEntity EntityJoinMapping<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(IDictionary<object, TEntity> lookup, PropertyInfo[] keyProperties,
            IList<PropertyInfo> childKeyProperties, IList<PropertyInfo> childProperties, IList<string> propertyNames, Type entityType, TEntity entity, params object[] childs)
        {
            TEntity target;
            var compositeKeyProperty = string.Join("|", keyProperties.Select(q => q.GetValue(entity).ToString()));

            if (!lookup.TryGetValue(compositeKeyProperty, out target))
                lookup.Add(compositeKeyProperty, target = entity);

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

                    if (child == null)
                        continue;

                    var childKey = childKeyProperty.GetValue(child);
                    var exist = (from object item in list select childKeyProperty.GetValue(item)).Contains(childKey);
                    if (!exist)
                        list.Add(child);
                }
                else
                {
                    entityType.GetProperty(propertyName).SetValue(target, child);
                }
            }

            return target;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual bool Delete(TEntity instance, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetDelete(instance);
            var deleted = Connection.Execute(queryResult.GetSql(), queryResult.Param, transaction) > 0;
            return deleted;
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(TEntity instance, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetDelete(instance);
            var deleted = await Connection.ExecuteAsync(queryResult.GetSql(), queryResult.Param, transaction) > 0;
            return deleted;
        }

        /// <inheritdoc />
        public virtual bool Update(TEntity instance, IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetUpdate(instance);
            var updated = Connection.Execute(sqlQuery.GetSql(), instance, transaction) > 0;
            return updated;
        }

        /// <inheritdoc />
        public virtual async Task<bool> UpdateAsync(TEntity instance, IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetUpdate(instance);
            var updated = await Connection.ExecuteAsync(sqlQuery.GetSql(), instance, transaction) > 0;
            return updated;
        }


        /// <inheritdoc />
        public IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction = null)
        {
            return FindAllBetween(from, to, btwField, null, transaction);
        }

        /// <inheritdoc />
        public IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> predicate = null, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, btwField, predicate);
            return Connection.Query<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction = null)
        {
            return FindAllBetween(from, to, btwField, null, transaction);
        }

        /// <inheritdoc />
        public IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var fromString = from.ToString(DateTimeFormat);
            var toString = to.ToString(DateTimeFormat);
            return FindAllBetween(fromString, toString, btwField, predicate);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction = null)
        {
            return FindAllBetweenAsync(from, to, btwField, null, transaction);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, btwField, predicate);
            return Connection.QueryAsync<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction = null)
        {
            return FindAllBetweenAsync(from, to, btwField, null, transaction);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            return FindAllBetweenAsync(from.ToString(DateTimeFormat), to.ToString(DateTimeFormat), btwField, predicate, transaction);
        }
    }
}