using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;
using MicroOrm.Dapper.Repositories.Extensions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <summary>
    /// Universal SqlGenerator for Tables
    /// </summary>
    public class SqlGenerator<TEntity> : ISqlGenerator<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Sql provider
        /// </summary>
        public ESqlConnector SqlConnector { get; set; }

        /// <summary>
        /// IsIdentity
        /// </summary>
        public bool IsIdentity => IdentitySqlProperty != null;

        /// <summary>
        /// Table Name
        /// </summary>
        public string TableName { get; protected set; }

        /// <summary>
        /// All original properties
        /// </summary>
        public PropertyInfo[] AllProperties { get; protected set; }

        /// <summary>
        /// Identity Metadata property
        /// </summary>
        public SqlPropertyMetadata IdentitySqlProperty { get; protected set; }

        /// <summary>
        /// Keys Metadata sql properties
        /// </summary>
        public SqlPropertyMetadata[] KeySqlProperties { get; protected set; }

        /// <summary>
        /// Metadata sql properties
        /// </summary>
        public SqlPropertyMetadata[] SqlProperties { get; protected set; }

        #region Logic delete

        /// <summary>
        /// Has Logical delete
        /// </summary>
        public bool LogicalDelete { get; protected set; }

        /// <summary>
        /// PropertyName of Status
        /// </summary>
        public string StatusPropertyName { get; protected set; }

        /// <summary>
        ///  Logical delete Value
        /// </summary>
        public object LogicalDeleteValue { get; protected set; }

        #endregion Logic delete

        /// <summary>
        /// Has Date of changed
        /// </summary>
        public bool HasDateChanged => DateChangedProperty != null;

        /// <summary>
        /// Date of Changed Property
        /// </summary>
        public PropertyInfo DateChangedProperty { get; protected set; }

        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        public SqlGenerator()
           : this(ESqlConnector.MSSQL)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SqlGenerator(ESqlConnector sqlConnector)
        {
            InitProperties();

            InitSqlConnector(sqlConnector);
            InitLogicalDeleted();

            //todo: init joins
        }

        private void InitProperties()
        {
            var entityType = typeof(TEntity);
            TableName = GetTableNameOrAlias(entityType);

            AllProperties = entityType.GetProperties().Where(q => q.CanWrite).ToArray();
            var props = AllProperties.Where(ExpressionHelper.GetPrimitivePropertiesPredicate()).ToArray();

            // Filter the non stored properties
            SqlProperties = props.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();

            // Filter key properties
            KeySqlProperties = props.Where(p => p.GetCustomAttributes<KeyAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();

            // Use identity as key pattern
            var identityProperty = props.FirstOrDefault(p => p.GetCustomAttributes<IdentityAttribute>().Any());
            IdentitySqlProperty = identityProperty != null ? new SqlPropertyMetadata(identityProperty) : null;

            var dateChangedProperty = props.FirstOrDefault(p => p.GetCustomAttributes<DateChangedAttribute>().Count() == 1);
            if (dateChangedProperty != null && (dateChangedProperty.PropertyType == typeof(DateTime) || dateChangedProperty.PropertyType == typeof(DateTime?)))
                DateChangedProperty = props.FirstOrDefault(p => p.GetCustomAttributes<DateChangedAttribute>().Any());
        }

        /// <summary>
        /// Init type Sql provider
        /// </summary>
        /// <param name="sqlConnector"></param>
        private void InitSqlConnector(ESqlConnector sqlConnector)
        {
            SqlConnector = sqlConnector;
            switch (SqlConnector)
            {
                case ESqlConnector.MSSQL:

                    TableName = "[" + TableName + "]";

                    foreach (var propertyMetadata in SqlProperties)
                    {
                        propertyMetadata.ColumnName = "[" + propertyMetadata.ColumnName + "]";
                    }

                    foreach (var propertyMetadata in KeySqlProperties)
                    {
                        propertyMetadata.ColumnName = "[" + propertyMetadata.ColumnName + "]";
                    }

                    break;

                case ESqlConnector.MySQL:
                    break;

                case ESqlConnector.PostgreSQL:
                    //todo: ковычки
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(SqlConnector));
            }
        }

        private static string GetTableNameOrAlias(Type t)
        {
            var entityTypeInfo = t.GetTypeInfo();
            var tableAliasAttribute = entityTypeInfo.GetCustomAttribute<TableAttribute>();
            return tableAliasAttribute != null ? tableAliasAttribute.Name : entityTypeInfo.Name;
        }

        private void InitLogicalDeleted()
        {
            var statusProperty =
                SqlProperties.FirstOrDefault(x => x.PropertyInfo.GetCustomAttributes<StatusAttribute>().Any());

            if (statusProperty == null) return;
            StatusPropertyName = statusProperty.ColumnName;

            if (statusProperty.PropertyInfo.PropertyType.IsBool())
            {
                var deleteProperty = AllProperties.FirstOrDefault(p => p.GetCustomAttributes<DeletedAttribute>().Any());
                if (deleteProperty == null) return;

                LogicalDelete = true;
                LogicalDeleteValue = 1; // true
            }
            else if (statusProperty.PropertyInfo.PropertyType.IsEnum())
            {
                var deleteOption = statusProperty.PropertyInfo.PropertyType.GetFields().FirstOrDefault(f => f.GetCustomAttribute<DeletedAttribute>() != null);

                if (deleteOption == null) return;

                var enumValue = Enum.Parse(statusProperty.PropertyInfo.PropertyType, deleteOption.Name);

                if (enumValue != null)
                    LogicalDeleteValue = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(statusProperty.PropertyInfo.PropertyType));

                LogicalDelete = true;
            }
        }

        #endregion Init

        #region Insert

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual SqlQuery GetInsert(TEntity entity)
        {
            var properties = (IsIdentity ? SqlProperties.Where(p => !p.Name.Equals(IdentitySqlProperty.Name, StringComparison.OrdinalIgnoreCase)) : SqlProperties).ToList();

            if (HasDateChanged)
            {
                DateChangedProperty.SetValue(entity, DateTime.UtcNow);
            }

            var columNames = string.Join(", ", properties.Select(p => p.ColumnName));
            var values = string.Join(", ", properties.Select(p => "@" + p.Name));

            var query = new SqlQuery(entity);
            query.SqlBuilder.Append("INSERT INTO " + TableName + (string.IsNullOrEmpty(columNames) ? "" : "(" + columNames + ")") + (string.IsNullOrEmpty(values) ? "" : " VALUES  (" + values + ")"));

            if (IsIdentity)
            {
                switch (SqlConnector)
                {
                    case ESqlConnector.MSSQL:
                        query.SqlBuilder.Append("SELECT SCOPE_IDENTITY() AS " + IdentitySqlProperty.ColumnName);
                        break;

                    case ESqlConnector.MySQL:
                        query.SqlBuilder.Append("; SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS " + IdentitySqlProperty.ColumnName);
                        break;

                    case ESqlConnector.PostgreSQL:
                        query.SqlBuilder.Append("RETURNING " + IdentitySqlProperty.ColumnName);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return query;
        }

        #endregion Insert

        #region Update

        /// <summary>
        /// Get SQL for UPDATE Query
        /// </summary>
        public virtual SqlQuery GetUpdate(TEntity entity)
        {
            var properties = SqlProperties.Where(p => !KeySqlProperties.Any(k => k.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));

            if (HasDateChanged)
            {
                DateChangedProperty.SetValue(entity, DateTime.UtcNow);
            }

            var query = new SqlQuery(entity);
            query.SqlBuilder.Append("UPDATE " + TableName + " SET " + string.Join(", ", properties.Select(p => p.ColumnName + " = @" + p.Name)) + " WHERE " + string.Join(" AND ", KeySqlProperties.Select(p => p.ColumnName + " = @" + p.Name)));

            return query;
        }

        #endregion Update

        #region Select

        /// <summary>
        ///
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, true, includes);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, false, includes);
        }

        private SqlQuery InitBuilderSelect(bool firstOnly)
        {
            var query = new SqlQuery();

            var select = "SELECT ";

            if (firstOnly && SqlConnector == ESqlConnector.MSSQL)
                select += "TOP 1 ";

            query.SqlBuilder.Append(select + GetFieldsSelect(TableName, SqlProperties));

            return query;
        }

        private string AppendJoinToSelect(SqlQuery originalBuilder, params Expression<Func<TEntity, object>>[] includes)
        {
            var joinSql = "";

            List<PropertyInfo> joinedProperties = new List<PropertyInfo>();
            foreach (var include in includes)
            {
                var propertyName = ExpressionHelper.GetFullPropertyName(include);
                var joinProperty = AllProperties.Concat(joinedProperties).First(x =>
                {
                    if (x.DeclaringType != null)
                        return $"{x.DeclaringType.FullName}.{x.Name}" == propertyName;
                    return false;
                });
                var tableName = GetTableNameOrAlias(joinProperty.DeclaringType);

                var attrJoin = joinProperty.GetCustomAttribute<JoinAttributeBase>();
                if (attrJoin == null) continue;

                var joinString = "";
                if (attrJoin is LeftJoinAttribute)
                {
                    joinString = "LEFT JOIN";
                }
                else if (attrJoin is InnerJoinAttribute)
                {
                    joinString = "INNER JOIN";
                }
                else if (attrJoin is RightJoinAttribute)
                {
                    joinString = "RIGHT JOIN";
                }

                var joinType = joinProperty.PropertyType.IsGenericType() ? joinProperty.PropertyType.GenericTypeArguments[0] : joinProperty.PropertyType;
                joinedProperties.AddRange(joinType.GetProperties().Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()));

                    var properties = joinType.GetProperties().Where(ExpressionHelper.GetPrimitivePropertiesPredicate());
                    var props = properties.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();
                 

                    switch (SqlConnector)
                    {
                        case ESqlConnector.MSSQL:
                            attrJoin.TableName = "[" + attrJoin.TableName + "]";
                            attrJoin.Key = "[" + attrJoin.Key + "]";
                            attrJoin.ExternalKey = "[" + attrJoin.ExternalKey + "]";
                            foreach (var prop in props)
                            {
                                prop.ColumnName = "[" + prop.ColumnName + "]";
                            }
                            break;
                        case ESqlConnector.MySQL:
                            break;
                        case ESqlConnector.PostgreSQL:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(SqlConnector));
                    }

                    originalBuilder.SqlBuilder.Append(", " + GetFieldsSelect(attrJoin.TableName, props));
                    joinSql += joinString + " " + attrJoin.TableName + " ON " + TableName + "." + attrJoin.Key + " = " + attrJoin.TableName + "." + attrJoin.ExternalKey + " ";
                }
            }
            return joinSql;
        }

        private static string GetFieldsSelect(string tableName, IEnumerable<SqlPropertyMetadata> properties)
        {
            //Projection function
            Func<SqlPropertyMetadata, string> projectionFunction = p => !string.IsNullOrEmpty(p.Alias)
                ? tableName + "." + p.ColumnName + " AS " + p.Name
                : tableName + "." + p.ColumnName;

            return string.Join(", ", properties.Select(projectionFunction));
        }

        private SqlQuery GetSelect(Expression<Func<TEntity, bool>> predicate, bool firstOnly, params Expression<Func<TEntity, object>>[] includes)
        {
            var sqlQuery = InitBuilderSelect(firstOnly);

            if (includes.Any())
            {
                var joinsBuilder = AppendJoinToSelect(sqlQuery, includes);
                sqlQuery.SqlBuilder.Append(" FROM " + TableName + " ");
                sqlQuery.SqlBuilder.Append(joinsBuilder);
            }
            else
            {
                sqlQuery.SqlBuilder.Append(" FROM " + TableName + " ");
            }

            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            if (predicate != null)
            {
                // WHERE
                var queryProperties = new List<QueryParameter>();
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(predicate.Body), ExpressionType.Default, ref queryProperties);

                sqlQuery.SqlBuilder.Append("WHERE ");

                for (var i = 0; i < queryProperties.Count; i++)
                {
                    var item = queryProperties[i];
                    var columnName = SqlProperties.First(x => x.Name == item.PropertyName).ColumnName;

                    if (item.PropertyValue == null)
                    {
                        var qOperator = (item.QueryOperator == "=") ? "IS" : "IS NOT";
                        builder.Append($"{TableName}.{columnName} {qOperator} NULL ");
                    }
                    else if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                    {
                        builder.Append($"{item.LinkingOperator} {TableName}.{columnName} {item.QueryOperator} @{item.PropertyName} ");
                    }
                    else
                    {
                        sqlQuery.SqlBuilder.Append(TableName + "." + columnName + " " + item.QueryOperator + " @" + item.PropertyName + " ");
                    }

                    dictionary[item.PropertyName] = item.PropertyValue;
                }

                if (LogicalDelete)
                {
                    sqlQuery.SqlBuilder.Append("AND " + TableName + "." + StatusPropertyName + " != " + LogicalDeleteValue + " ");
                }
            }
            else
            {
                if (LogicalDelete)
                {
                    sqlQuery.SqlBuilder.Append("WHERE " + TableName + "." + StatusPropertyName + " != " + LogicalDeleteValue + " ");
                }
            }

            if (firstOnly && (SqlConnector == ESqlConnector.MySQL || SqlConnector == ESqlConnector.PostgreSQL))
                sqlQuery.SqlBuilder.Append("LIMIT 1");

            sqlQuery.SetParam(dictionary);
            return sqlQuery;
        }

        /// <summary>
        ///
        /// </summary>
        public virtual SqlQuery GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression = null)
        {
            var fieldName = ExpressionHelper.GetPropertyName(btwField);
            var columnName = SqlProperties.First(x => x.Name == fieldName).ColumnName;
            var query = GetSelectAll(expression);
            var op = (expression == null && !LogicalDelete) ? "WHERE" : "AND";

            query.SqlBuilder.Append(" " + op + " " + filedName + " BETWEEN '" + from + "' AND '" + to + "'");

            return query;
        }

        /// <summary>
        ///
        /// </summary>
        public virtual SqlQuery GetDelete(TEntity entity)
        {
            var sqlQuery = new SqlQuery(entity);
            var whereSql = " WHERE " + string.Join("AND ", KeySqlProperties.Select(p => p.ColumnName + " = @" + p.Name));
            if (!LogicalDelete)
            {
                sqlQuery.SqlBuilder.Append("DELETE FROM " + TableName + whereSql);
            }
            else
            {
                if (HasDateChanged)
                {
                    DateChangedProperty.SetValue(entity, DateTime.UtcNow);
                }
                sqlQuery.SqlBuilder.Append("UPDATE " + TableName + " SET " + StatusPropertyName + " = " + LogicalDeleteValue + whereSql);
            }

            return sqlQuery;
        }

        /// <summary>
        /// Fill query properties
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="linkingType">Type of the linking.</param>
        /// <param name="queryProperties">The query properties.</param>
        private void FillQueryProperties(BinaryExpression body, ExpressionType linkingType, ref List<QueryParameter> queryProperties)
        {
            if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
            {
                var propertyName = ExpressionHelper.GetPropertyName(body);

                if (!SqlProperties.Select(x => x.Name).Contains(propertyName))
                {
                    throw new NotImplementedException("predicate can't parse");
                }

                var propertyValue = ExpressionHelper.GetValue(body.Right);
                var opr = ExpressionHelper.GetSqlOperator(body.NodeType);
                var link = ExpressionHelper.GetSqlOperator(linkingType);

                queryProperties.Add(new QueryParameter(link, propertyName, propertyValue, opr));
            }
            else
            {
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(body.Left), body.NodeType, ref queryProperties);
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(body.Right), body.NodeType, ref queryProperties);
            }
        }

        #endregion Select
    }
}