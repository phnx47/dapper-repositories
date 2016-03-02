namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    /// <summary>
    /// 
    /// </summary>
    public class LeftJoinAttribute : JoinAttributeBase
    {
        /// <summary>
        /// 
        /// </summary>
        public LeftJoinAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public LeftJoinAttribute(string tableName, string key, string externalKey) 
            : base(tableName, key, externalKey)
        {
        }
    }
}
