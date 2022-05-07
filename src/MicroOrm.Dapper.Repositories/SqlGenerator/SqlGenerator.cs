using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Config;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity> : ISqlGenerator<TEntity>
        where TEntity : class
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public SqlGenerator()
        {
            Provider = MicroOrmConfig.SqlProvider;

            UseQuotationMarks ??= Provider != SqlProvider.SQLite && Provider != SqlProvider.Oracle && MicroOrmConfig.UseQuotationMarks;

            Initialize();
        }

        private void Initialize()
        {
            // Order is important
            InitProperties();
            InitConfig();
            InitLogicalDeleted();
        }

        /// <summary>
        /// Constructor with params
        /// </summary>
        public SqlGenerator(SqlProvider provider, bool useQuotationMarks)
        {
            Provider = provider;
            UseQuotationMarks = provider != SqlProvider.SQLite && provider != SqlProvider.Oracle && useQuotationMarks;
            Initialize();
        }

        /// <summary>
        /// Constructor with params
        /// </summary>
        public SqlGenerator(SqlProvider provider)
        {
            Provider = provider;
            UseQuotationMarks = false;
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public SqlProvider Provider { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool? UseQuotationMarks { get; set; }

        /// <inheritdoc />
        public PropertyInfo[] AllProperties { get; protected set; } = Array.Empty<PropertyInfo>();

        /// <inheritdoc />
        [MemberNotNullWhen(true, nameof(UpdatedAtProperty), nameof(UpdatedAtPropertyMetadata))]
        public bool HasUpdatedAt => UpdatedAtProperty != null;

        /// <inheritdoc />
        public PropertyInfo? UpdatedAtProperty { get; protected set; }

        /// <inheritdoc />
        public SqlPropertyMetadata? UpdatedAtPropertyMetadata { get; protected set; }

        /// <inheritdoc />
        [MemberNotNullWhen(true, nameof(IdentitySqlProperty))]
        public bool IsIdentity => IdentitySqlProperty != null;

        /// <inheritdoc />
        public string TableName { get; protected set; } = string.Empty;

        /// <inheritdoc />
        public string? TableSchema { get; protected set; }

        /// <inheritdoc />
        public SqlPropertyMetadata? IdentitySqlProperty { get; protected set; }

        /// <inheritdoc />
        public SqlPropertyMetadata[] KeySqlProperties { get; protected set; } = Array.Empty<SqlPropertyMetadata>();

        /// <inheritdoc />
        public SqlPropertyMetadata[] SqlProperties { get; protected set; } = Array.Empty<SqlPropertyMetadata>();

        /// <inheritdoc />
        public SqlJoinPropertyMetadata[] SqlJoinProperties { get; protected set; }= Array.Empty<SqlJoinPropertyMetadata>();

        /// <inheritdoc />
        public bool LogicalDelete { get; protected set; }

        /// <inheritdoc />
        public Dictionary<string, PropertyInfo>? JoinsLogicalDelete { get; protected set; }

        /// <inheritdoc />
        public string? StatusPropertyName { get; protected set; }

        /// <inheritdoc />
        public object? LogicalDeleteValue { get; protected set; }

        /// <summary>
        ///     In Oracle parameter should be build with : instead of @. 
        /// </summary>
        public string ParameterSymbol { get; protected set; } = "@";

        private enum QueryType
        {
            Select,
            Delete,
            Update
        }
    }
}
