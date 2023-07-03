using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

namespace MicroOrm.Dapper.Repositories;

/// <summary>
///     Base ReadOnlyRepository
/// </summary>
public partial class ReadOnlyDapperRepository<TEntity> : IReadOnlyDapperRepository<TEntity>
    where TEntity : class
{
    private IDbConnection? _connection;
    private FilterData? _filterData;

    /// <summary>
    ///     Constructor
    /// </summary>
    public ReadOnlyDapperRepository(IDbConnection connection)
    {
        _connection = connection;
        _filterData = new FilterData();
        SqlGenerator = new SqlGenerator<TEntity>();
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    public ReadOnlyDapperRepository(IDbConnection connection, ISqlGenerator<TEntity> sqlGenerator)
    {
        _connection = connection;
        _filterData = new FilterData();
        SqlGenerator = sqlGenerator;
    }


    public IDbConnection Connection => _connection ?? throw new ObjectDisposedException(GetType().FullName);


    public FilterData FilterData => _filterData ?? throw new ObjectDisposedException(GetType().FullName);


    public ISqlGenerator<TEntity> SqlGenerator { get; }

    private static string GetProperty(Expression expression, Type type)
    {
        var field = (MemberExpression)expression;

        var prop = type.GetProperty(field.Member.Name);
        var declaringType = type.GetTypeInfo();
        var tableAttribute = declaringType.GetCustomAttribute<TableAttribute>();
        var tableName = MicroOrmConfig.TablePrefix + (tableAttribute != null ? tableAttribute.Name : declaringType.Name);

        if (prop == null || prop.GetCustomAttribute<NotMappedAttribute>() != null)
            return string.Empty;

        var name = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;
        return $"{tableName}.{name}";
    }


    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null;

        if (_filterData == null)
            return;

        _filterData.Ordered = false;

        if (_filterData.LimitInfo != null)
        {
            _filterData.LimitInfo = null;
        }

        if (_filterData.OrderInfo != null)
        {
            _filterData.OrderInfo.Columns?.Clear();
            _filterData.OrderInfo.Columns = null;
            _filterData.OrderInfo = null;
        }

        if (_filterData.SelectInfo != null)
        {
            _filterData.SelectInfo.Columns.Clear();
            _filterData.SelectInfo = null;
        }

        _filterData = null;
    }
}
