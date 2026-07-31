using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroOrm.Dapper.Repositories.SqlGenerator;


public partial class SqlGenerator<TEntity>
    where TEntity : class
{

    public virtual SqlQuery GetBulkUpdate(IEnumerable<TEntity> entities)
    {
        var entitiesArray = entities as TEntity[] ?? entities.ToArray();
        if (entitiesArray.Length == 0)
            throw new ArgumentException("collection is empty");

        var entityType = entitiesArray[0].GetType();

        var properties = SqlProperties.Where(p =>
            !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate).ToArray();

        var query = new SqlQuery();

        var parameters = new Dictionary<string, object?>();

        //In Oracle we use MERGE INTO to excute multipe update with argument.
        var singleSelectsForOracle = new List<string>();

        for (var i = 0; i < entitiesArray.Length; i++)
        {
            var entity = entitiesArray[i];
            SetUpdatedAt(entity);

            if (Provider != SqlProvider.Oracle)
            {
                if (i > 0)
                    query.SqlBuilder.Append("; ");

                query.SqlBuilder.Append(
                    $"UPDATE {TableName} SET {string.Join(", ", properties.Select(p => $"{p.ColumnName} = {ParameterSymbol}{p.PropertyName}{i}"))} WHERE {string.Join(" AND ", KeySqlProperties.Where(p => !p.IgnoreUpdate).Select(p => $"{p.ColumnName} = {ParameterSymbol}{p.PropertyName}{i}"))}");
            }
            else
            {
                var singleSelect =
                    $"SELECT {string.Join(", ", properties.Select(p => $"{ParameterSymbol}{p.PropertyName}{i} AS {p.ColumnName}"))}, {string.Join(" , ", KeySqlProperties.Where(p => !p.IgnoreUpdate).Select(p => $"{ParameterSymbol}{p.PropertyName}{i} AS {p.ColumnName}"))} FROM DUAL";
                singleSelectsForOracle.Add(singleSelect);
            }

            // ReSharper disable PossibleNullReferenceException
            foreach (var property in properties)
                parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName)?.GetValue(entity, null));

            foreach (var property in KeySqlProperties.Where(p => !p.IgnoreUpdate))
                parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName)?.GetValue(entity, null));

            // ReSharper restore PossibleNullReferenceException
        }

        query.SetParam(parameters);

        if (Provider == SqlProvider.Oracle)
        {
            var unionTName = $"{TableName}_BULKUPDATE";
            var unionSelect = string.Join(" UNION ALL ", singleSelectsForOracle);
            var unionOn = $"({string.Join(" AND ", KeySqlProperties.Where(p => !p.IgnoreUpdate).Select(p => $"{unionTName}.{p.ColumnName} = {TableName}.{p.ColumnName}"))})";
            var unionSet = $"{string.Join(",", properties.Select(p => $"{p.ColumnName} = {unionTName}.{p.ColumnName} "))}";

            query.SqlBuilder.Append($"MERGE INTO {TableName} {TableName} USING ({unionSelect}) {unionTName} ON {unionOn} WHEN MATCHED THEN UPDATE SET {unionSet}");
        }

        return query;
    }
}
