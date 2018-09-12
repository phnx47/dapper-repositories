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
        private void InitConfig(SqlGeneratorConfig sqlGeneratorConfig)
        {
            Config = sqlGeneratorConfig;

            if (Config.UseQuotationMarks)
            {
                switch (Config.SqlProvider)
                {
                    case SqlProvider.MSSQL:
                        TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema, "[", "]");

                        foreach (var propertyMetadata in SqlProperties)
                            propertyMetadata.ColumnName = "[" + propertyMetadata.ColumnName + "]";

                        foreach (var propertyMetadata in KeySqlProperties)
                            propertyMetadata.ColumnName = "[" + propertyMetadata.ColumnName + "]";

                        foreach (var propertyMetadata in SqlJoinProperties)
                        {
                            propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema, "[", "]");
                            propertyMetadata.ColumnName = "[" + propertyMetadata.ColumnName + "]";
                            propertyMetadata.TableAlias = "[" + propertyMetadata.TableAlias + "]";
                        }

                        if (IdentitySqlProperty != null)
                            IdentitySqlProperty.ColumnName = "[" + IdentitySqlProperty.ColumnName + "]";

                        break;

                    case SqlProvider.MySQL:
                        TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema, "`", "`");

                        foreach (var propertyMetadata in SqlProperties)
                            propertyMetadata.ColumnName = "`" + propertyMetadata.ColumnName + "`";

                        foreach (var propertyMetadata in KeySqlProperties)
                            propertyMetadata.ColumnName = "`" + propertyMetadata.ColumnName + "`";

                        foreach (var propertyMetadata in SqlJoinProperties)
                        {
                            propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema, "`", "`");
                            propertyMetadata.ColumnName = "`" + propertyMetadata.ColumnName + "`";
                            propertyMetadata.TableAlias = "`" + propertyMetadata.TableAlias + "`";
                        }

                        if (IdentitySqlProperty != null)
                            IdentitySqlProperty.ColumnName = "`" + IdentitySqlProperty.ColumnName + "`";

                        break;

                    case SqlProvider.PostgreSQL:
                        TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema, "\"", "\"");

                        foreach (var propertyMetadata in SqlProperties)
                            propertyMetadata.ColumnName = "\"" + propertyMetadata.ColumnName + "\"";

                        foreach (var propertyMetadata in KeySqlProperties)
                            propertyMetadata.ColumnName = "\"" + propertyMetadata.ColumnName + "\"";

                        foreach (var propertyMetadata in SqlJoinProperties)
                        {
                            propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema, "\"", "\"");
                            propertyMetadata.ColumnName = "\"" + propertyMetadata.ColumnName + "\"";
                            propertyMetadata.TableAlias = "\"" + propertyMetadata.TableAlias + "\"";
                        }

                        if (IdentitySqlProperty != null)
                            IdentitySqlProperty.ColumnName = "\"" + IdentitySqlProperty.ColumnName + "\"";

                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(Config.SqlProvider));
                }
            }
            else
            {
                TableName = GetTableNameWithSchemaPrefix(TableName, TableSchema);
                foreach (var propertyMetadata in SqlJoinProperties)
                    propertyMetadata.TableName = GetTableNameWithSchemaPrefix(propertyMetadata.TableName, propertyMetadata.TableSchema);
            }
        }
        
    }
}