using MicroOrm.Dapper.Repositories.SqlGenerator.Attributes;
using MicroOrm.Dapper.Repositories.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MicroOrm.Dapper.Repositories.SqlGenerator.Models
{
    public class SqlGenerator<TEntity> : ISqlGenerator<TEntity> where TEntity : new()
    {
        #region Constructors

        public SqlGenerator(ESqlConnector sqlConnector)
        {
            SqlConnector = sqlConnector;

            var entityType = typeof(TEntity);

            var aliasAttribute = entityType.GetCustomAttribute<TableAttribute>();
            this.TableName = aliasAttribute != null ? aliasAttribute.Name : entityType.Name;

            //Load all the "primitive" entity properties
            PropertyInfo[] props = entityType.GetProperties().Where(p => p.PropertyType.IsValueType || p.PropertyType.Name.Equals("String", StringComparison.InvariantCultureIgnoreCase)).ToArray();

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

        public PropertyMetadata IdentityProperty { get; }

        public IEnumerable<PropertyMetadata> KeyProperties { get; private set; }

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

            string columNames = String.Join(", ", properties.Select(p => $"{p.ColumnName}"));
            string values = String.Join(", ", properties.Select(p => $"@{p.Name}"));

            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("INSERT INTO {0} {1} {2} ",
                                    this.TableName,
                                    String.IsNullOrEmpty(columNames) ? String.Empty : $"({columNames})",
                                    String.IsNullOrEmpty(values) ? String.Empty : $" VALUES ({values})");

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
                                    String.Join(", ", properties.Select(p => $"{p.ColumnName} = @{p.Name}")),
                                    String.Join(" AND ", this.KeyProperties.Select(p => $"{p.ColumnName} = @{p.Name}")));

            return new QueryResult(sqlBuilder.ToString().TrimEnd(), entity);
        }

        #region Get Select

        private StringBuilder GetBaseBuilderSelect()
        {
            var builder = new StringBuilder();

            //Projection function
            Func<PropertyMetadata, string> projectionFunction = (p) =>
            {
                if (!String.IsNullOrEmpty(p.Alias))
                    return $"{p.ColumnName} AS {p.Name}";

                return $"{p.ColumnName}";
            };

            // convert the query parms into a SQL string and dynamic property object
            builder.AppendFormat("SELECT {0} FROM {1}",
                                    String.Join(", ", this.BaseProperties.Select(projectionFunction)),
                                    this.TableName);

            return builder;
        }

        public virtual QueryResult GetSelect()
        {
            var builder = GetBaseBuilderSelect();
            return new QueryResult(builder.ToString().TrimEnd(), new ExpandoObject());
        }

        public virtual QueryResult GetSelect(Expression<Func<TEntity, bool>> expression)
        {
            var builder = GetBaseBuilderSelect();

            // walk the tree and build up a list of query parameter objects
            // from the left and right branches of the expression tree
            var queryProperties = new List<QueryParameter>();
            FillQueryProperties(ExpressionHelper.GetBinaryExpression(expression.Body), ExpressionType.Default, ref queryProperties);

            builder.Append(" WHERE ");

            IDictionary<string, object> expando = new ExpandoObject();
            for (int i = 0; i < queryProperties.Count; i++)
            {
                var item = queryProperties[i];

                if (!String.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                {
                    builder.Append(String.Format("{0} {1} {2} @{1} ", item.LinkingOperator, item.PropertyName,
                        item.QueryOperator));
                }
                else
                {
                    builder.Append(String.Format("{0} {1} @{0} ", item.PropertyName, item.QueryOperator));
                }

                expando[item.PropertyName] = item.PropertyValue;
            }

            return new QueryResult(builder.ToString().TrimEnd(), expando);
        }

        public virtual QueryResult GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> expression)
        {
            string op;
            QueryResult queryResult;

            var filedName = ExpressionHelper.GetPropertyName(btwFiled);

            if (expression == null)
            {
                queryResult = this.GetSelect();
                op = "WHERE";
            }
            else
            {
                queryResult = this.GetSelect(expression);
                op = "AND";
            }

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
                    String.Join(" AND ",
                        this.KeyProperties.Select(
                            p => $"{p.ColumnName} = @{p.Name}")));
            }
            else
            {
                sqlBuilder.AppendFormat("UPDATE {0} SET {1} WHERE {2}",
                    this.TableName,
                    $"{this.StatusProperty.ColumnName} = {this.LogicalDeleteValue}",
                    String.Join(" AND ",
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
                string opr = ExpressionHelper.GetOperator(body.NodeType);
                string link = ExpressionHelper.GetOperator(linkingType);

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