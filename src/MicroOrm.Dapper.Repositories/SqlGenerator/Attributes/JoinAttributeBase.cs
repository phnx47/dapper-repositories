using System;

namespace MicroOrm.Dapper.Repositories.SqlGenerator.Attributes
{
    public class JoinAttribute : Attribute
    {
        protected JoinAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; set; }
    }
}