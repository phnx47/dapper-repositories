namespace MicroOrm.Dapper.Repositories
{
    public partial class DapperRepository<TEntity> 
        where TEntity : class
    {
        /// <summary>
        ///     Dummy type for excluding from multi-map
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Local
        private class DontMap
        {
        }
    }
}
