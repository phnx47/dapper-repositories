using System;
using MicroOrm.Dapper.Repositories.Config;

namespace MicroOrm.Dapper.Repositories.Attributes.Joins
{
    /// <inheritdoc />
    /// <summary>
    ///     Base JOIN for LEFT/INNER/RIGHT
    /// </summary>
    public abstract class JoinAttributeBase : Attribute
    {
        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        protected JoinAttributeBase()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        protected JoinAttributeBase(string tableName, string key, string externalKey, string tableSchema, string tableAlias,
            string attrString = "JOIN")
        {
            TableName = MicroOrmConfig.TablePrefix + tableName;
            Key = key;
            ExternalKey = externalKey;
            TableSchema = tableSchema;
            TableAlias = tableAlias;
            JoinAttribute = attrString;
        }

        /// <summary>
        /// Join attribute string
        /// </summary>
        private string JoinAttribute { get; }

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

        /// <summary>
        ///     Convert to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JoinAttribute;
        }
    }
}
