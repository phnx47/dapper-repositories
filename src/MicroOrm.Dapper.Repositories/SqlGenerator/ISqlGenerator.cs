using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
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
        bool HasUpdatedAt { get; }

        /// <summary>
        ///     Date of Changed Property
        /// </summary>
        PropertyInfo UpdatedAtProperty { get; }

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
        SqlPropertyMetadata UpdatedAtPropertyMetadata { get; }

        /// <summary>
        ///     Is Autoincrement table
        /// </summary>
        bool IsIdentity { get; }

        /// <summary>
        ///     Table Name
        /// </summary>
        string TableName { get; }

        /// <summary>
        ///     Table Schema
        /// </summary>
        string TableSchema { get; }

        /// <summary>
        ///     Identity Metadata property
        /// </summary>
        SqlPropertyMetadata IdentitySqlProperty { get; }

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
        Dictionary<string, PropertyInfo> JoinsLogicalDelete { get; }

        /// <summary>
        ///     Has Logical delete
        /// </summary>
        bool LogicalDelete { get; }

        /// <summary>
        ///     PropertyName of Status
        /// </summary>
        string StatusPropertyName { get; }

        /// <summary>
        ///     Logical delete Value
        /// </summary>
        object LogicalDeleteValue { get; }

        /// <summary>
        ///     Get SQL for COUNT Query
        /// </summary>
        SqlQuery GetCount(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Get SQL for COUNT with DISTINCT Query
        /// </summary>
        SqlQuery GetCount(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField, params Expression<Func<TEntity, object>>[] includes);

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
        SqlQuery GetUpdate(TEntity entity, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Get SQL for UPDATE Query
        /// </summary>
        SqlQuery GetUpdate(Expression<Func<TEntity, bool>> predicate, TEntity entity, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Get the SQL statement to update the fields under the anonymous class
        ///     (Filtering fields in anonymous classes that are inconsistent with the entity type)
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="setPropertyObj"></param>
        SqlQuery GetUpdate(Expression<Func<TEntity, bool>> predicate, object setPropertyObj);

        /// <summary>
        ///     Get the SQL statement to update the fields in the setPropertyDict dictionary
        ///     (Validates fields in the dictionary that are inconsistent with the entity type)
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="setPropertyDict"></param>
        SqlQuery GetUpdate(Expression<Func<TEntity, bool>> predicate, Dictionary<string, object> setPropertyDict);

        /// <summary>
        ///     Get SQL for bulk UPDATE Query
        /// </summary>
        SqlQuery GetBulkUpdate(IEnumerable<TEntity> entities);

        /// <summary>
        ///     Get SQL for SELECT Query by Id
        /// </summary>
        SqlQuery GetSelectById(object id, FilterData filterData, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Get SQL for SELECT Query
        /// </summary>
        SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate, FilterData filterData, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Get SQL for SELECT Query
        /// </summary>
        SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate, FilterData filterData, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Get SQL for SELECT Query with BETWEEN
        /// </summary>
        SqlQuery GetSelectBetween(object from, object to, FilterData filterData, Expression<Func<TEntity, object>> btwField);

        /// <summary>
        ///     Get SQL for SELECT Query with BETWEEN
        /// </summary>
        SqlQuery GetSelectBetween(object from, object to, FilterData filterData, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     Get SQL for DELETE Query
        /// </summary>
        SqlQuery GetDelete(TEntity entity);

        /// <summary>
        ///     Get SQL for DELETE Query
        /// </summary>
        SqlQuery GetDelete(Expression<Func<TEntity, bool>> predicate);
    }
}
