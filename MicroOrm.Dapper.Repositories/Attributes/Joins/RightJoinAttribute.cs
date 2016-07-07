namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    /// <summary>
    /// Generate RIGHT JOIN
    /// </summary>
    public class RightJoinAttribute : JoinAttributeBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RightJoinAttribute()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        /// <param name="externalKey"></param>
        public RightJoinAttribute(string tableName, string key, string externalKey)
            : base(tableName, key, externalKey)
        {
        }
    }
}
