using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

namespace MicroOrm.Dapper.Repositories.SqlGenerator;

/// <summary>
///     Universal SqlGenerator for Tables
/// </summary>
public interface ISqlGenerator<TEntity> where TEntity : class
{
    /// <summary>
    ///     All original properties
    /// </summary>
    PropertyInfo[] AllProperties { get; }

    /// <summary>
    ///     Has Date of changed
    /// </summary>
    [MemberNotNullWhen(true, nameof(UpdatedAtProperty), nameof(UpdatedAtPropertyMetadata))]
    bool HasUpdatedAt { get; }

    /// <summary>
    ///     Date of Changed Property
    /// </summary>
    PropertyInfo? UpdatedAtProperty { get; }

    /// <summary>
    ///     Type Sql provider
    /// </summary>
    SqlProvider Provider { get; }

    /// <summary>
    ///     Use quotation marks for TableName and ColumnName
    /// </summary>
    bool? UseQuotationMarks { get; set; }

    /// <summary>
    ///     Date of Changed Metadata Property
    /// </summary>
    SqlPropertyMetadata? UpdatedAtPropertyMetadata { get; }

    /// <summary>
    ///     Is Autoincrement table
    /// </summary>
    [MemberNotNullWhen(true, nameof(IdentitySqlProperty))]
    bool IsIdentity { get; }

    /// <summary>
    ///     Table Name
    /// </summary>
    string TableName { get; }

    /// <summary>
    ///     Table Schema
    /// </summary>
    string? TableSchema { get; }

    /// <summary>
    ///     Identity Metadata property
    /// </summary>
    SqlPropertyMetadata? IdentitySqlProperty { get; }

    /// <summary>
    ///     Keys Metadata sql properties
    /// </summary>
    SqlPropertyMetadata[] KeySqlProperties { get; }

    /// <summary>
    ///     Metadata sql properties
    /// </summary>
    SqlPropertyMetadata[] SqlProperties { get; }

    /// <summary>
    ///     Metadata sql join properties
    /// </summary>
    SqlJoinPropertyMetadata[] SqlJoinProperties { get; }

    /// <summary>
    ///     Joined tables with logical delete
    /// </summary>
    Dictionary<string, PropertyInfo>? JoinLogicalDeleteProperties { get; }

    /// <summary>
    ///     Has Logical delete
    /// </summary>
    bool LogicalDelete { get; }

    /// <summary>
    ///     PropertyName of Status
    /// </summary>
    string? StatusPropertyName { get; }

    /// <summary>
    ///     Logical delete Value
    /// </summary>
    object? LogicalDeleteValue { get; }

