namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    /// <summary>
    /// Generate INNER JOIN
    /// </summary>
    public class InnerJoinAttribute : JoinAttributeBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InnerJoinAttribute()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InnerJoinAttribute(string tableName, string key, string externalKey)
            : base(tableName, key, externalKey)
        {
        }
    }
}
