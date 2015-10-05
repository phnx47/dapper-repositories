using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes.LogicalDelete;
using System;
using System.Collections;
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
        #region Constructors

        public SqlGenerator(ESqlConnector sqlConnector)
        {
            SqlConnector = sqlConnector;
            var entityType = typeof(TEntity);
            var aliasAttribute = entityType.GetCustomAttribute<TableAttribute>();
            this.TableName = aliasAttribute != null ? aliasAttribute.Name : entityType.Name;
            AllProperties = entityType.GetProperties();
            //Load all the "primitive" entity properties
            var props = AllProperties.Where(p => p.PropertyType.IsValueType || p.PropertyType.Name.Equals("String", StringComparison.InvariantCultureIgnoreCase)).ToArray();

            //Filter the non stored properties
            this.BaseProperties = props.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new PropertyMetadata(p));

            //Filter key properties
            this.KeyProperties = props.Where(p => p.GetCustomAttributes<KeyAttribute>().Any()).Select(p => new PropertyMetadata(p));

            //Use identity as key pattern
            var identityProperty = props.FirstOrDefault(p => p.GetCustomAttributes<IdentityAttribute>().Any());
            this.IdentityProperty = identityProperty != null ? new PropertyMetadata(identityProperty) : null;

            //Status property (if exists, and if it does, it must be an enumeration)
            var statusProperty = props.FirstOrDefault(p => p.PropertyType.IsEnum && p.GetCustomAttributes<StatusAttribute>().Any());

            if (statusProperty == null) return;

            this.StatusProperty = new PropertyMetadata(statusProperty);
            var statusPropertyType = statusProperty.PropertyType;
            var deleteOption = statusPropertyType.GetFields().FirstOrDefault(f => f.GetCustomAttribute<DeletedAttribute>() != null);

            if (deleteOption == null) return;

            var enumValue = Enum.Parse(statusPropertyType, deleteOption.Name);

            if (enumValue != null)
                this.LogicalDeleteValue = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(statusPropertyType));
        }

        public SqlGenerator()
            : this(ESqlConnector.MSSQL)
        {
        }

        #endregion Constructors

        #region Properties

        public ESqlConnector SqlConnector { get; set; }

        public bool IsIdentity => this.IdentityProperty != null;

        public bool LogicalDelete => StatusProperty != null;

        public string TableName { get; }

        public PropertyInfo[] AllProperties { get; }

        public PropertyMetadata IdentityProperty { get; }

        public IEnumerable<PropertyMetadata> KeyProperties { get; }

        public IEnumerable<PropertyMetadata> BaseProperties { get; }

        public PropertyMetadata StatusProperty { get; }

        public object LogicalDeleteValue { get; }

        #endregion Properties

        #region Query generators

        public virtual QueryResult GetInsert(TEntity entity)
        {
            List<PropertyMetadata> properties = (this.IsIdentity ?
                this.BaseProperties.Where(p => !p.Name.Equals(this.IdentityProperty.Name, StringComparison.InvariantCultureIgnoreCase)) :
                this.BaseProperties).ToList();

            string columNames = string.Join(", ", properties.Select(p => $"{p.ColumnName}"));
            string values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("INSERT INTO {0} {1} {2} ",
                                    this.TableName,
                                    string.IsNullOrEmpty(columNames) ? "" : $"({columNames})",
                                    string.IsNullOrEmpty(values) ? "" : $" VALUES ({values})");

            if (this.IsIdentity)
            {
                switch (SqlConnector)
                {
                    case ESqlConnector.MSSQL:
                        sqlBuilder.Append("SELECT SCOPE_IDENTITY() AS " + this.IdentityProperty.ColumnName);
                        break;

                    case ESqlConnector.MySQL:
                        sqlBuilder.Append("; SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS " + this.IdentityProperty.ColumnName);
                        break;

                    case ESqlConnector.PostgreSQL:
                        sqlBuilder.Append("RETURNING " + this.IdentityProperty.ColumnName);
                        break;
                }
            }

            return new QueryResult(sqlBuilder.ToString(), entity);
        }

        public virtual QueryResult GetUpdate(TEntity entity)
        {
            var properties = this.BaseProperties.Where(p => !this.KeyProperties.Any(k => k.Name.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)));

            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("UPDATE {0} SET {1} WHERE {2}",
                                    this.TableName,
                                    string.Join(", ", properties.Select(p => $"{p.ColumnName} = @{p.Name}")),
                                    string.Join(" AND ", this.KeyProperties.Select(p => $"{p.ColumnName} = @{p.Name}")));

            return new QueryResult(sqlBuilder.ToString().TrimEnd(), entity);
        }

        #region Get Select

        private StringBuilder AppendJoinToSelect(StringBuilder originalBuilder, params Expression<Func<TEntity, object>>[] includes)
        {
            StringBuilder joinsBuilder = new StringBuilder();

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

                    var joinType = joinProperty.PropertyType.IsGenericType 
                        ? joinProperty.PropertyType.GenericTypeArguments[0] 
                        : joinProperty.PropertyType;

                    var properties = joinType.GetProperties().Where(p => p.PropertyType.IsValueType || p.PropertyType.Name.Equals("String", StringComparison.InvariantCultureIgnoreCase));
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
            Func<PropertyMetadata, string> projectionFunction = (p) =>
            {
                if (!string.IsNullOrEmpty(p.Alias))
                    return $"{tableName}.{p.ColumnName} AS {p.Name}";

                return $"{tableName}.{p.ColumnName}";
            };

            return string.Join(", ", properties.Select(projectionFunction));
        }

        public virtual QueryResult GetSelect(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
        {
            var builder = new StringBuilder();

            // convert the query parms into a SQL string and dynamic property object
            builder.Append($"SELECT {GetFieldsSelect(TableName, BaseProperties)}");

            if (includes.Any())
            {
                var joinsBuilder = AppendJoinToSelect(builder, includes);
                builder.Append($" FROM {TableName} ");
                builder.Append(joinsBuilder);
            }
            else
            {
                builder.Append($"FROM {TableName} ");
            }

            if (expression == null)
                return new QueryResult(builder.ToString().TrimEnd(), new ExpandoObject());

            // WHERE
            var queryProperties = new List<QueryParameter>();
            FillQueryProperties(ExpressionHelper.GetBinaryExpression(expression.Body), ExpressionType.Default, ref queryProperties);

            builder.Append(" WHERE ");

            IDictionary<string, object> expando = new ExpandoObject();
            for (int i = 0; i < queryProperties.Count; i++)
            {
                var item = queryProperties[i];

                if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                {
                    builder.Append(string.Format("{0} {1} {2}.{3} @{2} ", item.LinkingOperator, TableName, item.PropertyName, item.QueryOperator));
                }
                else
                {
                    builder.Append(string.Format("{0}.{1} {2} @{1} ", TableName, item.PropertyName, item.QueryOperator));
                }

                expando[item.PropertyName] = item.PropertyValue;
            }

            return new QueryResult(builder.ToString().TrimEnd(), expando);
        }

        public virtual QueryResult GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> expression)
        {
            var filedName = ExpressionHelper.GetPropertyName(btwFiled);
            var queryResult = this.GetSelect(expression);
            var op = expression == null ? "WHERE" : "AND";

            queryResult.AppendToSql($" {op} {filedName} BETWEEN '{@from}' AND '{to}'");

            return queryResult;
        }

        public virtual QueryResult GetDelete(TEntity entity)
        {
            var sqlBuilder = new StringBuilder();

            if (!LogicalDelete)
            {
                sqlBuilder.AppendFormat("DELETE FROM {0} WHERE {1}",
                    this.TableName,
                    string.Join(" AND ",
                        this.KeyProperties.Select(
                            p => $"{p.ColumnName} = @{p.Name}")));
            }
            else
            {
                sqlBuilder.AppendFormat("UPDATE {0} SET {1} WHERE {2}",
                    this.TableName,
                    $"{this.StatusProperty.ColumnName} = {this.LogicalDeleteValue}",
                    string.Join(" AND ",
                        this.KeyProperties.Select(
                            p => $"{p.ColumnName} = @{p.Name}")));
            }

            return new QueryResult(sqlBuilder.ToString(), entity);
        }

        #endregion Get Select

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

        #endregion Query generators
    }
}