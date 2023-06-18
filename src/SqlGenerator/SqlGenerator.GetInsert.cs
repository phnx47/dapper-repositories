using System;
using System.Linq;
using System.Reflection;
using Dapper;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Extensions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator;


public partial class SqlGenerator<TEntity>
    where TEntity : class
{

    public virtual SqlQuery GetInsert(TEntity entity)
    {
        var properties =
            (IsIdentity
                ? SqlProperties.Where(p => !p.PropertyName.Equals(IdentitySqlProperty.PropertyName, StringComparison.OrdinalIgnoreCase))
                : SqlProperties).ToList();

        if (HasUpdatedAt && UpdatedAtProperty.GetCustomAttribute<UpdatedAtAttribute>() is { } attribute)
        {
            var offset = attribute.TimeKind == DateTimeKind.Local
                ? new DateTimeOffset(DateTime.Now)
                : new DateTimeOffset(DateTime.UtcNow);
            if (attribute.OffSet != 0)
            {
                offset = offset.ToOffset(TimeSpan.FromHours(attribute.OffSet));
            }

            UpdatedAtProperty.SetValue(entity, offset.DateTime);
        }

        var query = new SqlQuery(entity);

        //sorry, dapper doesn't support guid mapping in oracle.
        //I can not convert guid to bytearray to build a seperate params, it can not be achieved in this lib.
        //see details below to get more informations.
        //https://github.com/DapperLib/Dapper/issues/633
        //https://github.com/DapperLib/Dapper/issues/637
        //https://github.com/vauto/Dapper.Database/pull/1

        if (Provider == SqlProvider.Oracle)
        {
            var oracleParams = new DynamicParameters(entity);
            oracleParams.AddOracleOutputParameterForId();
            query.SetParam(oracleParams);
        }

        query.SqlBuilder.AppendFormat("INSERT INTO {0} ({1}) VALUES ({2})", TableName, string.Join(", ", properties.Select(p => p.ColumnName)),
            string.Join(", ", properties.Select(p => ParameterSymbol + p.PropertyName))); // values

        if (IsIdentity)
            switch (Provider)
            {
                case SqlProvider.MSSQL:
                    query.SqlBuilder.Append(" SELECT SCOPE_IDENTITY() AS " + IdentitySqlProperty.ColumnName);
                    break;

                case SqlProvider.MySQL:
                    query.SqlBuilder.Append("; SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS " + IdentitySqlProperty.ColumnName);
                    break;

                case SqlProvider.SQLite:
                    query.SqlBuilder.Append("; SELECT LAST_INSERT_ROWID() AS " + IdentitySqlProperty.ColumnName);
                    break;

                case SqlProvider.PostgreSQL:
                    query.SqlBuilder.Append(" RETURNING " + IdentitySqlProperty.ColumnName);
                    break;

                case SqlProvider.Oracle:
                    query.SqlBuilder.Append(" RETURNING " + IdentitySqlProperty.ColumnName + " INTO :newId");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

        return query;
    }
}
