using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity>
        where TEntity : class
    {
        private void InitLogicalDeleted()
        {
            var statusProperty =
                SqlProperties.FirstOrDefault(x => x.PropertyInfo.GetCustomAttribute<StatusAttribute>() != null);

            foreach (var property in AllProperties)
            {
                var joinAttr = property.GetCustomAttribute<JoinAttributeBase>();
                if (joinAttr == null)
                    continue;

                //var deleted = joinProperty.JoinPropertyInfo.PropertyType.GetCustomAttribute<DeletedAttribute>();
                var deleteAttr = property.PropertyType.GetProperties().FirstOrDefault(x => x.GetCustomAttribute<DeletedAttribute>() != null);
                if (deleteAttr == null)
                    continue;

                if (JoinsLogicalDelete == null)
                    JoinsLogicalDelete = new Dictionary<string, PropertyInfo>();

                JoinsLogicalDelete.Add(joinAttr.TableName, deleteAttr);
            }


            if (statusProperty == null)
                return;
            StatusPropertyName = statusProperty.ColumnName;

            if (statusProperty.PropertyInfo.PropertyType == typeof(bool))
            {
                LogicalDelete = true;
                LogicalDeleteValue = 1; // true
            }
            else if (statusProperty.PropertyInfo.PropertyType.IsEnum)
            {
                var deleteOption = statusProperty.PropertyInfo.PropertyType.GetFields().FirstOrDefault(f => f.GetCustomAttribute<DeletedAttribute>() != null);

                if (deleteOption == null)
                    return;

                var enumValue = Enum.Parse(statusProperty.PropertyInfo.PropertyType, deleteOption.Name);
                LogicalDeleteValue = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(statusProperty.PropertyInfo.PropertyType));

                LogicalDelete = true;
            }
        }
    }
}
