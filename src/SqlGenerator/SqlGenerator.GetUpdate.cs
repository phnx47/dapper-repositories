using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator;

public partial class SqlGenerator<TEntity>
    where TEntity : class
{
    public virtual SqlQuery GetUpdate(TEntity entity, params Expression<Func<TEntity, object>>[] includes)
    {
        var properties = GetPropertiesToUpdate();

        if (properties.Length == 0)
            throw new ArgumentException("Can't update without [Key]");

        SetUpdatedAt(entity);

        return BuildUpdateByKey(entity, properties, includes);
    }

    public virtual SqlQuery GetUpdate(Expression<Func<TEntity, bool>>? predicate, TEntity entity, params Expression<Func<TEntity, object>>[] includes)
    {
        var properties = GetPropertiesToUpdate();

        SetUpdatedAt(entity);

        return BuildUpdateByPredicate(predicate, entity, properties, includes);
    }

    public virtual SqlQuery GetUpdateColumns(TEntity entity, Expression<Func<TEntity, object>> column, params Expression<Func<TEntity, object>>[] columns)
    {
        var properties = GetColumnsToUpdate(column, columns);

        SetUpdatedAt(entity);

        return BuildUpdateByKey(entity, properties, []);
    }

    public virtual SqlQuery GetUpdateColumns(Expression<Func<TEntity, bool>>? predicate, TEntity entity, Expression<Func<TEntity, object>> column,
        params Expression<Func<TEntity, object>>[] columns)
    {
        var properties = GetColumnsToUpdate(column, columns);

        SetUpdatedAt(entity);

        return BuildUpdateByPredicate(predicate, entity, properties, []);
    }

    public virtual SqlQuery GetUpdate(Expression<Func<TEntity, bool>>? predicate, object setPropertyObj)
    {
        var setProperties = setPropertyObj.GetType().GetProperties();
        var properties = SqlProperties
            .Where(p => !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate
                && setProperties.Any(k => k.Name.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        var updatedAt = GetUpdatedAtToSet(properties, out var updatedAtValue);
        var fields = properties;
        if (updatedAt != null)
            fields = [.. properties, updatedAt];

        var query = new SqlQuery();
        query.SqlBuilder
            .Append("UPDATE ")
            .Append(TableName)
            .Append(' ');
        query.SqlBuilder.Append("SET ");
        query.SqlBuilder.Append(GetFieldsUpdate(TableName, fields, UseQuotationMarks == true));
        query.SqlBuilder.Append(' ');
        AppendWherePredicateQuery(query, predicate, QueryType.Update);

        var parameters = (Dictionary<string, object?>)query.Param!;
        foreach (var metadata in properties)
        {
            var setProp = setProperties.FirstOrDefault(p => p.Name.Equals(metadata.PropertyName, StringComparison.OrdinalIgnoreCase));
            if (setProp == null)
                continue;
            parameters.Add($"{typeof(TEntity).Name}{metadata.PropertyName}", setProp.GetValue(setPropertyObj));
        }

        if (updatedAt != null)
            parameters.Add($"{typeof(TEntity).Name}{updatedAt.PropertyName}", updatedAtValue);

        return query;
    }

    public virtual SqlQuery GetUpdate(Expression<Func<TEntity, bool>>? predicate, Dictionary<string, object> setPropertyDict)
    {
        var propNameExceptItems = setPropertyDict.Keys.Except(SqlProperties.Select(p => p.PropertyName)).ToArray();
        if (propNameExceptItems.Length > 0)
        {
            string keys = string.Join(",", propNameExceptItems);
            throw new ArgumentException(string.Concat(nameof(setPropertyDict), "content error detail:", $" [{keys}] not equal entity column name"));
        }

        var properties = SqlProperties
            .Where(p => !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate
                && setPropertyDict.Any(k => k.Key.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        var updatedAt = GetUpdatedAtToSet(properties, out var updatedAtValue);
        var fields = properties;
        if (updatedAt != null)
            fields = [.. properties, updatedAt];

        var query = new SqlQuery();
        query.SqlBuilder
            .Append("UPDATE ")
            .Append(TableName)
            .Append(' ');
        query.SqlBuilder.Append("SET ");
        query.SqlBuilder.Append(GetFieldsUpdate(TableName, fields, UseQuotationMarks == true));
        query.SqlBuilder.Append(' ');
        AppendWherePredicateQuery(query, predicate, QueryType.Update);

        var parameters = (Dictionary<string, object?>)query.Param!;
        foreach (var metadata in properties)
        {
            var value = setPropertyDict.FirstOrDefault(p => p.Key.Equals(metadata.PropertyName, StringComparison.OrdinalIgnoreCase)).Value;
            parameters.Add($"{typeof(TEntity).Name}{metadata.PropertyName}", value);
        }

        if (updatedAt != null)
            parameters.Add($"{typeof(TEntity).Name}{updatedAt.PropertyName}", updatedAtValue);

        return query;
    }

    private SqlPropertyMetadata[] GetPropertiesToUpdate()
    {
        return SqlProperties.Where(p =>
            !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate).ToArray();
    }

    private SqlPropertyMetadata[] GetColumnsToUpdate(Expression<Func<TEntity, object>> column, Expression<Func<TEntity, object>>[] columns)
    {
        var properties = new List<SqlPropertyMetadata>(columns.Length + 1);

        AddColumnToUpdate(properties, column, nameof(column));
        foreach (var item in columns)
            AddColumnToUpdate(properties, item, nameof(columns));

        var updatedAt = GetUpdatedAtToSet(properties, out _);
        if (updatedAt != null)
            properties.Add(updatedAt);

        return properties.ToArray();
    }

    private void AddColumnToUpdate(List<SqlPropertyMetadata> properties, Expression<Func<TEntity, object>> column, string paramName)
    {
        var propertyName = ExpressionHelper.GetPropertyName(column);

        var property = SqlProperties.FirstOrDefault(p => p.PropertyName == propertyName)
            ?? throw new ArgumentException($"Can't update [{propertyName}]: not a mapped column of {typeof(TEntity).Name}", paramName);

        if (KeySqlProperties.Any(k => k.PropertyName == propertyName))
            throw new ArgumentException($"Can't update [{propertyName}]: property is marked with [Key]", paramName);

        if (property.IgnoreUpdate)
            throw new ArgumentException($"Can't update [{propertyName}]: property is marked with [IgnoreUpdate]", paramName);

        if (properties.All(p => p.PropertyName != propertyName))
            properties.Add(property);
    }

    private SqlQuery BuildUpdateByKey(TEntity entity, SqlPropertyMetadata[] properties, Expression<Func<TEntity, object>>[] includes)
    {
        var query = new SqlQuery();

        AppendUpdateSet(query, entity, properties, includes);

        query.SqlBuilder.Append(" WHERE ");

        query.SqlBuilder.Append(string.Join(" AND ", KeySqlProperties.Where(p => !p.IgnoreUpdate)
            .Select(p => $"{TableName}.{p.ColumnName} = {ParameterSymbol}{entity.GetType().Name}{p.PropertyName}")));

        if (query.Param is not Dictionary<string, object?> parameters)
            parameters = [];

        foreach (var metadata in properties.Concat(KeySqlProperties))
            parameters.Add($"{entity.GetType().Name}{metadata.PropertyName}", entity.GetType().GetProperty(metadata.PropertyName)?.GetValue(entity, null));

        query.SetParam(parameters);

        return query;
    }

    private SqlQuery BuildUpdateByPredicate(Expression<Func<TEntity, bool>>? predicate, TEntity entity, SqlPropertyMetadata[] properties,
        Expression<Func<TEntity, object>>[] includes)
    {
        var query = new SqlQuery();

        AppendUpdateSet(query, entity, properties, includes);

        query.SqlBuilder
            .Append(' ');

        AppendWherePredicateQuery(query, predicate, QueryType.Update);

        var parameters = (Dictionary<string, object?>)query.Param!;
        foreach (var metadata in properties)
            parameters.Add($"{entity.GetType().Name}{metadata.PropertyName}", entity.GetType().GetProperty(metadata.PropertyName)?.GetValue(entity, null));

        return query;
    }

    private void AppendUpdateSet(SqlQuery query, TEntity entity, SqlPropertyMetadata[] properties, Expression<Func<TEntity, object>>[] includes)
    {
        query.SqlBuilder
            .Append("UPDATE ")
            .Append(TableName)
            .Append(' ');

        var joinsBuilder = includes.Length > 0 ? AppendJoinToUpdate(entity, query, includes) : string.Empty;

        query.SqlBuilder.Append("SET ");
        query.SqlBuilder.Append(GetFieldsUpdate(TableName, properties, UseQuotationMarks == true));
        query.SqlBuilder.Append(joinsBuilder);
    }

    private string GetFieldsUpdate(string? tableName, IEnumerable<SqlPropertyMetadata> properties, bool useMarks)
    {
        return Provider is SqlProvider.SQLite or SqlProvider.PostgreSQL
            ? string.Join(", ", properties
                .Select(p =>
                    $"{(useMarks ? p.ColumnName : p.CleanColumnName)} = {ParameterSymbol}{p.PropertyInfo.ReflectedType?.Name}{p.PropertyName}"))
            : string.Join(", ", properties
                .Select(p =>
                    $"{tableName}.{(useMarks ? p.ColumnName : p.CleanColumnName)} = {ParameterSymbol}{p.PropertyInfo.ReflectedType?.Name}{p.PropertyName}"));
    }
}
