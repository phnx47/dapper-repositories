using System;

namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    public abstract class JoinAttributeBase : Attribute
    {
        protected JoinAttributeBase(string tableName)
        {
            TableName = tableName;
        }

        protected JoinAttributeBase()
        {
        }

        public string TableName { get; set; }

    }
}