namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    /// <summary>
    /// Generate LEFT JOIN
    /// </summary>
    public class LeftJoinAttribute : JoinAttributeBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LeftJoinAttribute()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LeftJoinAttribute(string tableName, string key, string externalKey) 
            : base(tableName, key, externalKey)
        {
        }
    }
}
