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
        #region Properties

        string TableName { get; }

        bool IsIdentity { get; }

        ESqlConnector SqlConnector { get; set; }

        IEnumerable<PropertyMetadata> KeyProperties { get; }

        IEnumerable<PropertyMetadata> BaseProperties { get; }

        PropertyMetadata IdentityProperty { get; }

        PropertyMetadata StatusProperty { get; }

        object LogicalDeleteValue { get; }

        bool LogicalDelete { get; }

        #endregion Properties

        #region Methods

        QueryResult GetSelect(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);

        QueryResult GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> expression);

        QueryResult GetInsert(TEntity entity);

        QueryResult GetUpdate(TEntity entity);

        QueryResult GetDelete(TEntity entity);

        #endregion Methods
    }
}