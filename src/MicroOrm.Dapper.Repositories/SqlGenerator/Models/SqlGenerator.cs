using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MicroOrm.Dapper.Repositories.SqlGenerator.Attributes;
using MicroOrm.Dapper.Repositories.SqlGenerator.Interfaces;

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

            string columNames = string.Join(", ", properties.Select(p => $"{p.ColumnName}"));
            string values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("INSERT INTO {0} {1} {2} ",
                                    this.TableName,
                                    string.IsNullOrEmpty(columNames) ? string.Empty : $"({columNames})",
                                    string.IsNullOrEmpty(values) ? string.Empty : $" VALUES ({values})");

            if (this.IsIdentity)
            {
                switch (SqlConnector)
                {
                    case ESqlConnector.MSSQL:
                        sqlBuilder.Append("DECLARE @NEWID NUMERIC(38, 0)");
                        sqlBuilder.Append("SET	@NEWID = SCOPE_IDENTITY()");
                        sqlBuilder.Append("SELECT @NEWID");
                        break;

                    case ESqlConnector.MySQL:
                        sqlBuilder.Append("; SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS Id;");
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

        public virtual QueryResult GetSelect()
        {
            IDictionary<string, object> expando = new ExpandoObject();
            var builder = new StringBuilder();

            //Projection function
            Func<PropertyMetadata, string> projectionFunction = (p) =>
            {
                if (!string.IsNullOrEmpty(p.Alias))
                    return $"{p.ColumnName} AS {p.Name}";

                return $"{p.ColumnName}";
            };

            // convert the query parms into a SQL string and dynamic property object
            builder.AppendFormat("SELECT {0} FROM {1}",
                                    string.Join(", ", this.BaseProperties.Select(projectionFunction)),
                                    this.TableName);

            return new QueryResult(builder.ToString().TrimEnd(), expando);
        }

        public virtual QueryResult GetSelect(Expression<Func<TEntity, bool>> expression)
        {
            var queryProperties = new List<QueryParameter>();



            IDictionary<string, object> expando = new ExpandoObject();
            var builder = new StringBuilder();

            // walk the tree and build up a list of query parameter objects
            // from the left and right branches of the expression tree

            FillQueryProperties(GetBinaryExpression(expression.Body), ExpressionType.Default, ref queryProperties);

            //Projection function
            Func<PropertyMetadata, string> projectionFunction = (p) =>
            {
                if (!string.IsNullOrEmpty(p.Alias))
                    return $"{p.ColumnName} AS {p.Name}";

                return $"{p.ColumnName}";
            };

            // convert the query parms into a SQL string and dynamic property object
            builder.AppendFormat("SELECT {0} FROM {1}",
                                    string.Join(", ", this.BaseProperties.Select(projectionFunction)),
                                    this.TableName);


            builder.Append(" WHERE ");
            for (int i = 0; i < queryProperties.Count; i++)
            {
                var item = queryProperties[i];

                if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                {
                    builder.Append(string.Format("{0} {1} {2} @{1} ", item.LinkingOperator, item.PropertyName,
                        item.QueryOperator));
                }
                else
                {
                    builder.Append(string.Format("{0} {1} @{0} ", item.PropertyName, item.QueryOperator));
                }

                expando[item.PropertyName] = item.PropertyValue;
            }


            return new QueryResult(builder.ToString().TrimEnd(), expando);

        }


        public virtual QueryResult GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwFiled, Expression<Func<TEntity, bool>> expression)
        {
            string op;
            QueryResult queryResult;

            var filedName = GetPropertyName(btwFiled);

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

        #endregion

        #region Private utility

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns>The property name for the property expression.</returns>
        private static string GetPropertyName(BinaryExpression body)
        {
            string propertyName = body.Left.ToString().Split('.')[1];

            if (body.Left.NodeType == ExpressionType.Convert)
            {
                // remove the trailing ) when convering.
                propertyName = propertyName.Replace(")", string.Empty);
            }

            return propertyName;
        }

        private static string GetPropertyName<TSource, TField>(Expression<Func<TSource, TField>> field)
        {
            if (Equals(field, null))
            {
                throw new NullReferenceException("Field is required");
            }

            MemberExpression expr = null;

            var body = field.Body as MemberExpression;
            if (body != null)
            {
                expr = body;
            }
            else
            {
                var expression = field.Body as UnaryExpression;
                if (expression != null)
                {
                    expr = (MemberExpression)expression.Operand;
                }
                else
                {
                    const string format = "Expression '{0}' not supported.";
                    string message = string.Format(format, field);

                    throw new ArgumentException(message, "field");
                }
            }

            return expr.Member.Name;
        }


        private static object GetValue(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
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
                string propertyName = GetPropertyName(body);
                object propertyValue = GetValue(body.Right);
                string opr = GetOperator(body.NodeType);
                string link = GetOperator(linkingType);

                queryProperties.Add(new QueryParameter(link, propertyName, propertyValue, opr));
            }
            else
            {
                FillQueryProperties(GetBinaryExpression(body.Left), body.NodeType, ref queryProperties);
                FillQueryProperties(GetBinaryExpression(body.Right), body.NodeType, ref queryProperties);
            }
        }

        private static string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    return "AND";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Default:
                    return string.Empty;
                default:
                    throw new NotImplementedException();
            }
        }

        private static BinaryExpression GetBinaryExpression(Expression expression)
        {
            var binaryExpression = expression as BinaryExpression;
            var body = binaryExpression ?? Expression.MakeBinary(ExpressionType.Equal, expression, Expression.Constant(true));
            return body;
        }

        #endregion Private utility
    }
}
