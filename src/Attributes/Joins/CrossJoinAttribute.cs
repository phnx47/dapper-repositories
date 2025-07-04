namespace MicroOrm.Dapper.Repositories.Attributes.Joins;

/// <summary>
/// Generate CROSS JOIN
/// </summary>
public sealed class CrossJoinAttribute : JoinAttributeBase
{
    /// <summary>
    /// Initialize a new instance with default CROSS JOIN type
    /// </summary>
    public CrossJoinAttribute()
        : base("CROSS JOIN")
    {
    }

    /// <summary>
    /// Initialize a new instance with the specified table
    /// </summary>
    /// <param name="tableName">Name of external table</param>
    public CrossJoinAttribute(string tableName)
        : base(tableName, string.Empty, string.Empty, string.Empty, string.Empty, "CROSS JOIN")
    {
    }
}
