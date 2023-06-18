namespace MicroOrm.Dapper.Repositories.Attributes.Joins;


/// <summary>
///     Generate CROSS JOIN
/// </summary>
public sealed class CrossJoinAttribute : JoinAttributeBase
{

    /// <summary>
    ///     Constructor
    /// </summary>
    public CrossJoinAttribute()
    {
    }


    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="tableName">Name of external table</param>
    public CrossJoinAttribute(string tableName)
        : base(tableName, string.Empty, string.Empty, string.Empty, string.Empty, "CROSS JOIN")
    {
    }
}
