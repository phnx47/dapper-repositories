using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    /// <summary>
    ///     Base JOIN for LEFT/INNER/RIGHT
    /// </summary>
    public abstract class JoinAttributeBase : Attribute
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        protected JoinAttributeBase()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected JoinAttributeBase(string tableName, string key, string externalKey, string tableSchema, string tableAlias)
        {
            TableName = tableName;
            Key = key;
            ExternalKey = externalKey;
            TableSchema = tableSchema;
            TableAlias = tableAlias == string.Empty ? GetAlias() : tableAlias;
        }

        private string GetAlias()
        {
            return $"{TableName}_{Key}";
        }

        /// <summary>
        ///     Name of external table
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///     Name of external table schema
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        ///     ForeignKey of this table
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Key of external table
        /// </summary>
        public string ExternalKey { get; set; }

        /// <summary>
        ///     Table abbreviation override
        /// </summary>
        public string TableAlias { get; set; }
    }
}