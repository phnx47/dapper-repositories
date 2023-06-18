using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace MicroOrm.Dapper.Repositories;

/// <summary>
///     Base Repository
/// </summary>
public partial class DapperRepository<TEntity>
    where TEntity : class
{

    public virtual bool Insert(TEntity instance)
    {
        return Insert(instance, null);
    }


    public virtual Task<bool> InsertAsync(TEntity instance)
    {
        return InsertAsync(instance, null, CancellationToken.None);
    }


    public virtual Task<bool> InsertAsync(TEntity instance, CancellationToken cancellationToken)
    {
        return InsertAsync(instance, null, cancellationToken);
    }


    public virtual bool Insert(TEntity instance, IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetInsert(instance);
        if (SqlGenerator.IsIdentity)
        {
            if (SqlGenerator.Provider == Repositories.SqlGenerator.SqlProvider.Oracle)
            {
                Connection.Execute(queryResult.GetSql(), queryResult.Param, transaction);
                int newId = ((DynamicParameters)(queryResult.Param!)).Get<int>(":newId");
                return SetValue(newId, instance);
            }
            else
            {
                var newId = Connection.Query<long>(queryResult.GetSql(), queryResult.Param, transaction).FirstOrDefault();
                return SetValue(newId, instance);
            }
        }

        return Connection.Execute(queryResult.GetSql(), instance, transaction) > 0;
    }


    public virtual Task<bool> InsertAsync(TEntity instance, IDbTransaction? transaction)
    {
        return InsertAsync(instance, transaction, CancellationToken.None);
    }


    public virtual async Task<bool> InsertAsync(TEntity instance, IDbTransaction? transaction, CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetInsert(instance);
        if (SqlGenerator.IsIdentity)
        {
            if (SqlGenerator.Provider == Repositories.SqlGenerator.SqlProvider.Oracle)
            {
                await Connection.ExecuteAsync(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken));
                int newId = ((DynamicParameters)(queryResult.Param!)).Get<int>(":newId");
                return SetValue(newId, instance);
            }
            else
            {
                var newId = (await Connection.QueryAsync<long>(queryResult.GetSql(), queryResult.Param, transaction)).FirstOrDefault();
                return SetValue(newId, instance);
            }
        }

        return await Connection.ExecuteAsync(new CommandDefinition(queryResult.GetSql(), instance, transaction, cancellationToken: cancellationToken)) > 0;
    }

    private bool SetValue(long newId, TEntity instance)
    {
        var added = newId > 0;
        if (added && SqlGenerator.IsIdentity)
        {
            var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentitySqlProperty.PropertyInfo.PropertyType);
            SqlGenerator.IdentitySqlProperty.PropertyInfo.SetValue(instance, newParsedId);
        }

        return added;
    }
}
