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
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.Extensions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity> : ISqlGenerator<TEntity>
        where TEntity : class
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public SqlGenerator()
        {
            Provider = MicroOrmConfig.SqlProvider;
            
            if (UseQuotationMarks == null)
                UseQuotationMarks = Provider != SqlProvider.SQLite && MicroOrmConfig.UseQuotationMarks;
            
            Initialize();
        }

        private void Initialize()
        {
            // Order is important
            InitProperties();
            InitConfig();
            InitLogicalDeleted();
        }

        /// <summary>
        /// Constructor with params
        /// </summary>
        public SqlGenerator(SqlProvider provider, bool useQuotationMarks)
        {
            Provider = provider;
            UseQuotationMarks = provider != SqlProvider.SQLite && useQuotationMarks;
            Initialize();
        }

        /// <summary>
        /// Constructor with params
        /// </summary>
        public SqlGenerator(SqlProvider provider)
        {
            Provider = provider;
            UseQuotationMarks = false;
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public SqlProvider Provider { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool? UseQuotationMarks { get; set; }

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
        public bool LogicalDelete { get; protected set; }

        /// <inheritdoc />
        public Dictionary<string, PropertyInfo> JoinsLogicalDelete { get; protected set; }

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
            {
                var attribute = UpdatedAtProperty.GetCustomAttribute<UpdatedAtAttribute>();
                var offset = attribute.TimeKind == DateTimeKind.Local
                    ? new DateTimeOffset(DateTime.Now)
                    : new DateTimeOffset(DateTime.UtcNow);
                if (attribute.OffSet != 0)
                {
                    offset = offset.ToOffset(TimeSpan.FromHours(attribute.OffSet));
                }

                UpdatedAtProperty.SetValue(entity, offset.DateTime);
            }

            var query = new SqlQuery(entity);

            query.SqlBuilder.AppendFormat("INSERT INTO {0} ({1}) VALUES ({2})", TableName, string.Join(", ", properties.Select(p => p.ColumnName)),
                string.Join(", ", properties.Select(p => "@" + p.PropertyName))); // values

            if (IsIdentity)
                switch (Provider)
                {
                    case SqlProvider.MSSQL:
                        query.SqlBuilder.Append(" SELECT SCOPE_IDENTITY() AS " + IdentitySqlProperty.ColumnName);
                        break;

                    case SqlProvider.MySQL:
                        query.SqlBuilder.Append("; SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS " + IdentitySqlProperty.ColumnName);
                        break;

                    case SqlProvider.SQLite:
                        query.SqlBuilder.Append("; SELECT LAST_INSERT_ROWID() AS " + IdentitySqlProperty.ColumnName);
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
                var entity = entitiesArray[i];
                if (HasUpdatedAt)
                {
                    var attribute = UpdatedAtProperty.GetCustomAttribute<UpdatedAtAttribute>();
                    var offset = attribute.TimeKind == DateTimeKind.Local
                        ? new DateTimeOffset(DateTime.Now)
                        : new DateTimeOffset(DateTime.UtcNow);
                    if (attribute.OffSet != 0)
                    {
                        offset = offset.ToOffset(TimeSpan.FromHours(attribute.OffSet));
                    }

                    UpdatedAtProperty.SetValue(entity, offset.DateTime);
                }

                foreach (var property in properties)
                    // ReSharper disable once PossibleNullReferenceException
                    parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName).GetValue(entity, null));

                values.Add(string.Format("({0})", string.Join(", ", properties.Select(p => "@" + p.PropertyName + i))));
            }

            query.SqlBuilder.AppendFormat("INSERT INTO {0} ({1}) VALUES {2}", TableName, string.Join(", ", properties.Select(p => p.ColumnName)),
                string.Join(",", values)); // values

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
                var entity = entitiesArray[i];
                if (HasUpdatedAt)
                {
                    var attribute = UpdatedAtProperty.GetCustomAttribute<UpdatedAtAttribute>();
                    var offset = attribute.TimeKind == DateTimeKind.Local
                        ? new DateTimeOffset(DateTime.Now)
                        : new DateTimeOffset(DateTime.UtcNow);
                    if (attribute.OffSet != 0)
                    {
                        offset = offset.ToOffset(TimeSpan.FromHours(attribute.OffSet));
                    }

                    UpdatedAtProperty.SetValue(entity, offset.DateTime);
                }

                if (i > 0)
                    query.SqlBuilder.Append("; ");

                query.SqlBuilder.Append(
                    $"UPDATE {TableName} SET {string.Join(", ", properties.Select(p => $"{p.ColumnName} = @{p.PropertyName}{i}"))} WHERE {string.Join(" AND ", KeySqlProperties.Where(p => !p.IgnoreUpdate).Select(p => $"{p.ColumnName} = @{p.PropertyName}{i}"))}");

                // ReSharper disable PossibleNullReferenceException
                foreach (var property in properties)
                    parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName).GetValue(entity, null));

                foreach (var property in KeySqlProperties.Where(p => !p.IgnoreUpdate))
                    parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName).GetValue(entity, null));

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
                    .Where(ExpressionHelper.GetPrimitivePropertiesPredicate());
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

        private string GetTableNameWithQuotes(JoinAttributeBase attrJoin, SqlPropertyMetadata[] props, string tableName)
        {
            switch (Provider)
            {
                case SqlProvider.MSSQL:
                    tableName = "[" + tableName + "]";
                    attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "[", "]");
                    attrJoin.Key = "[" + attrJoin.Key + "]";
                    attrJoin.ExternalKey = "[" + attrJoin.ExternalKey + "]";
                    attrJoin.TableAlias = string.IsNullOrEmpty(attrJoin.TableAlias) ? string.Empty : "[" + attrJoin.TableAlias + "]";
                    foreach (var prop in props)
                    {
                        prop.ColumnName = "[" + prop.CleanColumnName + "]";
                    }

                    break;

                case SqlProvider.MySQL:
                    tableName = "`" + tableName + "`";
                    attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "`", "`");
                    attrJoin.Key = "`" + attrJoin.Key + "`";
                    attrJoin.ExternalKey = "`" + attrJoin.ExternalKey + "`";
                    attrJoin.TableAlias = string.IsNullOrEmpty(attrJoin.TableAlias) ? string.Empty : "`" + attrJoin.TableAlias + "`";
                    foreach (var prop in props)
                    {
                        prop.ColumnName = "`" + prop.CleanColumnName + "`";
                    }

                    break;

                case SqlProvider.SQLite:
                    break;

                case SqlProvider.PostgreSQL:
                    tableName = "\"" + tableName + "\"";
                    attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "\"", "\"");
                    attrJoin.Key = "\"" + attrJoin.Key + "\"";
                    attrJoin.ExternalKey = "\"" + attrJoin.ExternalKey + "\"";
                    attrJoin.TableAlias = string.IsNullOrEmpty(attrJoin.TableAlias) ? string.Empty : "\"" + attrJoin.TableAlias + "\"";
                    foreach (var prop in props)
                    {
                        prop.ColumnName = "\"" + prop.CleanColumnName + "\"";
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Provider));
            }

            return tableName;
        }

        private string AppendJoinToUpdate<TBase>(TBase entity, SqlQuery originalBuilder, params Expression<Func<TEntity, object>>[] includes)
        {
            var joinBuilder = new StringBuilder();

            foreach (var include in includes)
            {
                var joinProperty = AllProperties.First(q => q.Name == ExpressionHelper.GetPropertyName(include));
                var attrJoin = joinProperty.GetCustomAttribute<JoinAttributeBase>();

                if (attrJoin == null)
                    continue;

                var declaringType = joinProperty.ReflectedType.GetTypeInfo();
                var tableAttribute = declaringType.GetCustomAttribute<TableAttribute>();
                var tableName = MicroOrmConfig.TablePrefix + (tableAttribute != null ? tableAttribute.Name : declaringType.Name);

                var joinType = joinProperty.PropertyType.IsGenericType ? joinProperty.PropertyType.GenericTypeArguments[0] : joinProperty.PropertyType;
                var properties = joinType.FindClassMetaDataProperties().Where(p=> !p.IgnoreUpdate).ToArray();

                var joinEntity = entity.GetType().GetProperty(joinProperty.Name)?.GetValue(entity, null);
                if (joinEntity == null)
                    return string.Empty;

                var dict = properties.ToDictionary(prop => $"{prop.PropertyInfo.ReflectedType?.Name}{prop.PropertyName}",
                    prop => joinType.GetProperty(prop.PropertyName)?.GetValue(joinEntity, null));
                originalBuilder.SetParam(dict);

                if (UseQuotationMarks == true)
                {
                    tableName = GetTableNameWithQuotes(attrJoin, properties, tableName);
                }
                else
                    attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema);

                joinBuilder.Append($", {GetFieldsUpdate(string.IsNullOrEmpty(attrJoin.TableAlias) ? attrJoin.TableName : attrJoin.TableAlias, properties, UseQuotationMarks == true)}");
                AppendJoinQuery(attrJoin, originalBuilder.SqlBuilder, tableName);
            }

            return joinBuilder.ToString();
        }

        private void AppendJoinQuery(JoinAttributeBase attrJoin, StringBuilder joinBuilder, string tableName)
        {
            var joinString = attrJoin.ToString();
            if (attrJoin is CrossJoinAttribute)
            {
                joinBuilder.Append(attrJoin.TableAlias == string.Empty
                    ? $"{joinString} {attrJoin.TableName} "
                    : $"{joinString} {attrJoin.TableName} AS {attrJoin.TableAlias} ");
            }
            else
            {
                var customFilter = string.Empty;
                if (JoinsLogicalDelete != null && JoinsLogicalDelete.TryGetValue(attrJoin.TableName, out var deleteAttr))
                {
                    var colAttr = deleteAttr.GetCustomAttribute<ColumnAttribute>();
                    var colName = colAttr == null ? deleteAttr.Name : colAttr.Name;
                    object deleteValue = 1;
                    if (deleteAttr.PropertyType.IsEnum)
                    {
                        var deleteOption = deleteAttr.PropertyType.GetFields().FirstOrDefault(f => f.GetCustomAttribute<DeletedAttribute>() != null);

                        if (deleteOption != null)
                        {
                            var enumValue = Enum.Parse(deleteAttr.PropertyType, deleteOption.Name);
                            deleteValue = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(deleteAttr.PropertyType));
                        }
                    }

                    customFilter = attrJoin.TableAlias == string.Empty
                        ? $"AND {attrJoin.TableName}.{colName} != {deleteValue} "
                        : $"AND {attrJoin.TableAlias}.{colName} != {deleteValue} ";
                }

                joinBuilder.Append(attrJoin.TableAlias == string.Empty
                    ? $"{joinString} {attrJoin.TableName} ON {tableName}.{attrJoin.Key} = {attrJoin.TableName}.{attrJoin.ExternalKey} {customFilter}"
                    : $"{joinString} {attrJoin.TableName} AS {attrJoin.TableAlias} ON {tableName}.{attrJoin.Key} = {attrJoin.TableAlias}.{attrJoin.ExternalKey} {customFilter}");
            }
        }

        private string AppendJoinToSelect(SqlQuery originalBuilder, bool hasSelectFilter, params Expression<Func<TEntity, object>>[] includes)
        {
            var joinBuilder = new StringBuilder();

            foreach (var include in includes)
            {
                var joinProperty = AllProperties.First(q => q.Name == ExpressionHelper.GetPropertyName(include));
                var attrJoin = joinProperty.GetCustomAttribute<JoinAttributeBase>();

                if (attrJoin == null)
                    continue;

                var declaringType = joinProperty.ReflectedType.GetTypeInfo();
                var tableAttribute = declaringType.GetCustomAttribute<TableAttribute>();
                var tableName = MicroOrmConfig.TablePrefix + (tableAttribute != null ? tableAttribute.Name : declaringType.Name);

                var joinType = joinProperty.PropertyType.IsGenericType ? joinProperty.PropertyType.GenericTypeArguments[0] : joinProperty.PropertyType;
                var properties = joinType.FindClassMetaDataProperties();
                
                if (UseQuotationMarks == true)
                {
                    tableName = GetTableNameWithQuotes(attrJoin, properties, tableName);
                }
                else
                    attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema);

                if (!hasSelectFilter)
                    originalBuilder.SqlBuilder.Append($", {GetFieldsSelect(string.IsNullOrEmpty(attrJoin.TableAlias) ? attrJoin.TableName : attrJoin.TableAlias, properties, UseQuotationMarks == true)}");

                AppendJoinQuery(attrJoin, joinBuilder, tableName);
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
