


using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MicroOrm.Dapper.Repositories
{

    public partial interface IReadOnlyDapperRepository<TEntity>
    {

        /// <summary>
        ///     Get number of rows
        /// </summary>
        int Count();

        /// <summary>
        ///     Get number of rows
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        ///     Get number of rows
        /// </summary>
        Task<int> CountAsync(CancellationToken cancellationToken);


        /// <summary>
        ///     Get number of rows
        /// </summary>
        int Count(IDbTransaction transaction);

        /// <summary>
        ///     Get number of rows
        /// </summary>
        Task<int> CountAsync(IDbTransaction transaction);

        /// <summary>
        ///     Get number of rows
        /// </summary>
        Task<int> CountAsync(IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get number of rows with WHERE clause
        /// </summary>
        int Count(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     Get number of rows with WHERE clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     Get number of rows with WHERE clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);


        /// <summary>
        ///     Get number of rows with WHERE clause
        /// </summary>
        int Count(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction);

        /// <summary>
        ///     Get number of rows with WHERE clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction);

        /// <summary>
        ///     Get number of rows with WHERE clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get number of rows with DISTINCT clause
        /// </summary>
        int Count(Expression<Func<TEntity, object>> distinctField);

        /// <summary>
        ///     Get number of rows with DISTINCT clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField);

        /// <summary>
        ///     Get number of rows with DISTINCT clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, CancellationToken cancellationToken);


        /// <summary>
        ///     Get number of rows with DISTINCT clause
        /// </summary>
        int Count(Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction);

        /// <summary>
        ///     Get number of rows with DISTINCT clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction);

        /// <summary>
        ///     Get number of rows with DISTINCT clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get number of rows with DISTINCT and WHERE clause
        /// </summary>
        int Count(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField);

        /// <summary>
        ///     Get number of rows with DISTINCT and WHERE clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField);

        /// <summary>
        ///     Get number of rows with DISTINCT and WHERE clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField, CancellationToken cancellationToken);


        /// <summary>
        ///     Get number of rows with DISTINCT and WHERE clause
        /// </summary>
        int Count(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction);

        /// <summary>
        ///     Get number of rows with DISTINCT and WHERE clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction);

        /// <summary>
        ///     Get number of rows with DISTINCT and WHERE clause
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object
        /// </summary>
        TEntity Find();

        /// <summary>
        ///     Get first object
        /// </summary>
        Task<TEntity> FindAsync();

        /// <summary>
        ///     Get first object
        /// </summary>
        Task<TEntity> FindAsync(CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object
        /// </summary>
        TEntity Find(IDbTransaction transaction);

        /// <summary>
        ///     Get first object
        /// </summary>
        Task<TEntity> FindAsync(IDbTransaction transaction);

        /// <summary>
        ///     Get first object
        /// </summary>
        Task<TEntity> FindAsync(IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object
        /// </summary>
        TEntity Find(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     Get first object
        /// </summary>
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     Get first object
        /// </summary>
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object
        /// </summary>
        TEntity Find(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction);

        /// <summary>
        ///     Get first object
        /// </summary>
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction);

        /// <summary>
        ///     Get first object
        /// </summary>
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6, CancellationToken cancellationToken);


        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        TEntity Find<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction);

        /// <summary>
        ///     Get first object with join objects
        /// </summary>
        Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects
        /// </summary>
        IEnumerable<TEntity> FindAll();

        /// <summary>
        ///     Get all objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync();

        /// <summary>
        ///     Get all objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects
        /// </summary>
        IEnumerable<TEntity> FindAll(IDbTransaction transaction);

        /// <summary>
        ///     Get all objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync(IDbTransaction transaction);

        /// <summary>
        ///     Get all objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync(IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects
        /// </summary>
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     Get all objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     Get all objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects
        /// </summary>
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction);

        /// <summary>
        ///     Get all objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction);

        /// <summary>
        ///     Get all objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with join objects
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id
        /// </summary>
        TEntity FindById(object id);

        /// <summary>
        ///     Get object by Id
        /// </summary>
        Task<TEntity> FindByIdAsync(object id);

        /// <summary>
        ///     Get object by Id
        /// </summary>
        Task<TEntity> FindByIdAsync(object id, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id
        /// </summary>
        TEntity FindById(object id, IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id
        /// </summary>
        Task<TEntity> FindByIdAsync(object id, IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id
        /// </summary>
        Task<TEntity> FindByIdAsync(object id, IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1, TChild2, TChild3>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1, TChild2, TChild3>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1, TChild2, TChild3, TChild4>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1, TChild2, TChild3, TChild4>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6, CancellationToken cancellationToken);


        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        TEntity FindById<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction);

        /// <summary>
        ///     Get object by Id with join objects
        /// </summary>
        Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField);

        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField);

        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        IEnumerable<TEntity> FindAllBetween(
            object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(
            object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(
            object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        IEnumerable<TEntity> FindAllBetween(object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all objects with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField);

        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField);

        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction);

        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction);

        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        IEnumerable<TEntity> FindAllBetween(
            DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(
            DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(
            DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);


        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        IEnumerable<TEntity> FindAllBetween(DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction);

        /// <summary>
        ///     Get all DateTimes with BETWEEN query
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction, CancellationToken cancellationToken);


    }

    public partial class ReadOnlyDapperRepository<TEntity>
    {

        /// <inheritdoc />
        public virtual int Count()
        {
            return Count(transaction: null);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync()
        {
            return CountAsync(transaction: null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return CountAsync(transaction: null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual int Count(IDbTransaction transaction)
        {
            return Count(null, transaction);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync(IDbTransaction transaction)
        {
            return CountAsync(null, transaction, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync(IDbTransaction transaction, CancellationToken cancellationToken)
        {
            return CountAsync(null, transaction, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Count(predicate, transaction: null);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return CountAsync(predicate, transaction: null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return CountAsync(predicate, transaction: null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction)
        {
            return CountAsync(predicate, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, object>> distinctField)
        {
            return Count(distinctField, transaction: null);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField)
        {
            return CountAsync(distinctField, transaction: null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, CancellationToken cancellationToken)
        {
            return CountAsync(distinctField, transaction: null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction)
        {
            return Count(null, distinctField, transaction);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction)
        {
            return CountAsync(null, distinctField, transaction, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            return CountAsync(null, distinctField, transaction, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField)
        {
            return Count(predicate, distinctField, transaction: null);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField)
        {
            return CountAsync(predicate, distinctField, transaction: null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField, CancellationToken cancellationToken)
        {
            return CountAsync(predicate, distinctField, transaction: null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction)
        {
            return CountAsync(predicate, distinctField, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity Find()
        {
            return Find(null, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync()
        {
            return FindAsync(null, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync(CancellationToken cancellationToken)
        {
            return FindAsync(null, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual TEntity Find(IDbTransaction transaction)
        {
            return Find(null, transaction);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync(IDbTransaction transaction)
        {
            return FindAsync(null, transaction, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync(IDbTransaction transaction, CancellationToken cancellationToken)
        {
            return FindAsync(null, transaction, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Find(predicate, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return FindAsync(predicate, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return FindAsync(predicate, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction)
        {
            return FindAsync(predicate, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity Find<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1)
        {
            return Find<TChild1>(predicate, tChild1, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1)
        {
            return FindAsync<TChild1>(predicate, tChild1, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1, CancellationToken cancellationToken)
        {
            return FindAsync<TChild1>(predicate, tChild1, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction)
        {
            return FindAsync<TChild1>(predicate, tChild1, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity Find<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2)
        {
            return Find<TChild1, TChild2>(predicate, tChild1, tChild2, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2)
        {
            return FindAsync<TChild1, TChild2>(predicate, tChild1, tChild2, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2, CancellationToken cancellationToken)
        {
            return FindAsync<TChild1, TChild2>(predicate, tChild1, tChild2, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction)
        {
            return FindAsync<TChild1, TChild2>(predicate, tChild1, tChild2, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity Find<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3)
        {
            return Find<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3)
        {
            return FindAsync<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3, CancellationToken cancellationToken)
        {
            return FindAsync<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction)
        {
            return FindAsync<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity Find<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4)
        {
            return Find<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4)
        {
            return FindAsync<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4, CancellationToken cancellationToken)
        {
            return FindAsync<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction)
        {
            return FindAsync<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity Find<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5)
        {
            return Find<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5)
        {
            return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5, CancellationToken cancellationToken)
        {
            return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction)
        {
            return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity Find<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6)
        {
            return Find<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6)
        {
            return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6, CancellationToken cancellationToken)
        {
            return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction)
        {
            return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll()
        {
            return FindAll(null, transaction: null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync()
        {
            return FindAllAsync(null, transaction: null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken)
        {
            return FindAllAsync(null, transaction: null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll(IDbTransaction transaction)
        {
            return FindAll(null, transaction);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(IDbTransaction transaction)
        {
            return FindAllAsync(null, transaction, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(IDbTransaction transaction, CancellationToken cancellationToken)
        {
            return FindAllAsync(null, transaction, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return FindAll(predicate, transaction: null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return FindAllAsync(predicate, transaction: null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return FindAllAsync(predicate, transaction: null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction)
        {
            return FindAllAsync(predicate, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1)
        {
            return FindAll<TChild1>(predicate, tChild1, null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1)
        {
            return FindAllAsync<TChild1>(predicate, tChild1, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1, CancellationToken cancellationToken)
        {
            return FindAllAsync<TChild1>(predicate, tChild1, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction)
        {
            return FindAllAsync<TChild1>(predicate, tChild1, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2)
        {
            return FindAll<TChild1, TChild2>(predicate, tChild1, tChild2, null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2)
        {
            return FindAllAsync<TChild1, TChild2>(predicate, tChild1, tChild2, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2, CancellationToken cancellationToken)
        {
            return FindAllAsync<TChild1, TChild2>(predicate, tChild1, tChild2, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction)
        {
            return FindAllAsync<TChild1, TChild2>(predicate, tChild1, tChild2, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3)
        {
            return FindAll<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3)
        {
            return FindAllAsync<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3, CancellationToken cancellationToken)
        {
            return FindAllAsync<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction)
        {
            return FindAllAsync<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4)
        {
            return FindAll<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4)
        {
            return FindAllAsync<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4, CancellationToken cancellationToken)
        {
            return FindAllAsync<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction)
        {
            return FindAllAsync<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5)
        {
            return FindAll<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5)
        {
            return FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5, CancellationToken cancellationToken)
        {
            return FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction)
        {
            return FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6)
        {
            return FindAll<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6)
        {
            return FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6, CancellationToken cancellationToken)
        {
            return FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction)
        {
            return FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity FindById(object id)
        {
            return FindById(id, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync(object id)
        {
            return FindByIdAsync(id, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync(object id, CancellationToken cancellationToken)
        {
            return FindByIdAsync(id, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync(object id, IDbTransaction transaction)
        {
            return FindByIdAsync(id, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity FindById<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1)
        {
            return FindById<TChild1>(id, tChild1, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1)
        {
            return FindByIdAsync<TChild1>(id, tChild1, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1, CancellationToken cancellationToken)
        {
            return FindByIdAsync<TChild1>(id, tChild1, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction transaction)
        {
            return FindByIdAsync<TChild1>(id, tChild1, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity FindById<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2)
        {
            return FindById<TChild1, TChild2>(id, tChild1, tChild2, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2)
        {
            return FindByIdAsync<TChild1, TChild2>(id, tChild1, tChild2, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2, CancellationToken cancellationToken)
        {
            return FindByIdAsync<TChild1, TChild2>(id, tChild1, tChild2, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction)
        {
            return FindByIdAsync<TChild1, TChild2>(id, tChild1, tChild2, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity FindById<TChild1, TChild2, TChild3>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3)
        {
            return FindById<TChild1, TChild2, TChild3>(id, tChild1, tChild2, tChild3, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3>(id, tChild1, tChild2, tChild3, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3, CancellationToken cancellationToken)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3>(id, tChild1, tChild2, tChild3, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3>(id, tChild1, tChild2, tChild3, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity FindById<TChild1, TChild2, TChild3, TChild4>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4)
        {
            return FindById<TChild1, TChild2, TChild3, TChild4>(id, tChild1, tChild2, tChild3, tChild4, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(id, tChild1, tChild2, tChild3, tChild4, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4, CancellationToken cancellationToken)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(id, tChild1, tChild2, tChild3, tChild4, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(id, tChild1, tChild2, tChild3, tChild4, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity FindById<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5)
        {
            return FindById<TChild1, TChild2, TChild3, TChild4, TChild5>(id, tChild1, tChild2, tChild3, tChild4, tChild5, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(id, tChild1, tChild2, tChild3, tChild4, tChild5, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5, CancellationToken cancellationToken)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(id, tChild1, tChild2, tChild3, tChild4, tChild5, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(id, tChild1, tChild2, tChild3, tChild4, tChild5, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual TEntity FindById<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6)
        {
            return FindById<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(id, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(id, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6, CancellationToken cancellationToken)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(id, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction)
        {
            return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(id, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField)
        {
            return FindAllBetween(from, to, btwField, transaction: null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField)
        {
            return FindAllBetweenAsync(from, to, btwField, transaction: null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, CancellationToken cancellationToken)
        {
            return FindAllBetweenAsync(from, to, btwField, transaction: null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction)
        {
            return FindAllBetween(from, to, btwField, null, transaction);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction)
        {
            return FindAllBetweenAsync(from, to, btwField, null, transaction, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            return FindAllBetweenAsync(from, to, btwField, null, transaction, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAllBetween(
            object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate)
        {
            return FindAllBetween(from, to, btwField, transaction: null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(
            object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate)
        {
            return FindAllBetweenAsync(from, to, btwField, transaction: null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(
            object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return FindAllBetweenAsync(from, to, btwField, transaction: null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction)
        {
            return FindAllBetweenAsync(from, to, btwField, null, transaction, cancellationToken: default);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField)
        {
            return FindAllBetween(from, to, btwField, transaction: null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField)
        {
            return FindAllBetweenAsync(from, to, btwField, transaction: null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, CancellationToken cancellationToken)
        {
            return FindAllBetweenAsync(from, to, btwField, transaction: null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction)
        {
            return FindAllBetween(from, to, btwField, null, transaction);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction)
        {
            return FindAllBetweenAsync(from, to, btwField, null, transaction, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            return FindAllBetweenAsync(from, to, btwField, null, transaction, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAllBetween(
            DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate)
        {
            return FindAllBetween(from, to, btwField, transaction: null);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(
            DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate)
        {
            return FindAllBetweenAsync(from, to, btwField, transaction: null, cancellationToken: default);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(
            DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return FindAllBetweenAsync(from, to, btwField, transaction: null, cancellationToken: cancellationToken);
        }



        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction)
        {
            return FindAllBetweenAsync(from, to, btwField, null, transaction, cancellationToken: default);
        }



    }
}
