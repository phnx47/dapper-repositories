using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;

namespace MicroOrm.Dapper.Repositories.SqlGenerator;

public partial class SqlGenerator<TEntity>
    where TEntity : class
{
    private void InitLogicalDeletion()
    {
        var statusProperty =
            SqlProperties.FirstOrDefault(x => x.PropertyInfo.GetCustomAttribute<StatusAttribute>() != null);

        foreach (var property in AllProperties)
        {
            var joinAttr = property.GetCustomAttribute<JoinAttributeBase>();
            if (joinAttr?.TableName == null)
                continue;

            var deleteProperty = property.PropertyType.GetProperties()
                .FirstOrDefault(x => x.GetCustomAttribute<DeletedAttribute>() != null);
            if (deleteProperty == null)
                continue;

            JoinLogicalDeleteProperties ??= new Dictionary<string, PropertyInfo>();

#if (NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
            JoinLogicalDeleteProperties.TryAdd(joinAttr.TableName, deleteProperty);
#else
            if (!JoinLogicalDeleteProperties.ContainsKey(joinAttr.TableName))
                JoinLogicalDeleteProperties.Add(joinAttr.TableName, deleteProperty);
#endif
        }


        if (statusProperty == null)
            return;

        StatusPropertyName = statusProperty.ColumnName;

        if (statusProperty.PropertyInfo.PropertyType == typeof(bool))
        {
            LogicalDelete = true;
            LogicalDeleteValue = Provider == SqlProvider.PostgreSQL ? "true" : 1;
        }
        else if (statusProperty.PropertyInfo.PropertyType == typeof(bool?))
        {
            LogicalDelete = true;
            LogicalDeleteValue = Provider == SqlProvider.PostgreSQL ? "true" : 1;
            LogicalDeleteValueNullable = true;
        }
        else if (statusProperty.PropertyInfo.PropertyType.IsEnum)
        {
            var deleteField = statusProperty.PropertyInfo.PropertyType.GetFields()
                .FirstOrDefault(f => f.GetCustomAttribute<DeletedAttribute>() != null);

            if (deleteField == null)
                return;

            var enumValue = Enum.Parse(statusProperty.PropertyInfo.PropertyType, deleteField.Name);
            LogicalDeleteValue = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(statusProperty.PropertyInfo.PropertyType));

            LogicalDelete = true;
        }
    }
}
