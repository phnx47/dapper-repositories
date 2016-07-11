using MicroOrm.Dapper.Repositories.SqlGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    /// Interface for Repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDapperRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Connection
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Transaction
        /// </summary>
        IDbTransaction Transaction { get; }

        /// <summary>
        ///
        /// </summary>
        ISqlGenerator<TEntity> SqlGenerator { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        TEntity Find(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="expression"></param>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        TEntity Find<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll();

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="expression"></param>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindAllAsync();

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="expression"></param>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TChild1"></typeparam>
        /// <param name="expression"></param>
        /// <param name="tChild1"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<TEntity> FindAsync();

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool Insert(TEntity instance);

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(TEntity instance);

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool Delete(TEntity instance);

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(TEntity instance);

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool Update(TEntity instance);

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(TEntity instance);

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField);

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression);

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField);

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression);

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField);

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression);

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField);

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="btwField"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression);
    }
}