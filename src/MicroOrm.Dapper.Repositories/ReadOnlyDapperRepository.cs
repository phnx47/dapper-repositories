using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

namespace MicroOrm.Dapper.Repositories
{
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
            SqlGenerator = new SqlGenerator<TEntity>();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public ReadOnlyDapperRepository(IDbConnection connection, ISqlGenerator<TEntity> sqlGenerator)
        {
            _connection = connection;
            SqlGenerator = sqlGenerator;
        }

        /// <inheritdoc />
        public IDbConnection Connection
        {
            get => _connection ?? throw new ObjectDisposedException(GetType().FullName);
            set => _connection = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        public FilterData FilterData
        {
            get => _filterData ??= new FilterData();
            set => _filterData = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Dispose()
        {
            _connection?.Dispose();
            _connection = null;
            if (_filterData == null)
                return;
            FilterData.LimitInfo = null;
            if (FilterData.OrderInfo != null)
            {
                FilterData.OrderInfo.Columns?.Clear();
                FilterData.OrderInfo.Columns = null;
                FilterData.OrderInfo = null;
            }

            if (FilterData.SelectInfo != null)
            {
                FilterData.SelectInfo.Columns.Clear();
                FilterData.SelectInfo = null;
            }

            _filterData = null;
        }
    }
}
