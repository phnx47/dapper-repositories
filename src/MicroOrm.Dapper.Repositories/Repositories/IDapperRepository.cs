using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MicroOrm.Dapper.Repositories.Repositories
{
    public interface IDapperRepository<TEntity> where TEntity : new()
    {
        IEnumerable<TEntity> FindAll();

        Task<IEnumerable<TEntity>> FindAllAsync();

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression);

        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> expression);

        TEntity Find(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression);

        bool Insert(TEntity instance);

        Task<bool> InsertAsync(TEntity instance);

        bool Delete(TEntity instance);

        Task<bool> DeleteAsync(TEntity instance);

        bool Update(TEntity instance);

        Task<bool> UpdateAsync(TEntity instance);

    }
}