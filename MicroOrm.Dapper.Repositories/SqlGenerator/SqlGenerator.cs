using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;
using MicroOrm.Dapper.Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    public class SqlGenerator<TEntity> : ISqlGenerator<TEntity>
        where TEntity : class
    {

        public SqlConnector SqlConnector { get; set; }

        public bool IsIdentity => IdentityProperty != null;

        public string TableName { get; protected set; }


        public PropertyInfo[] AllProperties { get; protected set; }

        public PropertyMetadata IdentityProperty { get; protected set; }

        public PropertyMetadata[] KeyProperties { get; protected set; }

        public PropertyMetadata[] BaseProperties { get; protected set; }

        #region Logic delete

        public bool LogicalDelete { get; protected set; }

        public string StatusPropertyName { get; protected set; }

        public object LogicalDeleteValue { get; protected set; }

        #endregion Logic delete



        #region Init

        public SqlGenerator()
           : this(SqlConnector.MSSQL)
        {
        }

        public SqlGenerator(SqlConnector sqlConnector)
        {


            InitProperties();

            InitSqlConnector(sqlConnector);
            InitLogicalDeleted();

            //todo: init joins

        }

        private void InitProperties()
        {
            var entityType = typeof(TEntity);
            var entityTypeInfo = entityType.GetTypeInfo();
            var tableAliasAttribute = entityTypeInfo.GetCustomAttribute<TableAttribute>();
            TableName = tableAliasAttribute != null ? tableAliasAttribute.Name : entityTypeInfo.Name;
            AllProperties = entityType.GetProperties().ToArray();
            var props = AllProperties.Where(ExpressionHelper.GetPrimitivePropertiesPredicate()).ToArray();

            // Filter the non stored properties
            BaseProperties = props.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new PropertyMetadata(p)).ToArray();

            // Filter key properties
            KeyProperties = props.Where(p => p.GetCustomAttributes<KeyAttribute>().Any()).Select(p => new PropertyMetadata(p)).ToArray();

            // Use identity as key pattern
            var identityProperty = props.FirstOrDefault(p => p.GetCustomAttributes<IdentityAttribute>().Any());
            IdentityProperty = identityProperty != null ? new PropertyMetadata(identityProperty) : null;
        }

        private void InitSqlConnector(SqlConnector sqlConnector)
        {
            SqlConnector = sqlConnector;
            switch (SqlConnector)
            {
                case SqlConnector.MSSQL:
                    TableName = TableName.Insert(0, "[");
                    TableName = TableName.Insert(TableName.Length, "]");

                    foreach (var propertyMetadata in BaseProperties)
                    {
                        propertyMetadata.ColumnName = propertyMetadata.ColumnName.Insert(0, "[");
                        propertyMetadata.ColumnName = propertyMetadata.ColumnName.Insert(propertyMetadata.ColumnName.Length, "]");
                    }

                    foreach (var propertyMetadata in KeyProperties)
                    {
                        propertyMetadata.ColumnName = propertyMetadata.ColumnName.Insert(0, "[");
                        propertyMetadata.ColumnName = propertyMetadata.ColumnName.Insert(propertyMetadata.ColumnName.Length, "]");
                    }

                    break;
                case SqlConnector.MySQL:
                    break;
                case SqlConnector.PostgreSQL:
                    //todo: ковычки
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(SqlConnector));
            }
        }

        private void InitLogicalDeleted()
        {
            var statusProperty =
                BaseProperties.FirstOrDefault(x => x.PropertyInfo.GetCustomAttributes<StatusAttribute>().Any());

            if (statusProperty == null) return;
            StatusPropertyName = statusProperty.ColumnName;

            if (statusProperty.PropertyInfo.PropertyType.IsBool())
            {
                var deleteProperty = AllProperties.FirstOrDefault(p => p.GetCustomAttributes<DeletedAttribute>().Any());
                if (deleteProperty == null) return;

                LogicalDelete = true;
                LogicalDeleteValue = 1; // true
            }
            else if (statusProperty.PropertyInfo.PropertyType.IsEnum())
            {
                var deleteOption = statusProperty.PropertyInfo.PropertyType.GetFields().FirstOrDefault(f => f.GetCustomAttribute<DeletedAttribute>() != null);

                if (deleteOption == null) return;

                var enumValue = Enum.Parse(statusProperty.PropertyInfo.PropertyType, deleteOption.Name);

                if (enumValue != null)
                    LogicalDeleteValue = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(statusProperty.PropertyInfo.PropertyType));

                LogicalDelete = true;
            }
        }

        #endregion

        #region Insert

        public virtual SqlQuery GetInsert(TEntity entity)
        {
            var properties = (this.IsIdentity ? this.BaseProperties.Where(p => !p.Name.Equals(this.IdentityProperty.Name, StringComparison.OrdinalIgnoreCase)) : this.BaseProperties).ToList();

            string columNames = string.Join(", ", properties.Select(p => $"{p.ColumnName}"));
            string values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"INSERT INTO {this.TableName} {(string.IsNullOrEmpty(columNames) ? "" : $"({columNames})")} {(string.IsNullOrEmpty(values) ? "" : $" VALUES ({values})")} ");

            if (this.IsIdentity)
            {
                switch (SqlConnector)
                {
                    case SqlConnector.MSSQL:
                        sqlBuilder.Append("SELECT SCOPE_IDENTITY() AS " + this.IdentityProperty.ColumnName);
                        break;

                    case SqlConnector.MySQL:
                        sqlBuilder.Append("; SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS " + this.IdentityProperty.ColumnName);
                        break;

                    case SqlConnector.PostgreSQL:
                        sqlBuilder.Append("RETURNING " + this.IdentityProperty.ColumnName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new SqlQuery(sqlBuilder.ToString(), entity);
        }

        #endregion

        #region Update

        public virtual SqlQuery GetUpdate(TEntity entity)
        {
            var properties = this.BaseProperties.Where(p => !this.KeyProperties.Any(k => k.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"UPDATE {this.TableName} SET {string.Join(", ", properties.Select(p => $"{p.ColumnName} = @{p.Name}"))} WHERE {string.Join(" AND ", this.KeyProperties.Select(p => $"{p.ColumnName} = @{p.Name}"))}");

            return new SqlQuery(sqlBuilder.ToString().TrimEnd(), entity);
        }

        #endregion

        #region  Select

        public virtual SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, true, includes);
        }

        public virtual SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, false, includes);
        }

        private StringBuilder InitBuilderSelect(bool firstOnly)
        {
            var builder = new StringBuilder();
            var select = "SELECT ";

            if (firstOnly && SqlConnector == SqlConnector.MSSQL)
                select += "TOP 1 ";

            // convert the query parms into a SQL string and dynamic property object
            builder.Append($"{select}{GetFieldsSelect(TableName, BaseProperties)}");

            return builder;
        }

        private StringBuilder AppendJoinToSelect(StringBuilder originalBuilder, params Expression<Func<TEntity, object>>[] includes)
        {
            var joinsBuilder = new StringBuilder();

            foreach (var include in includes)
            {
                var propertyName = ExpressionHelper.GetPropertyName(include);
                var joinProperty = AllProperties.First(x => x.Name == propertyName);
                var attrJoin = joinProperty.GetCustomAttribute<JoinAttributeBase>();
                if (attrJoin != null)
                {
                    var joinString = "";
                    if (attrJoin is LeftJoinAttribute)
                    {
                        joinString = "LEFT JOIN ";
                    }
                    else if (attrJoin is InnerJoinAttribute)
                    {
                        joinString = "INNER JOIN ";
                    }
                    else if (attrJoin is RightJoinAttribute)
                    {
                        joinString = "RIGHT JOIN ";
                    }

                    var joinType = joinProperty.PropertyType.IsGenericType() ? joinProperty.PropertyType.GenericTypeArguments[0] : joinProperty.PropertyType;

                    var properties = joinType.GetProperties().Where(ExpressionHelper.GetPrimitivePropertiesPredicate());
                    var props = properties.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new PropertyMetadata(p));
                    originalBuilder.Append(", " + GetFieldsSelect(attrJoin.TableName, props));


                    joinsBuilder.Append($"{joinString} {attrJoin.TableName} ON {TableName}.{attrJoin.Key} = {attrJoin.TableName}.{attrJoin.ExternalKey} ");
                }
            }
            return joinsBuilder;
        }

        private static string GetFieldsSelect(string tableName, IEnumerable<PropertyMetadata> properties)
        {
            //Projection function
            Func<PropertyMetadata, string> projectionFunction = (p) => !string.IsNullOrEmpty(p.Alias) ? $"{tableName}.{p.ColumnName} AS {p.Name}" : $"{tableName}.{p.ColumnName}";

            return string.Join(", ", properties.Select(projectionFunction));
        }


        private SqlQuery GetSelect(Expression<Func<TEntity, bool>> predicate, bool firstOnly, params Expression<Func<TEntity, object>>[] includes)
        {
            var builder = InitBuilderSelect(firstOnly);

            if (includes.Any())
            {
                var joinsBuilder = AppendJoinToSelect(builder, includes);
                builder.Append($" FROM {TableName} ");
                builder.Append(joinsBuilder);
            }
            else
            {
                builder.Append($" FROM {TableName} ");
            }

            IDictionary<string, object> expando = new ExpandoObject();

            if (predicate != null)
            {
                // WHERE
                var queryProperties = new List<QueryParameter>();
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(predicate.Body), ExpressionType.Default, ref queryProperties);

                builder.Append(" WHERE ");


                for (int i = 0; i < queryProperties.Count; i++)
                {
                    var item = queryProperties[i];
                    var columnName = BaseProperties.First(x => x.Name == item.PropertyName).ColumnName;

                    if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                    {
                        builder.Append($"{item.LinkingOperator} {TableName}.{columnName} {item.QueryOperator} @{item.PropertyName} ");
                    }
                    else
                    {
                        builder.Append($"{TableName}.{columnName} {item.QueryOperator} @{item.PropertyName} ");
                    }

                    expando[item.PropertyName] = item.PropertyValue;
                }

                if (LogicalDelete)
                {
                    builder.Append($"AND {TableName}.{StatusPropertyName} != {LogicalDeleteValue}");
                }
            }
            else
            {
                if (LogicalDelete)
                {
                    builder.Append($"WHERE {TableName}.{StatusPropertyName} != {LogicalDeleteValue}");
                }
            }

            if (firstOnly && (SqlConnector == SqlConnector.MySQL || SqlConnector == SqlConnector.PostgreSQL))
                builder.Append("LIMIT 1");


            return new SqlQuery(builder.ToString().TrimEnd(), expando);
        }

        public virtual SqlQuery GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression)
        {
            var filedName = ExpressionHelper.GetPropertyName(btwField);
            var queryResult = GetSelectAll(expression);
            var op = expression == null ? "WHERE" : "AND";

            queryResult.AppendToSql($" {op} {filedName} BETWEEN '{from}' AND '{to}'");

            return queryResult;
        }

        public virtual SqlQuery GetDelete(TEntity entity)
        {
            var sqlBuilder = new StringBuilder();

            if (!LogicalDelete)
            {
                sqlBuilder.Append($"DELETE FROM {this.TableName} WHERE {string.Join(" AND ", KeyProperties.Select(p => $"{p.ColumnName} = @{p.Name}"))}");
            }
            else
            {
                sqlBuilder.Append($"UPDATE {this.TableName} SET {this.StatusPropertyName} = {LogicalDeleteValue} WHERE {string.Join(" AND ", this.KeyProperties.Select(p => $"{p.ColumnName} = @{p.Name}"))}");
            }

            return new SqlQuery(sqlBuilder.ToString(), entity);
        }

        /// <summary>
        /// Fill query properties
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="linkingType">Type of the linking.</param>
        /// <param name="queryProperties">The query properties.</param>
        private static void FillQueryProperties(BinaryExpression body, ExpressionType linkingType, ref List<QueryParameter> queryProperties)
        {
            if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
            {
                string propertyName = ExpressionHelper.GetPropertyName(body);
                object propertyValue = ExpressionHelper.GetValue(body.Right);
                string opr = ExpressionHelper.GetSqlOperator(body.NodeType);
                string link = ExpressionHelper.GetSqlOperator(linkingType);

                queryProperties.Add(new QueryParameter(link, propertyName, propertyValue, opr));
            }
            else
            {
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(body.Left), body.NodeType, ref queryProperties);
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(body.Right), body.NodeType, ref queryProperties);
            }
        }

        #endregion  Select
    }
}