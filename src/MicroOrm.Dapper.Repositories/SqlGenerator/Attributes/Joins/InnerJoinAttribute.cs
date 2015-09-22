namespace MicroOrm.Dapper.Repositories.SqlGenerator.Attributes.Joins
{
    public class InnerJoinAttribute : JoinAttributeBase
    {
        public InnerJoinAttribute(string tableName) 
            : base(tableName)
        {
        }
    }
}
