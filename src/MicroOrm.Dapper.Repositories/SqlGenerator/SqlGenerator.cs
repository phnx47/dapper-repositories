using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;
using MicroOrm.Dapper.Repositories.Extensions;
using MicroOrm.Dapper.Repositories.SqlGenerator.QueryExpressions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity> : ISqlGenerator<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        public SqlGenerator()
            : this(new SqlGeneratorConfig
            {
                SqlProvider = SqlProvider.MSSQL,
                UseQuotationMarks = false
            })
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        public SqlGenerator(SqlProvider sqlProvider, bool useQuotationMarks = false)
            : this(new SqlGeneratorConfig
            {
                SqlProvider = sqlProvider,
                UseQuotationMarks = useQuotationMarks
            })
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
        public virtual SqlQuery GetInsert(TEntity entity)
        {
            var properties =
                (IsIdentity
                    ? SqlProperties.Where(p => !p.PropertyName.Equals(IdentitySqlProperty.PropertyName, StringComparison.OrdinalIgnoreCase))
                    : SqlProperties).ToList();

            if (HasUpdatedAt)
                UpdatedAtProperty.SetValue(entity, DateTime.UtcNow);

            var query = new SqlQuery(entity);

            query.SqlBuilder.AppendFormat("INSERT INTO {0} ({1}) VALUES ({2})", TableName, string.Join(", ", properties.Select(p => p.ColumnName)),
                                            string.Join(", ", properties.Select(p => "@" + p.PropertyName))); // values

            if (IsIdentity)
                switch (Config.SqlProvider)
                {
                    case SqlProvider.MSSQL:
                        query.SqlBuilder.Append(" SELECT SCOPE_IDENTITY() AS " + IdentitySqlProperty.ColumnName);
                        break;

                    case SqlProvider.MySQL:
                        query.SqlBuilder.Append("; SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS " + IdentitySqlProperty.ColumnName);
                        break;

                    case SqlProvider.PostgreSQL:
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

            var properties =
                (IsIdentity
                    ? SqlProperties.Where(p => !p.PropertyName.Equals(IdentitySqlProperty.PropertyName, StringComparison.OrdinalIgnoreCase))
                    : SqlProperties).ToList();

            var query = new SqlQuery();

            var values = new List<string>();
            var parameters = new Dictionary<string, object>();

            for (var i = 0; i < entitiesArray.Length; i++)
            {
                if (HasUpdatedAt)
                    UpdatedAtProperty.SetValue(entitiesArray[i], DateTime.UtcNow);

                foreach (var property in properties)
                    // ReSharper disable once PossibleNullReferenceException
                    parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName).GetValue(entitiesArray[i], null));

                values.Add(string.Format("({0})", string.Join(", ", properties.Select(p => "@" + p.PropertyName + i))));
            }

            query.SqlBuilder.AppendFormat("INSERT INTO {0} ({1}) VALUES {2}", TableName, string.Join(", ", properties.Select(p => p.ColumnName)), string.Join(",", values)); // values

            query.SetParam(parameters);

            return query;
        }

      

        /// <inheritdoc />
        public virtual SqlQuery GetBulkUpdate(IEnumerable<TEntity> entities)
        {
            var entitiesArray = entities as TEntity[] ?? entities.ToArray();
            if (!entitiesArray.Any())
                throw new ArgumentException("collection is empty");

            var entityType = entitiesArray[0].GetType();

            var properties = SqlProperties.Where(p =>
                !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate).ToArray();

            var query = new SqlQuery();

            var parameters = new Dictionary<string, object>();

            for (var i = 0; i < entitiesArray.Length; i++)
            {
                if (HasUpdatedAt)
                    UpdatedAtProperty.SetValue(entitiesArray[i], DateTime.UtcNow);

                if (i > 0)
                    query.SqlBuilder.Append("; ");

                query.SqlBuilder.Append(string.Format("UPDATE {0} SET {1} WHERE {2}", TableName,
                                                        string.Join(", ", properties.Select(p => string.Format("{0} = @{1}{2}", p.ColumnName, p.PropertyName, i))),
                                                        string.Join(" AND ", KeySqlProperties.Where(p => !p.IgnoreUpdate)
                                                                                             .Select(p => string.Format("{0} = @{1}{2}", p.ColumnName, p.PropertyName, i)))
                                                    ));

                // ReSharper disable PossibleNullReferenceException
                foreach (var property in properties)
                    parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName).GetValue(entitiesArray[i], null));

                foreach (var property in KeySqlProperties.Where(p => !p.IgnoreUpdate))
                    parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName).GetValue(entitiesArray[i], null));

                // ReSharper restore PossibleNullReferenceException
            }

            query.SetParam(parameters);

            return query;
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
                var joinInnerProperties = propertyInfo.PropertyType.GetProperties().Where(q => q.CanWrite)
                    .Where(ExpressionHelper.GetPrimitivePropertiesPredicate()).ToArray();
                joinPropertyMetadatas.AddRange(joinInnerProperties.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any())
                    .Select(p => new SqlJoinPropertyMetadata(propertyInfo, p)).ToArray());
            }

            return joinPropertyMetadatas.ToArray();
        }

        private static string GetTableNameWithSchemaPrefix(string tableName, string tableSchema, string startQuotationMark = "", string endQuotationMark = "")
        {
            return !string.IsNullOrEmpty(tableSchema)
                ? startQuotationMark + tableSchema + endQuotationMark + "." + startQuotationMark + tableName + endQuotationMark
                : startQuotationMark + tableName + endQuotationMark;
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
                    switch (Config.SqlProvider)
                    {
                        case SqlProvider.MSSQL:
                            tableName = "[" + tableName + "]";
                            attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "[", "]");
                            attrJoin.Key = "[" + attrJoin.Key + "]";
                            attrJoin.ExternalKey = "[" + attrJoin.ExternalKey + "]";
                            attrJoin.TableAlias = "[" + attrJoin.TableAlias + "]";
                            foreach (var prop in props)
                                prop.ColumnName = "[" + prop.ColumnName + "]";
                            break;

                        case SqlProvider.MySQL:
                            tableName = "`" + tableName + "`";
                            attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "`", "`");
                            attrJoin.Key = "`" + attrJoin.Key + "`";
                            attrJoin.ExternalKey = "`" + attrJoin.ExternalKey + "`";
                            attrJoin.TableAlias = "`" + attrJoin.TableAlias + "`";
                            foreach (var prop in props)
                                prop.ColumnName = "`" + prop.ColumnName + "`";
                            break;

                        case SqlProvider.PostgreSQL:
                            tableName = "\"" + tableName + "\"";
                            attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "\"", "\"");
                            attrJoin.Key = "\"" + attrJoin.Key + "\"";
                            attrJoin.ExternalKey = "\"" + attrJoin.ExternalKey + "\"";
                            attrJoin.TableAlias = "\"" + attrJoin.TableAlias + "\"";
                            foreach (var prop in props)
                                prop.ColumnName = "\"" + prop.ColumnName + "\"";
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(Config.SqlProvider));
                    }
                else
                    attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema);

                originalBuilder.SqlBuilder.Append($", {GetFieldsSelect(attrJoin.TableAlias, props)}");
                joinBuilder.Append(
                    $"{joinString} {attrJoin.TableName} AS {attrJoin.TableAlias} ON {tableName}.{attrJoin.Key} = {attrJoin.TableAlias}.{attrJoin.ExternalKey} ");
            }

            return joinBuilder.ToString();
        }

        private enum QueryType
        {
            Select,
            Delete,
            Update
        }
    }
}
