using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MicroOrm.Dapper.Repositories.SqlGenerator.Models
{
    public class PropertyMetadata
    {
        public PropertyInfo PropertyInfo { get; private set; }

        public string Alias { get; private set; }

        public string ColumnName
        {
            get
            {
                return string.IsNullOrEmpty(this.Alias) ? this.PropertyInfo.Name : this.Alias;
            }
        }

        public string Name
        {
            get 
            {
                return PropertyInfo.Name;
            }
        }

        public PropertyMetadata(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            var alias = this.PropertyInfo.GetCustomAttribute<ColumnAttribute>();
            this.Alias = alias != null ? alias.Name : string.Empty;
        }
    }
}
