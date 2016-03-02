using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    public class PropertyMetadata
    {
        public PropertyInfo PropertyInfo { get; }

        public string Alias { get; set; }

        public string ColumnName { get; set; }

        public string Name => PropertyInfo.Name;

        public PropertyMetadata(PropertyInfo propertyInfo)
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