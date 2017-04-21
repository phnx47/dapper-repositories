using System.Reflection;
using MicroOrm.Dapper.Repositories.Attributes.Joins;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <summary>
    /// </summary>
    public class SqlJoinPropertyMetadata : SqlPropertyMetadata
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="joinPropertyInfo"></param>
        /// <param name="propertyInfo"></param>
        public SqlJoinPropertyMetadata(PropertyInfo joinPropertyInfo, PropertyInfo propertyInfo) : base(propertyInfo)
        {
            var joinAtttribute = joinPropertyInfo.GetCustomAttribute<JoinAttributeBase>();
            JoinPropertyInfo = joinPropertyInfo;
            TableSchema = joinAtttribute.TableSchema;
            TableName = joinAtttribute.TableName;
        }

        /// <summary>
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PropertyInfo JoinPropertyInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string JoinPropertyName => JoinPropertyInfo.Name;

        /// <summary>
        /// 
        /// </summary>
        public override string PropertyName => JoinPropertyName + base.PropertyName;
    }
}