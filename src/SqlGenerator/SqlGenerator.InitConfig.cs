using Dapper;
using System;
using System.Data;

namespace MicroOrm.Dapper.Repositories.SqlGenerator;


public partial class SqlGenerator<TEntity>
    where TEntity : class
{
    /// <summary>
    ///     Init type Sql provider
    /// </summary>
    private void InitConfig()
    {
        if (UseQuotationMarks == true)
        {
            switch (Provider)
            {
                case SqlProvider.MSSQL:
                    InitMetaData("[", "]");
                    break;

                case SqlProvider.MySQL:
                case SqlProvider.SQLite:
                    InitMetaData("`", "`");
                    break;

                case SqlProvider.PostgreSQL:
                    InitMetaData("\"", "\"");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Provider));
            }
        }
        else
        {
            TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema, Provider);
            foreach (var propertyMetadata in SqlJoinProperties)
                propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema, Provider);
        }

        // set ParameterSymbol with : and mapping Boolean type to Int
        if (Provider == SqlProvider.Oracle)
        {
            ParameterSymbol = ":";
            SqlMapper.AddTypeMap(typeof(bool), DbType.Int32);
        }
    }

    private void InitMetaData(string startQuotationMark, string endQuotationMark)
    {
        TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema, Provider, startQuotationMark, endQuotationMark);

        foreach (var propertyMetadata in SqlProperties)
            propertyMetadata.ColumnName = startQuotationMark + propertyMetadata.CleanColumnName + endQuotationMark;

        foreach (var propertyMetadata in KeySqlProperties)
            propertyMetadata.ColumnName = startQuotationMark + propertyMetadata.CleanColumnName + endQuotationMark;

        foreach (var propertyMetadata in SqlJoinProperties)
        {
            propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema, Provider, startQuotationMark, endQuotationMark);
            propertyMetadata.ColumnName = startQuotationMark + propertyMetadata.CleanColumnName + endQuotationMark;
            propertyMetadata.TableAlias =
                string.IsNullOrEmpty(propertyMetadata.TableAlias) ? string.Empty : startQuotationMark + propertyMetadata.TableAlias + endQuotationMark;
        }

        if (IdentitySqlProperty != null)
            IdentitySqlProperty.ColumnName = startQuotationMark + IdentitySqlProperty.CleanColumnName + endQuotationMark;
    }
}
