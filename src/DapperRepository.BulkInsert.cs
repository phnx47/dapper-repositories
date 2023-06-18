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

    public virtual int BulkInsert(IEnumerable<TEntity> instances)
    {
        return BulkInsert(instances, null);
    }


    public virtual Task<int> BulkInsertAsync(IEnumerable<TEntity> instances)
    {
        return BulkInsertAsync(instances, null, CancellationToken.None);
    }


    public virtual Task<int> BulkInsertAsync(IEnumerable<TEntity> instances, CancellationToken cancellationToken)
    {
        return BulkInsertAsync(instances, null, cancellationToken);
    }


    public virtual int BulkInsert(IEnumerable<TEntity> instances, IDbTransaction? transaction)
    {
        if (SqlGenerator.Provider == SqlProvider.MSSQL)
        {
            var count = 0;
            var totalInstances = instances.Count();

            var properties =
                (SqlGenerator.IsIdentity
                    ? SqlGenerator.SqlProperties.Where(p => !p.PropertyName.Equals(SqlGenerator.IdentitySqlProperty.PropertyName, StringComparison.OrdinalIgnoreCase))
                    : SqlGenerator.SqlProperties).ToList();

            var exceededTimes = (int)Math.Ceiling(totalInstances * properties.Count / 2099d);
            if (exceededTimes > 1)
            {
                var maxAllowedInstancesPerBatch = totalInstances / exceededTimes;
                if (maxAllowedInstancesPerBatch > 1000) maxAllowedInstancesPerBatch = 1000;

                var maxIterationCount = (int)Math.Ceiling((double)totalInstances / (double)maxAllowedInstancesPerBatch);

                for (var i = 0; i <= maxIterationCount; i++)
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


    public virtual Task<int> BulkInsertAsync(IEnumerable<TEntity> instances, IDbTransaction? transaction)
    {
        return BulkInsertAsync(instances, transaction, CancellationToken.None);
    }


    public virtual async Task<int> BulkInsertAsync(IEnumerable<TEntity> instances, IDbTransaction? transaction, CancellationToken cancellationToken)
    {
        if (SqlGenerator.Provider == SqlProvider.MSSQL)
        {
            var count = 0;
            var totalInstances = instances.Count();

            var properties =
                (SqlGenerator.IsIdentity
                    ? SqlGenerator.SqlProperties.Where(p => !p.PropertyName.Equals(SqlGenerator.IdentitySqlProperty.PropertyName, StringComparison.OrdinalIgnoreCase))
                    : SqlGenerator.SqlProperties).ToList();

            var exceededTimes = (int)Math.Ceiling(totalInstances * properties.Count / 2099d);
            if (exceededTimes > 1)
            {
                var maxAllowedInstancesPerBatch = totalInstances / exceededTimes;
                if (maxAllowedInstancesPerBatch > 1000) maxAllowedInstancesPerBatch = 1000;

                var maxIterationCount = (int)Math.Ceiling((double)totalInstances / (double)maxAllowedInstancesPerBatch);

                for (var i = 0; i <= maxIterationCount; i++)
                {
                    var skips = i * maxAllowedInstancesPerBatch;

                    if (skips >= totalInstances)
                        break;

                    var items = instances.Skip(i * maxAllowedInstancesPerBatch).Take(maxAllowedInstancesPerBatch);
                    var msSqlQueryResult = SqlGenerator.GetBulkInsert(items);
                    count += await Connection.ExecuteAsync(new CommandDefinition(msSqlQueryResult.GetSql(), msSqlQueryResult.Param, transaction,
                        cancellationToken: cancellationToken));
                }

                return count;
            }
        }

        var queryResult = SqlGenerator.GetBulkInsert(instances);
        return await Connection.ExecuteAsync(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken))
            .ConfigureAwait(false);
    }
}
