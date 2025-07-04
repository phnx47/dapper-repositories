using System;
using MicroOrm.Dapper.Repositories.Config;

namespace MicroOrm.Dapper.Repositories.Attributes.Joins;

/// <summary>
///     Base JOIN for LEFT/INNER/RIGHT
/// </summary>
public abstract class JoinAttributeBase : Attribute
{
    /// <summary>
    ///     Constructor
    /// </summary>
    protected JoinAttributeBase(string joinType)
    {
        JoinType = joinType;
    }

    /// <summary>
    ///     Constructor
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
    /// Join attribute string
    /// </summary>
    private string? JoinType { get; }


    private string? _tableName;

    /// <summary>
    ///     Name of external table
    /// </summary>
    public string? TableName
    {
        get
        {
            return _tableName;
        }
        set
        {
            _tableName = MicroOrmConfig.TablePrefix + value;
        }
    }

    /// <summary>
    ///     Name of external table schema
    /// </summary>
    public string? TableSchema { get; set; }

    /// <summary>
    ///     ForeignKey of this table
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    ///     Key of external table
    /// </summary>
    public string? ExternalKey { get; set; }

    /// <summary>
    ///     Table alias
    /// </summary>
    public string? TableAlias { get; set; }

    /// <summary>
    ///     Convert to string
    /// </summary>
    /// <returns></returns>
    public override string? ToString()
    {
        return JoinType;
    }
}
