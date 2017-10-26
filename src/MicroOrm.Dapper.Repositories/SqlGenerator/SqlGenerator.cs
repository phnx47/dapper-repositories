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

        /// <inheritdoc />
        public PropertyInfo[] AllProperties { get; protected set; }

        /// <inheritdoc />
        public bool HasUpdatedAt => UpdatedAtProperty != null;

        /// <inheritdoc />
        public PropertyInfo UpdatedAtProperty { get; protected set; }

        /// <inheritdoc />
        public SqlPropertyMetadata UpdatedAtPropertyMetadata { get; protected set; }

        /// <inheritdoc />
        public bool IsIdentity => IdentitySqlProperty != null;

        /// <inheritdoc />
        public string TableName { get; protected set; }

        /// <inheritdoc />
        public string TableSchema { get; protected set; }

        /// <inheritdoc />
        public SqlPropertyMetadata IdentitySqlProperty { get; protected set; }

        /// <inheritdoc />
        public SqlPropertyMetadata[] KeySqlProperties { get; protected set; }

        /// <inheritdoc />
        public SqlPropertyMetadata[] SqlProperties { get; protected set; }

        /// <inheritdoc />
        public SqlJoinPropertyMetadata[] SqlJoinProperties { get; protected set; }

        /// <inheritdoc />
        public SqlGeneratorConfig Config { get; protected set; }

        /// <inheritdoc />
        public bool LogicalDelete { get; protected set; }

        /// <inheritdoc />
        public string StatusPropertyName { get; protected set; }

        /// <inheritdoc />
        public object LogicalDeleteValue { get; protected set; }

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

        /// <inheritdoc />
        public SqlQuery GetSelectById(object id, params Expression<Func<TEntity, object>>[] includes)
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
                { keyProperty.PropertyName, id }
            };
            sqlQuery.SqlBuilder.Append("WHERE " + TableName + "." + keyProperty.ColumnName + " = @" + keyProperty.PropertyName + " ");

            if (LogicalDelete)
                sqlQuery.SqlBuilder.Append("AND " + TableName + "." + StatusPropertyName + " != " + LogicalDeleteValue + " ");

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
            var sqlQuery = new SqlQuery();
            var whereSql = " WHERE " + string.Join(" AND ", KeySqlProperties.Select(p => TableName + "." + p.ColumnName + " = @" + p.PropertyName));

            if (!LogicalDelete)
            {
                sqlQuery.SqlBuilder.Append("DELETE FROM " + TableName + whereSql);
            }
            else
            {
                sqlQuery.SqlBuilder.Append("UPDATE " + TableName + " SET " + StatusPropertyName + " = " + LogicalDeleteValue);

                if (HasUpdatedAt)
                {
                    UpdatedAtProperty.SetValue(entity, DateTime.UtcNow);
                    sqlQuery.SqlBuilder.Append(", " + UpdatedAtPropertyMetadata.ColumnName + " = @" + UpdatedAtPropertyMetadata.PropertyName);
                }

                sqlQuery.SqlBuilder.Append(whereSql);
            }

            sqlQuery.SetParam(entity);
            return sqlQuery;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetDelete(Expression<Func<TEntity, bool>> predicate)
        {
            var sqlQuery = new SqlQuery();

            if (!LogicalDelete)
            {
                sqlQuery.SqlBuilder.Append("DELETE FROM " + TableName + " ");
            }
            else
            {
                sqlQuery.SqlBuilder.Append("UPDATE " + TableName + " SET " + StatusPropertyName + " = " + LogicalDeleteValue);
                sqlQuery.SqlBuilder.Append(HasUpdatedAt ? ", " + UpdatedAtPropertyMetadata.ColumnName + " = @" + UpdatedAtPropertyMetadata.PropertyName + " " : " ");
            }

            AppendWherePredicateQuery(sqlQuery, predicate, QueryType.Delete);
            return sqlQuery;
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
                + " (" + string.Join(", ", properties.Select(p => p.ColumnName)) + ")" // columNames
                + " VALUES (" + string.Join(", ", properties.Select(p => "@" + p.PropertyName)) + ")"); // values

            if (IsIdentity)
                switch (Config.SqlConnector)
                {
                    case ESqlConnector.MSSQL:
                        query.SqlBuilder.Append(" SELECT SCOPE_IDENTITY() AS " + IdentitySqlProperty.ColumnName);
                        break;

                    case ESqlConnector.MySQL:
                        query.SqlBuilder.Append("; SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS " + IdentitySqlProperty.ColumnName);
                        break;

                    case ESqlConnector.PostgreSQL:
                        query.SqlBuilder.Append(" RETURNING " + IdentitySqlProperty.ColumnName);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

            return query;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetBulkInsert(IEnumerable<TEntity> entities)
        {
            var entitiesArray = entities as TEntity[] ?? entities.ToArray();
            if (!entitiesArray.Any())
                throw new ArgumentException("collection is empty");

            var entityType = entitiesArray[0].GetType();

            var properties = (IsIdentity ? SqlProperties.Where(p => !p.PropertyName.Equals(IdentitySqlProperty.PropertyName, StringComparison.OrdinalIgnoreCase)) : SqlProperties).ToList();

            var query = new SqlQuery();

            var values = new List<string>();
            var parameters = new Dictionary<string, object>();

            for (var i = 0; i < entitiesArray.Length; i++)
            {
                if (HasUpdatedAt)
                    UpdatedAtProperty.SetValue(entitiesArray[i], DateTime.UtcNow);

                foreach (var property in properties)
                    parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName).GetValue(entitiesArray[i], null));

                values.Add("(" + string.Join(", ", properties.Select(p => "@" + p.PropertyName + i)) + ")");
            }

            query.SqlBuilder.Append(
                "INSERT INTO " + TableName
                + " (" + string.Join(", ", properties.Select(p => p.ColumnName)) + ")" // columNames
                + " VALUES " + string.Join(",", values)); // values

            query.SetParam(parameters);

            return query;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetUpdate(TEntity entity)
        {
            var properties = SqlProperties.Where(p => !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate).ToArray();
            if (!properties.Any())
                throw new ArgumentException("Can't update without [Key]");

            if (HasUpdatedAt)
                UpdatedAtProperty.SetValue(entity, DateTime.UtcNow);

            var query = new SqlQuery(entity);
            query.SqlBuilder.Append("UPDATE " + TableName + " SET " + string.Join(", ", properties.Select(p => p.ColumnName + " = @" + p.PropertyName))
                + " WHERE " + string.Join(" AND ", KeySqlProperties.Where(p => !p.IgnoreUpdate).Select(p => p.ColumnName + " = @" + p.PropertyName)));

            return query;
        }

        
        /// <inheritdoc />
        public virtual SqlQuery GetUpdate(Expression<Func<TEntity, bool>> predicate, TEntity entity)
        {
            var properties = SqlProperties.Where(p => !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate);

            if (HasUpdatedAt)
                UpdatedAtProperty.SetValue(entity, DateTime.UtcNow);

            var query = new SqlQuery(entity);
            query.SqlBuilder.Append("UPDATE " + TableName + " SET " + string.Join(", ", properties.Select(p => p.ColumnName + " = @" + p.PropertyName)) + " ");
            AppendWherePredicateQuery(query, predicate, QueryType.Update);

            return query;
        }
        

        /// <inheritdoc />
        public virtual SqlQuery GetBulkUpdate(IEnumerable<TEntity> entities)
        {
            var entitiesArray = entities as TEntity[] ?? entities.ToArray();
            if (!entitiesArray.Any())
                throw new ArgumentException("collection is empty");

            var entityType = entitiesArray[0].GetType();

            var properties = SqlProperties.Where(p => !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate).ToArray();

            var query = new SqlQuery();

            var parameters = new Dictionary<string, object>();

            for (var i = 0; i < entitiesArray.Length; i++)
            {
                if (HasUpdatedAt)
                    UpdatedAtProperty.SetValue(entitiesArray[i], DateTime.UtcNow);

                if (i > 0)
                    query.SqlBuilder.Append("; ");
                
                query.SqlBuilder.Append("UPDATE " + TableName + " SET " + string.Join(", ", properties.Select(p => p.ColumnName + " = @" + p.PropertyName + i))
                    + " WHERE " + string.Join(" AND ", KeySqlProperties.Where(p => !p.IgnoreUpdate).Select(p => p.ColumnName + " = @" + p.PropertyName + i)));

                foreach (var property in properties)
                {
                    parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName).GetValue(entitiesArray[i], null));
                }

                foreach (var property in KeySqlProperties.Where(p => !p.IgnoreUpdate))
                {
                    parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName).GetValue(entitiesArray[i], null));
                }
            }

            query.SetParam(parameters);

            return query;
        }

        private void AppendWherePredicateQuery(SqlQuery sqlQuery, Expression<Func<TEntity, bool>> predicate, QueryType queryType)
        {
            IDictionary<string, object> dictionaryParams = new Dictionary<string, object>();

            if (predicate != null)
            {
                // WHERE
                var queryProperties = new List<QueryParameter>();
                FillQueryProperties(predicate.Body, ExpressionType.Default, ref queryProperties);

                sqlQuery.SqlBuilder.Append("WHERE ");

                for (var i = 0; i < queryProperties.Count; i++)
                {
                    var item = queryProperties[i];
                    var tableName = TableName;
                    string columnName;
                    if (item.NestedProperty)
                    {
                        var joinProperty = SqlJoinProperties.First(x => x.PropertyName == item.PropertyName);
                        tableName = joinProperty.TableName;
                        columnName = joinProperty.ColumnName;
                    }
                    else
                    {
                        columnName = SqlProperties.First(x => x.PropertyName == item.PropertyName).ColumnName;
                    }

                    if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                        sqlQuery.SqlBuilder.Append(item.LinkingOperator + " ");

                    if (item.PropertyValue == null)
                        sqlQuery.SqlBuilder.Append(tableName + "." + columnName + " " + (item.QueryOperator == "=" ? "IS" : "IS NOT") + " NULL ");
                    else
                        sqlQuery.SqlBuilder.Append(tableName + "." + columnName + " " + item.QueryOperator + " @" + item.PropertyName + " ");


                    dictionaryParams[item.PropertyName] = item.PropertyValue;
                }

                if (LogicalDelete && queryType == QueryType.Select)
                    sqlQuery.SqlBuilder.Append("AND " + TableName + "." + StatusPropertyName + " != " + LogicalDeleteValue + " ");
            }
            else
            {
                if (LogicalDelete && queryType == QueryType.Select)
                    sqlQuery.SqlBuilder.Append("WHERE " + TableName + "." + StatusPropertyName + " != " + LogicalDeleteValue + " ");
            }

            if (LogicalDelete && HasUpdatedAt && queryType == QueryType.Delete)
                dictionaryParams.Add(UpdatedAtPropertyMetadata.ColumnName, DateTime.UtcNow);

            sqlQuery.SetParam(dictionaryParams);
        }

        private void InitProperties()
        {
            var entityType = typeof(TEntity);
            var entityTypeInfo = entityType.GetTypeInfo();
            var tableAttribute = entityTypeInfo.GetCustomAttribute<TableAttribute>();

            TableName = tableAttribute != null ? tableAttribute.Name : entityTypeInfo.Name;
            TableSchema = tableAttribute != null ? tableAttribute.Schema : string.Empty;

            AllProperties = entityType.FindClassProperties().Where(q => q.CanWrite).ToArray();

            var props = AllProperties.Where(ExpressionHelper.GetPrimitivePropertiesPredicate()).ToArray();

            var joinProperties = AllProperties.Where(p => p.GetCustomAttributes<JoinAttributeBase>().Any()).ToArray();

            SqlJoinProperties = GetJoinPropertyMetadata(joinProperties);

            // Filter the non stored properties
            SqlProperties = props.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();

            // Filter key properties
            KeySqlProperties = props.Where(p => p.GetCustomAttributes<KeyAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();

            // Use identity as key pattern
            var identityProperty = props.FirstOrDefault(p => p.GetCustomAttributes<IdentityAttribute>().Any());
            IdentitySqlProperty = identityProperty != null ? new SqlPropertyMetadata(identityProperty) : null;

            var dateChangedProperty = props.FirstOrDefault(p => p.GetCustomAttributes<UpdatedAtAttribute>().Count() == 1);
            if (dateChangedProperty != null && (dateChangedProperty.PropertyType == typeof(DateTime) || dateChangedProperty.PropertyType == typeof(DateTime?)))
            {
                UpdatedAtProperty = props.FirstOrDefault(p => p.GetCustomAttributes<UpdatedAtAttribute>().Any());
                UpdatedAtPropertyMetadata = new SqlPropertyMetadata(UpdatedAtProperty);
            }

        }

        /// <summary>
        ///     Get join/nested properties
        /// </summary>
        /// <returns></returns>
        private static SqlJoinPropertyMetadata[] GetJoinPropertyMetadata(PropertyInfo[] joinPropertiesInfo)
        {
            // Filter and get only non collection nested properties
            var singleJoinTypes = joinPropertiesInfo.Where(p => !p.PropertyType.IsConstructedGenericType).ToArray();

            var joinPropertyMetadatas = new List<SqlJoinPropertyMetadata>();

            foreach (var propertyInfo in singleJoinTypes)
            {
                var joinInnerProperties = propertyInfo.PropertyType.GetProperties().Where(q => q.CanWrite).Where(ExpressionHelper.GetPrimitivePropertiesPredicate()).ToArray();

                joinPropertyMetadatas.AddRange(joinInnerProperties.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new SqlJoinPropertyMetadata(propertyInfo, p)).ToArray());
            }

            return joinPropertyMetadatas.ToArray();
        }

        /// <summary>
        ///     Init type Sql provider
        /// </summary>
        private void InitConfig(SqlGeneratorConfig sqlGeneratorConfig)
        {
            Config = sqlGeneratorConfig;

            if (Config.UseQuotationMarks)
            {
                switch (Config.SqlConnector)
                {
                    case ESqlConnector.MSSQL:
                        TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema, "[", "]");

                        foreach (var propertyMetadata in SqlProperties)
                            propertyMetadata.ColumnName = "[" + propertyMetadata.ColumnName + "]";

                        foreach (var propertyMetadata in KeySqlProperties)
                            propertyMetadata.ColumnName = "[" + propertyMetadata.ColumnName + "]";

                        foreach (var propertyMetadata in SqlJoinProperties)
                        {
                            propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema, "[", "]");
                            propertyMetadata.ColumnName = "[" + propertyMetadata.ColumnName + "]";
                        }

                        if (IdentitySqlProperty != null)
                            IdentitySqlProperty.ColumnName = "[" + IdentitySqlProperty.ColumnName + "]";

                        break;

                    case ESqlConnector.MySQL:
                        TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema, "`", "`");

                        foreach (var propertyMetadata in SqlProperties)
                            propertyMetadata.ColumnName = "`" + propertyMetadata.ColumnName + "`";

                        foreach (var propertyMetadata in KeySqlProperties)
                            propertyMetadata.ColumnName = "`" + propertyMetadata.ColumnName + "`";

                        foreach (var propertyMetadata in SqlJoinProperties)
                        {
                            propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema, "`", "`");
                            propertyMetadata.ColumnName = "`" + propertyMetadata.ColumnName + "`";
                        }

                        if (IdentitySqlProperty != null)
                            IdentitySqlProperty.ColumnName = "`" + IdentitySqlProperty.ColumnName + "`";

                        break;

                    case ESqlConnector.PostgreSQL:
                        TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema, "\"", "\"");

                        foreach (var propertyMetadata in SqlProperties)
                            propertyMetadata.ColumnName = "\"" + propertyMetadata.ColumnName + "\"";

                        foreach (var propertyMetadata in KeySqlProperties)
                            propertyMetadata.ColumnName = "\"" + propertyMetadata.ColumnName + "\"";

                        foreach (var propertyMetadata in SqlJoinProperties)
                        {
                            propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema, "\"", "\"");
                            propertyMetadata.ColumnName = "\"" + propertyMetadata.ColumnName + "\"";
                        }

                        if (IdentitySqlProperty != null)
                            IdentitySqlProperty.ColumnName = "\"" + IdentitySqlProperty.ColumnName + "\"";

                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(Config.SqlConnector));
                }
            }
            else
            {
                TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema);
                foreach (var propertyMetadata in SqlJoinProperties)
                    propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema);
            }
        }

        private static string GetTableNameWithSchemaPrefix(string tableName, string tableSchema, string startQuotationMark = "", string endQuotationMark = "")
        {
            return !string.IsNullOrEmpty(tableSchema)
                ? startQuotationMark + tableSchema + endQuotationMark + "." + startQuotationMark + tableName + endQuotationMark
                : startQuotationMark + tableName + endQuotationMark;
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

        private string AppendJoinToSelect(SqlQuery originalBuilder, params Expression<Func<TEntity, object>>[] includes)
        {
            var joinBuilder = new StringBuilder();

            foreach (var include in includes)
            {
                var joinProperty = AllProperties.First(q => q.Name == ExpressionHelper.GetPropertyName(include));
                var declaringType = joinProperty.DeclaringType.GetTypeInfo();
                var tableAttribute = declaringType.GetCustomAttribute<TableAttribute>();
                var tableName = tableAttribute != null ? tableAttribute.Name : declaringType.Name;

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
                            attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "[", "]");
                            attrJoin.Key = "[" + attrJoin.Key + "]";
                            attrJoin.ExternalKey = "[" + attrJoin.ExternalKey + "]";
                            foreach (var prop in props)
                                prop.ColumnName = "[" + prop.ColumnName + "]";
                            break;

                        case ESqlConnector.MySQL:
                            tableName = "`" + tableName + "`";
                            attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "`", "`");
                            attrJoin.Key = "`" + attrJoin.Key + "`";
                            attrJoin.ExternalKey = "`" + attrJoin.ExternalKey + "`";
                            foreach (var prop in props)
                                prop.ColumnName = "`" + prop.ColumnName + "`";
                            break;

                        case ESqlConnector.PostgreSQL:
                            tableName = "\"" + tableName + "\"";
                            attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "\"", "\"");
                            attrJoin.Key = "\"" + attrJoin.Key + "\"";
                            attrJoin.ExternalKey = "\"" + attrJoin.ExternalKey + "\"";
                            foreach (var prop in props)
                                prop.ColumnName = "\"" + prop.ColumnName + "\"";
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(Config.SqlConnector));
                    }
                else
                    attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema);

                originalBuilder.SqlBuilder.Append(", " + GetFieldsSelect(attrJoin.TableName, props));
                joinBuilder.Append(joinString + " " + attrJoin.TableName + " ON " + tableName + "." + attrJoin.Key + " = " + attrJoin.TableName + "." + attrJoin.ExternalKey + " ");
            }
            return joinBuilder.ToString();
        }


        private static string GetFieldsSelect(string tableName, IEnumerable<SqlPropertyMetadata> properties)
        {
            //Projection function
            string ProjectionFunction(SqlPropertyMetadata p)
            {
                return !string.IsNullOrEmpty(p.Alias)
                    ? tableName + "." + p.ColumnName + " AS " + p.PropertyName
                    : tableName + "." + p.ColumnName;
            }

            return string.Join(", ", properties.Select(ProjectionFunction));
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

            AppendWherePredicateQuery(sqlQuery, predicate, QueryType.Select);

            if (firstOnly && (Config.SqlConnector == ESqlConnector.MySQL || Config.SqlConnector == ESqlConnector.PostgreSQL))
                sqlQuery.SqlBuilder.Append("LIMIT 1");

            return sqlQuery;
        }

        /// <summary>
        ///     Fill query properties
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <param name="linkingType">Type of the linking.</param>
        /// <param name="queryProperties">The query properties.</param>
        private void FillQueryProperties(Expression expr, ExpressionType linkingType, ref List<QueryParameter> queryProperties)
        {
            var body = expr as MethodCallExpression;
            if (body != null)
            {
                var innerBody = body;
                var methodName = innerBody.Method.Name;
                switch (methodName)
                {
                    case "Contains":
                        {
                            var propertyName = ExpressionHelper.GetPropertyNamePath(innerBody, out bool isNested);

                            if (!SqlProperties.Select(x => x.PropertyName).Contains(propertyName) && !SqlJoinProperties.Select(x => x.PropertyName).Contains(propertyName))
                                throw new NotImplementedException("predicate can't parse");

                            var propertyValue = ExpressionHelper.GetValuesFromCollection(innerBody);
                            var opr = ExpressionHelper.GetMethodCallSqlOperator(methodName);
                            var link = ExpressionHelper.GetSqlOperator(linkingType);
                            queryProperties.Add(new QueryParameter(link, propertyName, propertyValue, opr, isNested));
                            break;
                        }

                    default:
                        throw new NotImplementedException($"'{methodName}' method is not implemented");
                }
            }
            else if (expr is BinaryExpression)
            {
                var innerbody = (BinaryExpression)expr;
                if (innerbody.NodeType != ExpressionType.AndAlso && innerbody.NodeType != ExpressionType.OrElse)
                {
                    var propertyName = ExpressionHelper.GetPropertyNamePath(innerbody, out bool isNested);

                    if (!SqlProperties.Select(x => x.PropertyName).Contains(propertyName) && !SqlJoinProperties.Select(x => x.PropertyName).Contains(propertyName))
                        throw new NotImplementedException("predicate can't parse");

                    var propertyValue = ExpressionHelper.GetValue(innerbody.Right);
                    var opr = ExpressionHelper.GetSqlOperator(innerbody.NodeType);
                    var link = ExpressionHelper.GetSqlOperator(linkingType);

                    queryProperties.Add(new QueryParameter(link, propertyName, propertyValue, opr, isNested));
                }
                else
                {
                    FillQueryProperties(innerbody.Left, innerbody.NodeType, ref queryProperties);
                    FillQueryProperties(innerbody.Right, innerbody.NodeType, ref queryProperties);
                }
            }
            else
            {
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(expr), linkingType, ref queryProperties);
            }
        }

        private enum QueryType
        {
            Select,
            Delete,
            Update
        }
    }
}