    /// <summary>
    ///     Get SQL for COUNT Query
    /// </summary>
    SqlQuery GetCount(Expression<Func<TEntity, bool>>? predicate, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    ///     Get SQL for COUNT with DISTINCT Query
    /// </summary>
    SqlQuery GetCount(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>> distinctField, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    ///     Get SQL for INSERT Query
    /// </summary>
    SqlQuery GetInsert(TEntity entity);

    /// <summary>
    ///     Get SQL for bulk INSERT Query
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    SqlQuery GetBulkInsert(IEnumerable<TEntity> entities);

    /// <summary>
    ///     Get SQL for UPDATE Query
    /// </summary>
    /// <param name="entity">Entity to update; every mapped column is written</param>
    /// <param name="includes">Join properties whose rows are updated along with the entity. MySQL only</param>
    /// <exception cref="ArgumentException">A property in <paramref name="includes" /> isn't marked with a join attribute</exception>
    /// <exception cref="NotSupportedException">A join would be written for a provider other than MySQL</exception>
    SqlQuery GetUpdate(TEntity entity, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    ///     Get SQL for UPDATE Query
    /// </summary>
    /// <param name="predicate">Filter for the rows to update</param>
    /// <param name="entity">Entity to update; every mapped column is written</param>
    /// <param name="includes">Join properties whose rows are updated along with the entity. MySQL only</param>
    /// <exception cref="ArgumentException">A property in <paramref name="includes" /> isn't marked with a join attribute</exception>
    /// <exception cref="NotSupportedException">A join would be written for a provider other than MySQL</exception>
    SqlQuery GetUpdate(Expression<Func<TEntity, bool>>? predicate, TEntity entity, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    ///     Get SQL for UPDATE Query of the listed columns only
    /// </summary>
    /// <param name="entity">Entity the values are read from; rows are matched by [Key]</param>
    /// <param name="column">Column to update</param>
    /// <param name="columns">More columns to update</param>
    /// <exception cref="ArgumentException">A column isn't mapped, or is marked with [Key] or [IgnoreUpdate]</exception>
    SqlQuery GetUpdateColumns(TEntity entity, Expression<Func<TEntity, object>> column, params Expression<Func<TEntity, object>>[] columns);

    /// <summary>
    ///     Get SQL for UPDATE Query of the listed columns only
    /// </summary>
    /// <param name="predicate">Filter for the rows to update</param>
    /// <param name="entity">Entity the values are read from</param>
    /// <param name="column">Column to update</param>
    /// <param name="columns">More columns to update</param>
    /// <exception cref="ArgumentException">A column isn't mapped, or is marked with [Key] or [IgnoreUpdate]</exception>
    SqlQuery GetUpdateColumns(Expression<Func<TEntity, bool>>? predicate, TEntity entity, Expression<Func<TEntity, object>> column,
        params Expression<Func<TEntity, object>>[] columns);

    /// <summary>
    ///     Get the SQL statement to update the fields under the anonymous class
    ///     (Filtering fields in anonymous classes that are inconsistent with the entity type)
    /// </summary>
    /// <param name="predicate">Filter for the rows to update</param>
    /// <param name="setPropertyObj">Object whose properties name the columns to update and hold their values</param>
    SqlQuery GetUpdate(Expression<Func<TEntity, bool>>? predicate, object setPropertyObj);

    /// <summary>
    ///     Get the SQL statement to update the fields in the setPropertyDict dictionary
    ///     (Validates fields in the dictionary that are inconsistent with the entity type)
    /// </summary>
    /// <param name="predicate">Filter for the rows to update</param>
    /// <param name="setPropertyDict">Columns to update and their values</param>
    SqlQuery GetUpdate(Expression<Func<TEntity, bool>>? predicate, Dictionary<string, object> setPropertyDict);

    /// <summary>
    ///     Get SQL for bulk UPDATE Query
    /// </summary>
    SqlQuery GetBulkUpdate(IEnumerable<TEntity> entities);

    /// <summary>
    ///     Get SQL for SELECT Query by Id
    /// </summary>
    SqlQuery GetSelectById(object id, FilterData? filterData, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    ///     Get SQL for SELECT Query
    /// </summary>
    SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>>? predicate, FilterData? filterData, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    ///     Get SQL for SELECT Query
    /// </summary>
    SqlQuery GetSelectAll(Expression<Func<TEntity, bool>>? predicate, FilterData? filterData, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    ///     Get SQL for SELECT Query with BETWEEN
    /// </summary>
    SqlQuery GetSelectBetween(object from, object to, FilterData? filterData, Expression<Func<TEntity, object>> btwField);

    /// <summary>
    ///     Get SQL for SELECT Query with BETWEEN
    /// </summary>
    SqlQuery GetSelectBetween(object from, object to, FilterData? filterData, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>>? predicate);

    /// <summary>
    ///     Get SQL for DELETE Query
    /// </summary>
    SqlQuery GetDelete(TEntity entity);

    /// <summary>
    ///     Get SQL for DELETE Query
    /// </summary>
    SqlQuery GetDelete(Expression<Func<TEntity, bool>>? predicate);
}
