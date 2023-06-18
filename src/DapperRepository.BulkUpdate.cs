using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MicroOrm.Dapper.Repositories.SqlGenerator;

namespace MicroOrm.Dapper.Repositories;

/// <summary>
///     Base Repository
/// </summary>
public partial class DapperRepository<TEntity>
    where TEntity : class
{

    public virtual bool BulkUpdate(IEnumerable<TEntity> instances)
    {
        return BulkUpdate(instances, null);
    }


    public virtual bool BulkUpdate(IEnumerable<TEntity> instances, IDbTransaction? transaction)
    {
        if (SqlGenerator.Provider == SqlProvider.MSSQL)
        {
            int count = 0;
            int totalInstances = instances.Count();

            var properties = SqlGenerator.SqlProperties.ToList();

            int exceededTimes = (int)Math.Ceiling(totalInstances * properties.Count / 2099d);
            if (exceededTimes > 1)
            {
                int maxAllowedInstancesPerBatch = totalInstances / exceededTimes;
                if (maxAllowedInstancesPerBatch > 1000) maxAllowedInstancesPerBatch = 1000;
                var maxIterationCount = (int)Math.Ceiling((double)totalInstances / (double)maxAllowedInstancesPerBatch);
                for (int i = 0; i <= maxIterationCount; i++)
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


    public virtual Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances)
    {
        return BulkUpdateAsync(instances, null, CancellationToken.None);
    }


    public virtual Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances, CancellationToken cancellationToken)
    {
        return BulkUpdateAsync(instances, null, cancellationToken);
    }


    public virtual Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances, IDbTransaction? transaction)
    {
        return BulkUpdateAsync(instances, transaction, CancellationToken.None);
    }


    public virtual async Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances, IDbTransaction? transaction, CancellationToken cancellationToken)
    {
        if (SqlGenerator.Provider == SqlProvider.MSSQL)
        {
            int count = 0;
            int totalInstances = instances.Count();

            var properties = SqlGenerator.SqlProperties.ToList();

            int exceededTimes = (int)Math.Ceiling(totalInstances * properties.Count / 2099d);
            if (exceededTimes > 1)
            {
                int maxAllowedInstancesPerBatch = totalInstances / exceededTimes;
                if (maxAllowedInstancesPerBatch > 1000) maxAllowedInstancesPerBatch = 1000;

                var maxIterationCount = (int)Math.Ceiling((double)totalInstances / (double)maxAllowedInstancesPerBatch);

                for (var i = 0; i <= maxIterationCount; i++)
                {
                    var skips = i * maxAllowedInstancesPerBatch;

                    if (skips >= totalInstances)
                        break;

                    var items = instances.Skip(skips).Take(maxAllowedInstancesPerBatch);
                    var msSqlQueryResult = SqlGenerator.GetBulkUpdate(items);
                    count += await Connection.ExecuteAsync(new CommandDefinition(msSqlQueryResult.GetSql(), msSqlQueryResult.Param, transaction,
                        cancellationToken: cancellationToken));
                }

                return count > 0;
            }
        }

        var queryResult = SqlGenerator.GetBulkUpdate(instances);
        var result = await Connection.ExecuteAsync(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken)) > 0;
        return result;
    }
}
