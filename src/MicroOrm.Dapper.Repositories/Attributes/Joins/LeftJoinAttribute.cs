namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    /// <summary>
    ///     Generate LEFT JOIN
    /// </summary>
    public class LeftJoinAttribute : JoinAttributeBase
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public LeftJoinAttribute()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="tableName">Name of external table</param>
        /// <param name="key">ForeignKey of this table</param>
        /// <param name="externalKey">Key of external table</param>
        public LeftJoinAttribute(string tableName, string key, string externalKey)
            : base(tableName, key, externalKey)
        {
        }
    }
}