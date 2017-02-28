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
        /// <param name="tableName">Name of external table</param>
        /// <param name="key">ForeignKey of this table</param>
        /// <param name="externalKey">Key of external table</param>
        public InnerJoinAttribute(string tableName, string key, string externalKey)
            : base(tableName, key, externalKey)
        {
        }
    }
}
