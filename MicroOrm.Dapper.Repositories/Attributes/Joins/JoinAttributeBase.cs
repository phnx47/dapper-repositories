using System;

namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    /// <summary>
    /// Base JOIN for LEFT/INNER/RIGHT
    /// </summary>
    public abstract class JoinAttributeBase : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected JoinAttributeBase()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected JoinAttributeBase(string tableName, string key, string externalKey)
        {
            TableName = tableName;
            Key = key;
            ExternalKey = externalKey;
        }

        /// <summary>
        /// Name of external table
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// ForeignKey of this table
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Key of external table
        /// </summary>
        public string ExternalKey { get; set; }
    }
}