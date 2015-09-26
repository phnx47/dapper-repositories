namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    public class LeftJoinAttribute : JoinAttributeBase
    {
        public LeftJoinAttribute()
        {
        }

        public LeftJoinAttribute(string tableName) 
            : base(tableName)
        {
        }
    }
}
