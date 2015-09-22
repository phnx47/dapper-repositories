using System;

namespace MicroOrm.Dapper.Repositories.SqlGenerator.Attributes.Joins
{
    public abstract class JoinAttributeBase : Attribute
    {
        protected JoinAttributeBase(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; set; }
    }
}