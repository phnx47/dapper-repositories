using System;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
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
                        InitMetaData("`", "`");
                        break;

                    case SqlProvider.PostgreSQL:
                        InitMetaData("\"", "\"");
                        break;
                    case SqlProvider.SQLite:
                        //SQLite doesn't use it.
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Provider));
                }
            }
            else
            {
                TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema);
                foreach (var propertyMetadata in SqlJoinProperties)
                    propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema);
            }
        }

        private void InitMetaData(string startQuotationMark, string endQuotationMark)
        {
            TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema, startQuotationMark, endQuotationMark);

            foreach (var propertyMetadata in SqlProperties)
                propertyMetadata.ColumnName = startQuotationMark + propertyMetadata.CleanColumnName + endQuotationMark;

            foreach (var propertyMetadata in KeySqlProperties)
                propertyMetadata.ColumnName = startQuotationMark + propertyMetadata.CleanColumnName + endQuotationMark;

            foreach (var propertyMetadata in SqlJoinProperties)
            {
                propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema, startQuotationMark, endQuotationMark);
                propertyMetadata.ColumnName = startQuotationMark + propertyMetadata.CleanColumnName + endQuotationMark;
                propertyMetadata.TableAlias = string.IsNullOrEmpty(propertyMetadata.TableAlias) ? string.Empty : startQuotationMark + propertyMetadata.TableAlias + endQuotationMark;
            }

            if (IdentitySqlProperty != null)
                IdentitySqlProperty.ColumnName = startQuotationMark + IdentitySqlProperty.CleanColumnName + endQuotationMark;
        }
    }
}
