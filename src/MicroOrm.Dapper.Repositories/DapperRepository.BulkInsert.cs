using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator;


namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     Base Repository
    /// </summary>
    public partial class DapperRepository<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual int BulkInsert(IEnumerable<TEntity> instances, IDbTransaction transaction = null)
        {
            if (SqlGenerator.Provider == SqlProvider.MSSQL)
            {
                int count = 0;
                int totalInstances = instances.Count();

                var properties =
                    (SqlGenerator.IsIdentity
                        ? SqlGenerator.SqlProperties.Where(p => !p.PropertyName.Equals(SqlGenerator.IdentitySqlProperty.PropertyName, StringComparison.OrdinalIgnoreCase))
                        : SqlGenerator.SqlProperties).ToList();

                int exceededTimes = (int) Math.Ceiling(totalInstances * properties.Count / 2099d);
                if (exceededTimes > 1)
                {
                    int maxAllowedInstancesPerBatch = totalInstances / exceededTimes;

                    for (int i = 0; i <= exceededTimes; i++)
                    {
                        var skips = i * maxAllowedInstancesPerBatch;
                        
                        if (skips >= totalInstances)
                            break;

                        var items = instances.Skip(skips).Take(maxAllowedInstancesPerBatch);
                        var msSqlQueryResult = SqlGenerator.GetBulkInsert(items);
                        count += Connection.Execute(msSqlQueryResult.GetSql(), msSqlQueryResult.Param, transaction);
                    }

                    return count;
                }
            }

            var queryResult = SqlGenerator.GetBulkInsert(instances);
            return Connection.Execute(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual async Task<int> BulkInsertAsync(IEnumerable<TEntity> instances, IDbTransaction transaction = null)
        {
            if (SqlGenerator.Provider == SqlProvider.MSSQL)
            {
                int count = 0;
                int totalInstances = instances.Count();

                var properties =
                    (SqlGenerator.IsIdentity
                        ? SqlGenerator.SqlProperties.Where(p => !p.PropertyName.Equals(SqlGenerator.IdentitySqlProperty.PropertyName, StringComparison.OrdinalIgnoreCase))
                        : SqlGenerator.SqlProperties).ToList();

                int exceededTimes = (int) Math.Ceiling(totalInstances * properties.Count / 2099d);
                if (exceededTimes > 1)
                {
                    int maxAllowedInstancesPerBatch = totalInstances / exceededTimes;

                    for (int i = 0; i <= exceededTimes; i++)
                    {
                        var skips = i * maxAllowedInstancesPerBatch;

                        if (skips >= totalInstances)
                            break;

                        var items = instances.Skip(i * maxAllowedInstancesPerBatch).Take(maxAllowedInstancesPerBatch);
                        var msSqlQueryResult = SqlGenerator.GetBulkInsert(items);
                        count += await Connection.ExecuteAsync(msSqlQueryResult.GetSql(), msSqlQueryResult.Param, transaction);
                    }

                    return count;
                }
            }

            var queryResult = SqlGenerator.GetBulkInsert(instances);
            return await Connection.ExecuteAsync(queryResult.GetSql(), queryResult.Param, transaction).ConfigureAwait(false);
        }
    }
}
