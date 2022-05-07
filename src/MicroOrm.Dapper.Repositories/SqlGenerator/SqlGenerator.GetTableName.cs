using System;
using MicroOrm.Dapper.Repositories.Attributes.Joins;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity>
        where TEntity : class
    {
        private static string GetTableNameWithSchemaPrefix(string? tableName, string? tableSchema, string startQuotationMark = "", string endQuotationMark = "")
        {
            return !string.IsNullOrEmpty(tableSchema)
                ? startQuotationMark + tableSchema + endQuotationMark + "." + startQuotationMark + tableName + endQuotationMark
                : startQuotationMark + tableName + endQuotationMark;
        }

        private string GetTableNameWithQuotes(JoinAttributeBase attrJoin, SqlPropertyMetadata[] props, string tableName)
        {
            switch (Provider)
            {
                case SqlProvider.MSSQL:
                    tableName = "[" + tableName + "]";
                    attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "[", "]");
                    attrJoin.Key = "[" + attrJoin.Key + "]";
                    attrJoin.ExternalKey = "[" + attrJoin.ExternalKey + "]";
                    attrJoin.TableAlias = string.IsNullOrEmpty(attrJoin.TableAlias) ? string.Empty : "[" + attrJoin.TableAlias + "]";
                    foreach (var prop in props)
                    {
                        prop.ColumnName = "[" + prop.CleanColumnName + "]";
                    }

                    break;

                case SqlProvider.MySQL:
                    tableName = "`" + tableName + "`";
                    attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "`", "`");
                    attrJoin.Key = "`" + attrJoin.Key + "`";
                    attrJoin.ExternalKey = "`" + attrJoin.ExternalKey + "`";
                    attrJoin.TableAlias = string.IsNullOrEmpty(attrJoin.TableAlias) ? string.Empty : "`" + attrJoin.TableAlias + "`";
                    foreach (var prop in props)
                    {
                        prop.ColumnName = "`" + prop.CleanColumnName + "`";
                    }

                    break;
                
                case SqlProvider.PostgreSQL:
                    tableName = "\"" + tableName + "\"";
                    attrJoin.TableName = GetTableNameWithSchemaPrefix(attrJoin.TableName, attrJoin.TableSchema, "\"", "\"");
                    attrJoin.Key = "\"" + attrJoin.Key + "\"";
                    attrJoin.ExternalKey = "\"" + attrJoin.ExternalKey + "\"";
                    attrJoin.TableAlias = string.IsNullOrEmpty(attrJoin.TableAlias) ? string.Empty : "\"" + attrJoin.TableAlias + "\"";
                    foreach (var prop in props)
                    {
                        prop.ColumnName = "\"" + prop.CleanColumnName + "\"";
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Provider));
            }

            return tableName;
        }
    }
}
