using System;
using System.Linq.Expressions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{

    /// <summary>
    /// Universal SqlGenerator for Tables
    /// </summary>
    public interface ISqlGenerator<TEntity> where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsIdentity { get; }

        /// <summary>
        /// 
        /// </summary>
        ESqlConnector SqlConnector { get; set; }

        /// <summary>
        /// 
        /// </summary>
        SqlPropertyMetadata[] KeySqlProperties { get; }

        /// <summary>
        /// 
        /// </summary>
        SqlPropertyMetadata[] SqlProperties { get; }

        /// <summary>
        /// 
        /// </summary>
        SqlPropertyMetadata IdentitySqlProperty { get; }

        /// <summary>
        /// 
        /// </summary>
        string StatusPropertyName { get; }

        /// <summary>
        /// 
        /// </summary>
        object LogicalDeleteValue { get; }

        /// <summary>
        /// 
        /// </summary>
        bool LogicalDelete { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwFiled"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        SqlQuery GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Get SQL for INSERT Query
        /// </summary>
        SqlQuery GetInsert(TEntity entity);

        /// <summary>
        /// Get SQL for UPDATE Query
        /// </summary>
        SqlQuery GetUpdate(TEntity entity);

        /// <summary>
        /// Get SQL for DELETE Query
        /// </summary>
        SqlQuery GetDelete(TEntity entity);


    }
}