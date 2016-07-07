using System;
using System.Linq.Expressions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{

    /// <summary>
    /// Universal SqlGenerator for Tables
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISqlGenerator<TEntity> where TEntity : class
    {
        string TableName { get; }

        bool IsIdentity { get; }

        ESqlConnector ESqlConnector { get; set; }

        PropertyMetadata[] KeyProperties { get; }

        PropertyMetadata[] BaseProperties { get; }

        PropertyMetadata IdentityProperty { get; }

        string StatusPropertyName { get; }

        object LogicalDeleteValue { get; }

        bool LogicalDelete { get; }

        SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        SqlQuery GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> predicate);

        SqlQuery GetInsert(TEntity entity);

        SqlQuery GetUpdate(TEntity entity);

        SqlQuery GetDelete(TEntity entity);
    }
}