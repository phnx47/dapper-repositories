namespace MicroOrm.Dapper.Repositories.Attributes.Joins;

/// <inheritdoc />
/// <summary>
///     Generate RIGHT JOIN
/// </summary>
public sealed class RightJoinAttribute : JoinAttributeBase
{
    /// <inheritdoc />
    /// <summary>
    ///     Constructor
    /// </summary>
    public RightJoinAttribute()
    {
    }

    /// <inheritdoc />
    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="tableName">Name of external table</param>
    /// <param name="key">ForeignKey of this table</param>
    /// <param name="externalKey">Key of external table</param>
    public RightJoinAttribute(string tableName, string key, string externalKey)
        : base(tableName, key, externalKey, string.Empty, string.Empty, "RIGHT JOIN")
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
    public RightJoinAttribute(string tableName, string key, string externalKey, string tableSchema)
        : base(tableName, key, externalKey, tableSchema, string.Empty, "RIGHT JOIN")
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
    public RightJoinAttribute(string tableName, string key, string externalKey, string tableSchema, string tableAbbreviation)
        : base(tableName, key, externalKey, tableSchema, tableAbbreviation, "RIGHT JOIN")
    {
    }
}
