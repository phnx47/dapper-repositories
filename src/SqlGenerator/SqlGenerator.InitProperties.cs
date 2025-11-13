using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.Extensions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator;

public partial class SqlGenerator<TEntity>
    where TEntity : class
{
    private void InitProperties()
    {
        var entityType = typeof(TEntity);
        var entityTypeInfo = entityType.GetTypeInfo();
        var tableAttribute = entityTypeInfo.GetCustomAttribute<TableAttribute>();

        TableName = MicroOrmConfig.TablePrefix + (tableAttribute != null ? tableAttribute.Name : entityTypeInfo.Name);

        TableSchema = tableAttribute != null ? tableAttribute.Schema : string.Empty;

        AllProperties = entityType.FindClassProperties().Where(q => q.CanWrite).ToArray();
        InitSqlJoinProperties();

        var props = entityType.FindClassPrimitiveProperties();


        // Filter the non stored properties
        SqlProperties = props.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();

        // Filter key properties
        KeySqlProperties = props.Where(p => p.GetCustomAttributes<KeyAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();

        // Use identity as key pattern
        var identityProperty = props.FirstOrDefault(p => p.GetCustomAttributes<IdentityAttribute>().Any());
        if (identityProperty == null && MicroOrmConfig.AllowKeyAsIdentity)
        {
            identityProperty = props.FirstOrDefault(p => p.GetCustomAttributes<KeyAttribute>().Any());
        }

        IdentitySqlProperty = identityProperty != null ? new SqlPropertyMetadata(identityProperty) : null;

        var dateChangedProperty = props.FirstOrDefault(p => p.GetCustomAttributes<UpdatedAtAttribute>().Any());
        if (dateChangedProperty != null && (dateChangedProperty.PropertyType == typeof(DateTime) || dateChangedProperty.PropertyType == typeof(DateTime?)))
        {
            UpdatedAtProperty = dateChangedProperty;
            UpdatedAtPropertyMetadata = new SqlPropertyMetadata(UpdatedAtProperty);
        }
    }

    /// <summary>
    ///     Init join/nested properties
    /// </summary>
    private void InitSqlJoinProperties()
    {
        // Filter and get only non collection nested properties
        var singleJoinTypes = AllProperties
            .Where(p => p.GetCustomAttributes<JoinAttributeBase>().Any())
            .Where(p => !p.PropertyType.IsConstructedGenericType);

        var joinPropertyMetadatas = new List<SqlJoinPropertyMetadata>();

        foreach (var propertyInfo in singleJoinTypes)
        {
            joinPropertyMetadatas.AddRange(propertyInfo.PropertyType.GetProperties()
                .Where(q => q.CanWrite)
                .Where(ExpressionHelper.GetPrimitivePropertiesPredicate())
                .Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any())
                .Select(p => new SqlJoinPropertyMetadata(propertyInfo, p)));
        }

        SqlJoinProperties = joinPropertyMetadatas.ToArray();
    }
}
