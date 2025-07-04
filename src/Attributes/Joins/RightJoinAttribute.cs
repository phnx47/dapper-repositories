namespace MicroOrm.Dapper.Repositories.Attributes.Joins;


/// <summary>
///     Generate RIGHT JOIN
/// </summary>
public sealed class RightJoinAttribute : JoinAttributeBase
{

    /// <summary>
    ///     Constructor
    /// </summary>
    public RightJoinAttribute()
        : base("RIGHT JOIN")
    {
    }


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


    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="tableName">Name of external table</param>
    /// <param name="key">ForeignKey of this table</param>
    /// <param name="externalKey">Key of external table</param>
    /// <param name="tableSchema">Name of external table schema</param>
    /// <param name="tableAlias">External table alias</param>
    public RightJoinAttribute(string tableName, string key, string externalKey, string tableSchema, string tableAlias)
        : base(tableName, key, externalKey, tableSchema, tableAlias, "RIGHT JOIN")
    {
    }
}
