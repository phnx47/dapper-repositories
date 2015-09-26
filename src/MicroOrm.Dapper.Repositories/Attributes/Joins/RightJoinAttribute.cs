namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    public class RightJoinAttribute : JoinAttributeBase
    {
        public RightJoinAttribute()
        {
        }

        public RightJoinAttribute(string tableName)
            : base(tableName)
        {
        }
    }
}
