using System;
using MicroOrm.Dapper.Repositories.Config;

namespace MicroOrm.Dapper.Repositories.Attributes.Joins;

/// <summary>
/// Base class for JOIN attributes
/// </summary>
public abstract class JoinAttributeBase : Attribute
{
    /// <summary>
    /// Initialize a new instance with the specified join type
    /// </summary>
    protected JoinAttributeBase(string joinType)
    {
        JoinType = joinType;
    }

    /// <summary>
    /// Initialize a new instance with full join specification
    /// </summary>
    protected JoinAttributeBase(string tableName, string key, string externalKey, string tableSchema, string tableAlias, string joinType)
        : this(joinType)
    {
        TableName = tableName;
        Key = key;
        ExternalKey = externalKey;
        TableSchema = tableSchema;
        TableAlias = tableAlias;
    }

    /// <summary>
    /// The SQL JOIN type keyword
    /// </summary>
    private string? JoinType { get; }

    private string? _tableName;

    /// <summary>
    /// Name of external table
    /// </summary>
    public string? TableName
    {
        get => _tableName;
        set => _tableName = MicroOrmConfig.TablePrefix + value;
    }

    /// <summary>
    /// Name of external table schema
    /// </summary>
    public string? TableSchema { get; set; }

    /// <summary>
    /// ForeignKey of this table
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Key of external table
    /// </summary>
    public string? ExternalKey { get; set; }

    /// <summary>
    ///     Table alias
    /// </summary>
    public string? TableAlias { get; set; }

    /// <summary>
    /// Returns the join type string
    /// </summary>
    public override string? ToString() => JoinType;
}
