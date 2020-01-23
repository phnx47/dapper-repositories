using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     Base Repository
    /// </summary>
    public partial class ReadOnlyDapperRepository<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetLimit()
        {
            FilterData.LimitInfo = null;
            return this;
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetLimit(uint limit, uint? offset, bool permanent)
        {
            if (limit <= 0)
                return this;

            var data = FilterData.LimitInfo ?? new LimitInfo();
            data.Limit = limit;
            data.Offset = offset;
            data.Permanent = permanent;
            FilterData.LimitInfo = data;

            return this;
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetLimit(uint limit)
        {
            return SetLimit(limit, null, false);
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetLimit(uint limit, uint offset)
        {
            return SetLimit(limit, offset, false);
        }
    }
}
