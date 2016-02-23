using System;
using System.Collections.Generic;
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

        ESqlConnector SqlConnector { get; set; }

        IEnumerable<PropertyMetadata> KeyProperties { get; }

        IEnumerable<PropertyMetadata> BaseProperties { get; }

        PropertyMetadata IdentityProperty { get; }

        PropertyMetadata StatusProperty { get; }

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