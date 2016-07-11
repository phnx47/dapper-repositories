using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{

    /// <summary>
    /// 
    /// </summary>
    public class SqlPropertyMetadata
    {
        /// <summary>
        /// 
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name => PropertyInfo.Name;


        /// <summary>
        /// 
        /// </summary>
        public SqlPropertyMetadata(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            var alias = PropertyInfo.GetCustomAttribute<ColumnAttribute>();
            if (alias != null)
            {

                Alias = alias.Name;
                ColumnName = Alias;
            }
            else
            {
                ColumnName = PropertyInfo.Name;
            }

        }
    }
}