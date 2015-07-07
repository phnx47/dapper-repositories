using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MicroOrm.Dapper.Repositories.SqlGenerator.Models;

namespace MicroOrm.Dapper.Repositories.SqlGenerator.Interfaces
{
    /// <summary>
    /// Universal SqlGenerator for Tables
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISqlGenerator<TEntity> where TEntity : new()
    {
        #region Properties

        string TableName { get; }

        bool IsIdentity { get; }

        IEnumerable<PropertyMetadata> KeyProperties { get; }

        IEnumerable<PropertyMetadata> BaseProperties { get; }

        PropertyMetadata IdentityProperty { get; }

        PropertyMetadata StatusProperty { get; }

        object LogicalDeleteValue { get; }

        bool LogicalDelete { get; }

        #endregion

        #region Methods

        QueryResult GetSelect();

        QueryResult GetSelect(Expression<Func<TEntity, bool>> expression);

        QueryResult GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> expression);

        QueryResult GetInsert(TEntity entity); 

        QueryResult GetUpdate(TEntity entity);

        QueryResult GetDelete(TEntity entity); 

        #endregion
    }
}
