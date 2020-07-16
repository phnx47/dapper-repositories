using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using MicroOrm.Dapper.Repositories.Extensions;

[assembly: InternalsVisibleTo("MicroOrm.Dapper.Repositories.Tests")]

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    internal static class ExpressionHelper
    {
        public static string GetPropertyName<TSource, TField>(Expression<Func<TSource, TField>> field)
        {
            if (Equals(field, null))
                throw new ArgumentNullException(nameof(field), "field can't be null");

            MemberExpression expr;

            switch (field.Body)
            {
                case MemberExpression body:
                    expr = body;
                    break;

                case UnaryExpression expression:
                    expr = (MemberExpression)expression.Operand;
                    break;

                default:
                    throw new ArgumentException("Expression field isn't supported", nameof(field));
            }

            return expr.Member.Name;
        }

        public static object GetValue(Expression member)
        {
            return GetValue(member, out _);
        }

        private static object GetValue(Expression member, out string parameterName)
        {
            parameterName = null;
            if (member == null)
                return null;

            switch (member)
            {
                case MemberExpression memberExpression:
                    var instanceValue = GetValue(memberExpression.Expression, out parameterName);
                    try
                    {
                        switch (memberExpression.Member)
                        {
                            case FieldInfo fieldInfo:
                                parameterName = (parameterName != null ? parameterName + "_" : "") + fieldInfo.Name;
                                return fieldInfo.GetValue(instanceValue);

                            case PropertyInfo propertyInfo:
                                parameterName = (parameterName != null ? parameterName + "_" : "") + propertyInfo.Name;
                                return propertyInfo.GetValue(instanceValue);
                        }
                    }
                    catch
                    {
                        // Try again when we compile the delegate
                    }

                    break;

                case ConstantExpression constantExpression:
                    return constantExpression.Value;

                case MethodCallExpression methodCallExpression:
                    parameterName = methodCallExpression.Method.Name;
                    break;

                case UnaryExpression unaryExpression
                    when (unaryExpression.NodeType == ExpressionType.Convert
                        || unaryExpression.NodeType == ExpressionType.ConvertChecked)
                    && (unaryExpression.Type.UnwrapNullableType() == unaryExpression.Operand.Type):
                    return GetValue(unaryExpression.Operand, out parameterName);
            }

            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        public static string GetSqlOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                case ExpressionType.Not:
                case ExpressionType.MemberAccess:
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
                    throw new NotSupportedException(type + " isn't supported");
            }
        }

        public static string GetSqlLikeValue(string methodName, object value)
        {
            if (value == null)
                value = string.Empty;

            switch (methodName)
            {
                case "CompareString":
                case "Equals":
                    return value.ToString();

                case "StartsWith":
                    return string.Format("{0}%", value);

                case "EndsWith":
                    return string.Format("%{0}", value);

                case "StringContains":
                    return string.Format("%{0}%", value);

                default:
                    throw new NotImplementedException();
            }
        }

        public static string GetMethodCallSqlOperator(string methodName, bool isNotUnary = false)
        {
            switch (methodName)
            {
                case "StartsWith":
                case "EndsWith":
                case "StringContains":
                    return isNotUnary ? "NOT LIKE" : "LIKE";

                case "Contains":
                    return isNotUnary ? "NOT IN" : "IN";

                case "Equals":
                case "CompareString":
                    return isNotUnary ? "!=" : "=";

                case "Any":
                case "All":
                    return methodName.ToUpperInvariant();

                default:
                    throw new NotSupportedException(methodName + " isn't supported");
            }
        }

        public static BinaryExpression GetBinaryExpression(Expression expression)
        {
            var binaryExpression = expression as BinaryExpression;
            var body = binaryExpression ?? Expression.MakeBinary(ExpressionType.Equal, expression,
                expression.NodeType == ExpressionType.Not ? Expression.Constant(false) : Expression.Constant(true));
            return body;
        }

        public static Func<PropertyInfo, bool> GetPrimitivePropertiesPredicate()
        {
            return p => p.CanWrite && (p.PropertyType.IsValueType || p.GetCustomAttributes<ColumnAttribute>().Any() || p.PropertyType == typeof(string) || p.PropertyType == typeof(byte[]));
        }

        public static object GetValuesFromStringMethod(MethodCallExpression callExpr)
        {
            var expr = callExpr.Method.IsStatic ? callExpr.Arguments[1] : callExpr.Arguments[0];

            return GetValue(expr);
        }

        public static object GetValuesFromCollection(MethodCallExpression callExpr)
        {
            var expr = (callExpr.Method.IsStatic ? callExpr.Arguments.First() : callExpr.Object)
                            as MemberExpression;

            try
            {
                return GetValue(expr);
            }
            catch
            {
                throw new NotSupportedException(callExpr.Method.Name + " isn't supported");
            }
        }

        public static MemberExpression GetMemberExpression(Expression expression)
        {
            switch (expression)
            {
                case MethodCallExpression expr:
                    if (expr.Method.IsStatic)
                        return (MemberExpression)expr.Arguments.Last(x => x.NodeType == ExpressionType.MemberAccess);
                    else
                        return (MemberExpression)expr.Arguments[0];

                case MemberExpression memberExpression:
                    return memberExpression;

                case UnaryExpression unaryExpression:
                    return (MemberExpression)unaryExpression.Operand;

                case BinaryExpression binaryExpression:
                    var binaryExpr = binaryExpression;

                    if (binaryExpr.Left is UnaryExpression left)
                        return (MemberExpression)left.Operand;

                    //should we take care if right operation is memberaccess, not left?
                    return (MemberExpression)binaryExpr.Left;

                case LambdaExpression lambdaExpression:

                    switch (lambdaExpression.Body)
                    {
                        case MemberExpression body:
                            return body;

                        case UnaryExpression expressionBody:
                            return (MemberExpression)expressionBody.Operand;
                    }

                    break;
            }

            return null;
        }

        /// <summary>
        ///     Gets the name of the property.
        /// </summary>
        /// <param name="expr">The Expression.</param>
        /// <param name="nested">Out. Is nested property.</param>
        /// <returns>The property name for the property expression.</returns>
        public static string GetPropertyNamePath(Expression expr, out bool nested)
        {
            var path = new StringBuilder();
            var memberExpression = GetMemberExpression(expr);
            var count = 0;
            do
            {
                count++;
                if (path.Length > 0)
                    path.Insert(0, "");

                path.Insert(0, memberExpression.Member.Name);
                memberExpression = GetMemberExpression(memberExpression.Expression);
            } while (memberExpression != null);

            if (count > 2)
                throw new ArgumentException("Only one degree of nesting is supported");

            nested = count == 2;

            return path.ToString();
        }
    }
}
