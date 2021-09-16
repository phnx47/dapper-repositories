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
        public bool BulkUpdate(IEnumerable<TEntity> instances)
        {
            return BulkUpdate(instances, null);
        }

        /// <inheritdoc />
        public bool BulkUpdate(IEnumerable<TEntity> instances, IDbTransaction transaction)
        {
            if (SqlGenerator.Provider == SqlProvider.MSSQL)
            {
                int count = 0;
                int totalInstances = instances.Count();

                var properties = SqlGenerator.SqlProperties.ToList();

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
                        var msSqlQueryResult = SqlGenerator.GetBulkUpdate(items);
                        count += Connection.Execute(msSqlQueryResult.GetSql(), msSqlQueryResult.Param, transaction);
                    }

                    return count > 0;
                }
            }

            var queryResult = SqlGenerator.GetBulkUpdate(instances);
            var result = Connection.Execute(queryResult.GetSql(), queryResult.Param, transaction) > 0;
            return result;
        }

        /// <inheritdoc />
        public Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances)
        {
            return BulkUpdateAsync(instances, null);
        }

        /// <inheritdoc />
        public async Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances, IDbTransaction transaction)
        {
            if (SqlGenerator.Provider == SqlProvider.MSSQL)
            {
                int count = 0;
                int totalInstances = instances.Count();

                var properties = SqlGenerator.SqlProperties.ToList();

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
                        var msSqlQueryResult = SqlGenerator.GetBulkUpdate(items);
                        count += await Connection.ExecuteAsync(msSqlQueryResult.GetSql(), msSqlQueryResult.Param, transaction);
                    }

                    return count > 0;
                }
            }

            var queryResult = SqlGenerator.GetBulkUpdate(instances);
            var result = await Connection.ExecuteAsync(queryResult.GetSql(), queryResult.Param, transaction) > 0;
            return result;
        }
    }
}
