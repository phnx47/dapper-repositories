namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    /// <inheritdoc />
    /// <summary>
    ///     Generate INNER JOIN
    /// </summary>
    public sealed class InnerJoinAttribute : JoinAttributeBase
    {
        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        public InnerJoinAttribute()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="tableName">Name of external table</param>
        /// <param name="key">ForeignKey of this table</param>
        /// <param name="externalKey">Key of external table</param>
        public InnerJoinAttribute(string tableName, string key, string externalKey)
            : base(tableName, key, externalKey, string.Empty, string.Empty, "INNER JOIN")
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="tableName">Name of external table</param>
        /// <param name="key">ForeignKey of this table</param>
        /// <param name="externalKey">Key of external table</param>
        /// <param name="tableSchema">Name of external table schema</param>
        public InnerJoinAttribute(string tableName, string key, string externalKey, string tableSchema)
            : base(tableName, key, externalKey, tableSchema, string.Empty, "INNER JOIN")
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="tableName">Name of external table</param>
        /// <param name="key">ForeignKey of this table</param>
        /// <param name="externalKey">Key of external table</param>
        /// <param name="tableSchema">Name of external table schema</param>
        /// <param name="tableAbbreviation">External table alias</param>
        public InnerJoinAttribute(string tableName, string key, string externalKey, string tableSchema, string tableAbbreviation)
            : base(tableName, key, externalKey, tableSchema, tableAbbreviation, "INNER JOIN")
        {
        }
    }
}
