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
    public partial class DapperRepository<TEntity> 
        where TEntity : class
    {

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

        /// <summary>
        ///     Dummy type for excluding from multi-map
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Local
        private class DontMap
        {
        }
    }
}
