namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    public class LeftJoinAttribute : JoinAttributeBase
    {
        public LeftJoinAttribute()
        {
        }

        public LeftJoinAttribute(string tableName, string key, string externalKey) 
            : base(tableName, key, externalKey)
        {
        }
    }
}
