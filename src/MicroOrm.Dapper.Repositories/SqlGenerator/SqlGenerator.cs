using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;
using MicroOrm.Dapper.Repositories.Extensions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public class SqlGenerator<TEntity> : ISqlGenerator<TEntity> where TEntity : class
    {
        /// <inheritdoc />
        public PropertyInfo[] AllProperties { get; protected set; }

        /// <inheritdoc />
        public bool HasUpdatedAt => UpdatedAtProperty != null;

        /// <inheritdoc />
        public PropertyInfo UpdatedAtProperty { get; protected set; }

        /// <inheritdoc />
        public bool IsIdentity => IdentitySqlProperty != null;

        /// <inheritdoc />
        public string TableName { get; protected set; }

        /// <inheritdoc />
        public SqlPropertyMetadata IdentitySqlProperty { get; protected set; }

        /// <inheritdoc />
        public SqlPropertyMetadata[] KeySqlProperties { get; protected set; }

        /// <inheritdoc />
        public SqlPropertyMetadata[] SqlProperties { get; protected set; }

        /// <inheritdoc />
        public SqlGeneratorConfig Config { get; protected set; }

        /// <inheritdoc />
        public bool LogicalDelete { get; protected set; }

        /// <inheritdoc />
        public string StatusPropertyName { get; protected set; }

        /// <inheritdoc />
        public object LogicalDeleteValue { get; protected set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public SqlGenerator()
            : this(new SqlGeneratorConfig { SqlConnector = ESqlConnector.MSSQL, UseQuotationMarks = false })
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public SqlGenerator(ESqlConnector sqlConnector, bool useQuotationMarks = false)
            : this(new SqlGeneratorConfig { SqlConnector = sqlConnector, UseQuotationMarks = useQuotationMarks })
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public SqlGenerator(SqlGeneratorConfig sqlGeneratorConfig)
        {
            // Order is important
            InitProperties();
            InitConfig(sqlGeneratorConfig);
            InitLogicalDeleted();
        }

        private void InitProperties()
        {
            var entityType = typeof(TEntity);
            TableName = GetTableNameOrAlias(entityType);

            AllProperties = entityType.FindClassProperties().Where(q => q.CanWrite).ToArray();
            var props = AllProperties.Where(ExpressionHelper.GetPrimitivePropertiesPredicate()).ToArray();

            // Filter the non stored properties
            SqlProperties = props.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();

            // Filter key properties
            KeySqlProperties = props.Where(p => p.GetCustomAttributes<KeyAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();

            // Use identity as key pattern
            var identityProperty = props.FirstOrDefault(p => p.GetCustomAttributes<IdentityAttribute>().Any());
            IdentitySqlProperty = identityProperty != null ? new SqlPropertyMetadata(identityProperty) : null;

            var dateChangedProperty = props.FirstOrDefault(p => p.GetCustomAttributes<UpdatedAtAttribute>().Count() == 1);
            if (dateChangedProperty != null && (dateChangedProperty.PropertyType == typeof(DateTime) || dateChangedProperty.PropertyType == typeof(DateTime?)))
                UpdatedAtProperty = props.FirstOrDefault(p => p.GetCustomAttributes<UpdatedAtAttribute>().Any());
        }

        /// <summary>
        ///     Init type Sql provider
        /// </summary>
        private void InitConfig(SqlGeneratorConfig sqlGeneratorConfig)
        {
            Config = sqlGeneratorConfig;

            if (Config.UseQuotationMarks)
                switch (Config.SqlConnector)
                {
                    case ESqlConnector.MSSQL:
                        TableName = "[" + TableName + "]";

                        foreach (var propertyMetadata in SqlProperties)
                            propertyMetadata.ColumnName = "[" + propertyMetadata.ColumnName + "]";

                        foreach (var propertyMetadata in KeySqlProperties)
                            propertyMetadata.ColumnName = "[" + propertyMetadata.ColumnName + "]";
                        break;

                    case ESqlConnector.MySQL:
                        TableName = "`" + TableName + "`";

                        foreach (var propertyMetadata in SqlProperties)
                            propertyMetadata.ColumnName = "`" + propertyMetadata.ColumnName + "`";

                        foreach (var propertyMetadata in KeySqlProperties)
                            propertyMetadata.ColumnName = "`" + propertyMetadata.ColumnName + "`";
                        break;

                    case ESqlConnector.PostgreSQL:
                        TableName = "\"" + TableName + "\"";

                        foreach (var propertyMetadata in SqlProperties)
                            propertyMetadata.ColumnName = "\"" + propertyMetadata.ColumnName + "\"";

                        foreach (var propertyMetadata in KeySqlProperties)
                            propertyMetadata.ColumnName = "\"" + propertyMetadata.ColumnName + "\"";
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(Config.SqlConnector));
                }
        }

        private static string GetTableNameOrAlias(Type type)
        {
            var entityTypeInfo = type.GetTypeInfo();
            var tableAliasAttribute = entityTypeInfo.GetCustomAttribute<TableAttribute>();
            return tableAliasAttribute != null ? tableAliasAttribute.Name : entityTypeInfo.Name;
        }

        private void InitLogicalDeleted()
        {
            var statusProperty =
                SqlProperties.FirstOrDefault(x => x.PropertyInfo.GetCustomAttributes<StatusAttribute>().Any());

            if (statusProperty == null)
                return;
            StatusPropertyName = statusProperty.ColumnName;

            if (statusProperty.PropertyInfo.PropertyType.IsBool())
            {
                var deleteProperty = AllProperties.FirstOrDefault(p => p.GetCustomAttributes<DeletedAttribute>().Any());
                if (deleteProperty == null)
                    return;

                LogicalDelete = true;
                LogicalDeleteValue = 1; // true
            }
            else if (statusProperty.PropertyInfo.PropertyType.IsEnum())
            {
                var deleteOption = statusProperty.PropertyInfo.PropertyType.GetFields().FirstOrDefault(f => f.GetCustomAttribute<DeletedAttribute>() != null);

                if (deleteOption == null)
                    return;

                var enumValue = Enum.Parse(statusProperty.PropertyInfo.PropertyType, deleteOption.Name);

                if (enumValue != null)
                    LogicalDeleteValue = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(statusProperty.PropertyInfo.PropertyType));

                LogicalDelete = true;
            }
        }

        private SqlQuery InitBuilderSelect(bool firstOnly)
        {
            var query = new SqlQuery();
            query.SqlBuilder.Append("SELECT " + (firstOnly && Config.SqlConnector == ESqlConnector.MSSQL ? "TOP 1 " : "") + GetFieldsSelect(TableName, SqlProperties));
            return query;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, true, includes);
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, false, includes);
        }

        private string AppendJoinToSelect(SqlQuery originalBuilder, params Expression<Func<TEntity, object>>[] includes)
        {
            var joinBuilder = new StringBuilder();

            foreach (var include in includes)
            {
                var joinProperty = AllProperties.First(q => q.Name == ExpressionHelper.GetPropertyName(include));
                var tableName = GetTableNameOrAlias(joinProperty.DeclaringType);
                var attrJoin = joinProperty.GetCustomAttribute<JoinAttributeBase>();

                if (attrJoin == null)
                    continue;

                var joinString = "";
                if (attrJoin is LeftJoinAttribute)
                    joinString = "LEFT JOIN";
                else if (attrJoin is InnerJoinAttribute)
                    joinString = "INNER JOIN";
                else if (attrJoin is RightJoinAttribute)
                    joinString = "RIGHT JOIN";

                var joinType = joinProperty.PropertyType.IsGenericType() ? joinProperty.PropertyType.GenericTypeArguments[0] : joinProperty.PropertyType;
                var properties = joinType.FindClassProperties().Where(ExpressionHelper.GetPrimitivePropertiesPredicate());
                var props = properties.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();

                if (Config.UseQuotationMarks)
                    switch (Config.SqlConnector)
                    {
                        case ESqlConnector.MSSQL:
                            tableName = "[" + tableName + "]";
                            attrJoin.TableName = "[" + attrJoin.TableName + "]";
                            attrJoin.Key = "[" + attrJoin.Key + "]";
                            attrJoin.ExternalKey = "[" + attrJoin.ExternalKey + "]";
                            foreach (var prop in props)
                                prop.ColumnName = "[" + prop.ColumnName + "]";
                            break;

                        case ESqlConnector.MySQL:
                            tableName = "`" + tableName + "`";
                            attrJoin.TableName = "`" + attrJoin.TableName + "`";
                            attrJoin.Key = "`" + attrJoin.Key + "`";
                            attrJoin.ExternalKey = "`" + attrJoin.ExternalKey + "`";
                            foreach (var prop in props)
                                prop.ColumnName = "`" + prop.ColumnName + "`";
                            break;

                        case ESqlConnector.PostgreSQL:
                            tableName = "\"" + tableName + "\"";
                            attrJoin.TableName = "\"" + attrJoin.TableName + "\"";
                            attrJoin.Key = "\"" + attrJoin.Key + "\"";
                            attrJoin.ExternalKey = "\"" + attrJoin.ExternalKey + "\"";
                            foreach (var prop in props)
                                prop.ColumnName = "\"" + prop.ColumnName + "\"";
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(Config.SqlConnector));
                    }

                originalBuilder.SqlBuilder.Append(", " + GetFieldsSelect(attrJoin.TableName, props));
                joinBuilder.Append(joinString + " " + attrJoin.TableName + " ON " + tableName + "." + attrJoin.Key + " = " + attrJoin.TableName + "." + attrJoin.ExternalKey + " ");
            }
            return joinBuilder.ToString();
        }


        private static string GetFieldsSelect(string tableName, IEnumerable<SqlPropertyMetadata> properties)
        {
            //Projection function
            Func<SqlPropertyMetadata, string> projectionFunction = p => !string.IsNullOrEmpty(p.Alias)
                ? tableName + "." + p.ColumnName + " AS " + p.PropertyName
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
                    var columnName = SqlProperties.First(x => x.PropertyName == item.PropertyName).ColumnName;

                    if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                        sqlQuery.SqlBuilder.Append(item.LinkingOperator + " ");

                    if (item.PropertyValue == null)
                        sqlQuery.SqlBuilder.Append(TableName + "." + columnName + " " + (item.QueryOperator == "=" ? "IS" : "IS NOT") + " NULL ");
                    else
                        sqlQuery.SqlBuilder.Append(TableName + "." + columnName + " " + item.QueryOperator + " @" + item.PropertyName + " ");

                    dictionary[item.PropertyName] = item.PropertyValue;
                }

                if (LogicalDelete)
                    sqlQuery.SqlBuilder.Append("AND " + TableName + "." + StatusPropertyName + " != " + LogicalDeleteValue + " ");
            }
            else
            {
                if (LogicalDelete)
                    sqlQuery.SqlBuilder.Append("WHERE " + TableName + "." + StatusPropertyName + " != " + LogicalDeleteValue + " ");
            }

            if (firstOnly && (Config.SqlConnector == ESqlConnector.MySQL || Config.SqlConnector == ESqlConnector.PostgreSQL))
                sqlQuery.SqlBuilder.Append("LIMIT 1");

            sqlQuery.SetParam(dictionary);
            return sqlQuery;
        }

        private SqlQuery GetSelectById(object id, params Expression<Func<TEntity, object>>[] includes)
        {
            if (KeySqlProperties.Length != 1)
                throw new NotSupportedException("This method support only 1 key");

            var keyProperty = KeySqlProperties[0];

            var sqlQuery = InitBuilderSelect(true);

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

            IDictionary<string, object> dictionary = new Dictionary<string, object>
            {
                { keyProperty.ColumnName, id }
            };
            sqlQuery.SqlBuilder.Append("WHERE " + keyProperty.ColumnName + " = @" + keyProperty.PropertyName);

            if (Config.SqlConnector == ESqlConnector.MySQL || Config.SqlConnector == ESqlConnector.PostgreSQL)
                sqlQuery.SqlBuilder.Append("LIMIT 1");

            sqlQuery.SetParam(dictionary);
            return sqlQuery;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression = null)
        {
            var fieldName = ExpressionHelper.GetPropertyName(btwField);
            var columnName = SqlProperties.First(x => x.PropertyName == fieldName).ColumnName;
            var query = GetSelectAll(expression);

            query.SqlBuilder.Append((expression == null && !LogicalDelete ? "WHERE" : "AND") + " " + TableName + "." + columnName + " BETWEEN '" + from + "' AND '" + to + "'");

            return query;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetDelete(TEntity entity)
        {
            var sqlQuery = new SqlQuery(entity);
            var whereSql = " WHERE " + string.Join(" AND ", KeySqlProperties.Select(p => p.ColumnName + " = @" + p.PropertyName));
            if (!LogicalDelete)
            {
                sqlQuery.SqlBuilder.Append("DELETE FROM " + TableName + whereSql);
            }
            else
            {
                if (HasUpdatedAt)
                    UpdatedAtProperty.SetValue(entity, DateTime.UtcNow);
                sqlQuery.SqlBuilder.Append("UPDATE " + TableName + " SET " + StatusPropertyName + " = " + LogicalDeleteValue + whereSql);
            }

            return sqlQuery;
        }

        /// <summary>
        ///     Fill query properties
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="linkingType">Type of the linking.</param>
        /// <param name="queryProperties">The query properties.</param>
        private void FillQueryProperties(BinaryExpression body, ExpressionType linkingType, ref List<QueryParameter> queryProperties)
        {
            if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
            {
                var propertyName = ExpressionHelper.GetPropertyName(body);

                if (!SqlProperties.Select(x => x.PropertyName).Contains(propertyName))
                    throw new NotImplementedException("predicate can't parse");

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

        /// <inheritdoc />
        public virtual SqlQuery GetInsert(TEntity entity)
        {
            var properties = (IsIdentity ? SqlProperties.Where(p => !p.PropertyName.Equals(IdentitySqlProperty.PropertyName, StringComparison.OrdinalIgnoreCase)) : SqlProperties).ToList();

            if (HasUpdatedAt)
                UpdatedAtProperty.SetValue(entity, DateTime.UtcNow);

            var query = new SqlQuery(entity);

            query.SqlBuilder.Append(
                "INSERT INTO " + TableName
                + "(" + string.Join(", ", properties.Select(p => p.ColumnName)) + ")" // columNames
                + " VALUES  (" + string.Join(", ", properties.Select(p => "@" + p.PropertyName)) + ")"); // values

            if (IsIdentity)
                switch (Config.SqlConnector)
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

            return query;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetUpdate(TEntity entity)
        {
            var properties = SqlProperties.Where(p => !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)));

            if (HasUpdatedAt)
                UpdatedAtProperty.SetValue(entity, DateTime.UtcNow);

            var query = new SqlQuery(entity);
            query.SqlBuilder.Append("UPDATE " + TableName + " SET " + string.Join(", ", properties.Select(p => p.ColumnName + " = @" + p.PropertyName)) + " WHERE " + string.Join(" AND ", KeySqlProperties.Select(p => p.ColumnName + " = @" + p.PropertyName)));

            return query;
        }
    }
}