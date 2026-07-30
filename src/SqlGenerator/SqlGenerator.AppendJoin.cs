using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.Extensions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator;

public partial class SqlGenerator<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Resolve an "includes" expression to the join property it names
    /// </summary>
    private PropertyInfo GetJoinProperty(Expression<Func<TEntity, object>> include, string paramName, out JoinAttributeBase attribute)
    {
        var propertyName = ExpressionHelper.GetPropertyName(include);

        var property = AllProperties.FirstOrDefault(q => q.Name == propertyName)
            ?? throw new ArgumentException($"Can't join [{propertyName}]: not a writable property of {typeof(TEntity).Name}", paramName);

        attribute = property.GetCustomAttribute<JoinAttributeBase>()
            ?? throw new ArgumentException(
                $"Can't join [{propertyName}]: property isn't marked with [LeftJoin], [InnerJoin], [RightJoin] or [CrossJoin]", paramName);

        return property;
    }

    private string AppendJoinToUpdate<TBase>(TBase entity, SqlQuery originalBuilder, params Expression<Func<TEntity, object>>[] includes) where TBase : notnull
    {
        var joinBuilder = new StringBuilder();

        foreach (var include in includes)
        {
            var joinProperty = GetJoinProperty(include, nameof(includes), out var attrJoin);

            var declaringType = joinProperty.ReflectedType?.GetTypeInfo();
            var tableAttribute = declaringType?.GetCustomAttribute<TableAttribute>();
            var tableName = MicroOrmConfig.TablePrefix + (tableAttribute != null ? tableAttribute.Name : declaringType?.Name);

            var joinType = joinProperty.PropertyType.IsGenericType ? joinProperty.PropertyType.GenericTypeArguments[0] : joinProperty.PropertyType;
            var properties = joinType.FindClassMetaDataProperties().Where(p => !p.IgnoreUpdate).ToArray();

            var joinEntity = entity.GetType().GetProperty(joinProperty.Name)?.GetValue(entity, null);
            if (joinEntity == null)
                continue;

            // "UPDATE t1 JOIN t2 ON ... SET ..." is MySQL-only syntax; every other provider rejects it at parse time
            if (Provider != SqlProvider.MySQL)
                throw new NotSupportedException(
                    $"Update with joins isn't supported for {Provider}, only for MySQL. Update [{joinProperty.Name}] with a separate query");

            var dict = properties.ToDictionary(prop => $"{prop.PropertyInfo.ReflectedType?.Name}{prop.PropertyName}",
                prop => joinType.GetProperty(prop.PropertyName)?.GetValue(joinEntity, null));
            originalBuilder.SetParam(dict);

            if (UseQuotationMarks == true)
            {
                tableName = GetTableNameWithQuotes(attrJoin, properties, tableName);
            }
            else
            {
                attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, Provider);
            }

            joinBuilder.Append(
                $", {GetFieldsUpdate(string.IsNullOrEmpty(attrJoin.TableAlias) ? attrJoin.TableName : attrJoin.TableAlias, properties, UseQuotationMarks == true)}");
            AppendJoinQuery(attrJoin, originalBuilder.SqlBuilder, tableName);
        }

        return joinBuilder.ToString();
    }

    private void AppendJoinQuery(JoinAttributeBase attrJoin, StringBuilder joinBuilder, string tableName)
    {
        var joinString = attrJoin.ToString();
        var joinAs = Provider == SqlProvider.Oracle ? " " : " AS ";
        if (attrJoin is CrossJoinAttribute)
        {
            joinBuilder.Append(string.IsNullOrEmpty(attrJoin.TableAlias)
                ? $"{joinString} {attrJoin.TableName} "
                : $"{joinString} {attrJoin.TableName}{joinAs}{attrJoin.TableAlias} ");
        }
        else if (attrJoin.TableName != null)
        {
            var customFilter = string.Empty;
            if (JoinLogicalDeleteProperties != null && JoinLogicalDeleteProperties.TryGetValue(attrJoin.TableName, out var deleteAttr))
            {
                var colAttr = deleteAttr.GetCustomAttribute<ColumnAttribute>();
                var colName = colAttr == null ? deleteAttr.Name : colAttr.Name;
                object deleteValue = Provider == SqlProvider.PostgreSQL ? "true" : 1;
                if (deleteAttr.PropertyType == typeof(bool?))
                {
                    customFilter = string.IsNullOrEmpty(attrJoin.TableAlias)
                        ? $"AND {attrJoin.TableName}.{colName} IS NULL "
                        : $"AND {attrJoin.TableAlias}.{colName} IS NULL ";
                }
                else
                {
                    if (deleteAttr.PropertyType.IsEnum)
                    {
                        var deleteOption = deleteAttr.PropertyType.GetFields().FirstOrDefault(f => f.GetCustomAttribute<DeletedAttribute>() != null);

                        if (deleteOption != null)
                        {
                            var enumValue = Enum.Parse(deleteAttr.PropertyType, deleteOption.Name);
                            deleteValue = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(deleteAttr.PropertyType));
                        }
                    }

                    customFilter = string.IsNullOrEmpty(attrJoin.TableAlias)
                        ? $"AND {attrJoin.TableName}.{colName} != {deleteValue} "
                        : $"AND {attrJoin.TableAlias}.{colName} != {deleteValue} ";
                }
            }

            joinBuilder.Append(string.IsNullOrEmpty(attrJoin.TableAlias)
                ? $"{joinString} {attrJoin.TableName} ON {tableName}.{attrJoin.Key} = {attrJoin.TableName}.{attrJoin.ExternalKey} {customFilter}"
                : $"{joinString} {attrJoin.TableName}{joinAs}{attrJoin.TableAlias} ON {tableName}.{attrJoin.Key} = {attrJoin.TableAlias}.{attrJoin.ExternalKey} {customFilter}");
        }
    }

    private string AppendJoinToSelect(SqlQuery originalBuilder, bool hasSelectFilter, params Expression<Func<TEntity, object>>[] includes)
    {
        var joinBuilder = new StringBuilder();

        foreach (var include in includes)
        {
            var joinProperty = GetJoinProperty(include, nameof(includes), out var attrJoin);

            var declaringType = joinProperty.ReflectedType?.GetTypeInfo();
            var tableAttribute = declaringType?.GetCustomAttribute<TableAttribute>();
            var tableName = MicroOrmConfig.TablePrefix + (tableAttribute != null ? tableAttribute.Name : declaringType?.Name);

            var joinType = joinProperty.PropertyType.IsGenericType ? joinProperty.PropertyType.GenericTypeArguments[0] : joinProperty.PropertyType;
            var properties = joinType.FindClassMetaDataProperties();

            if (UseQuotationMarks == true)
            {
                tableName = GetTableNameWithQuotes(attrJoin, properties, tableName);
            }
            else
            {
                attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, Provider);
            }

            if (!hasSelectFilter)
                originalBuilder.SqlBuilder.Append(
                    $", {GetFieldsSelect(string.IsNullOrEmpty(attrJoin.TableAlias) ? attrJoin.TableName : attrJoin.TableAlias, properties, UseQuotationMarks == true)}");

            AppendJoinQuery(attrJoin, joinBuilder, tableName);
        }

        return joinBuilder.ToString();
    }
}
