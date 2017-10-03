using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MicroOrm.Dapper.Repositories.Extensions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    internal static class ExpressionHelper
    {
        public static string GetPropertyName<TSource, TField>(Expression<Func<TSource, TField>> field)
        {
            if (Equals(field, null))
                throw new ArgumentNullException(nameof(field), "field can't be null");

            MemberExpression expr;

            var body = field.Body as MemberExpression;
            if (body != null)
            {
                expr = body;
            }
            else
            {
                var expression = field.Body as UnaryExpression;
                if (expression != null)
                    expr = (MemberExpression)expression.Operand;
                else
                    throw new ArgumentException("Expression field is not supported.", nameof(field));
            }

            return expr.Member.Name;
        }

        public static object GetValue(Expression member)
        {
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
                    throw new NotImplementedException();
            }
        }

        public static string GetMethodCallSqlOperator(string methodName)
        {
            switch (methodName)
            {
                case "Contains":
                    return "IN";

                case "Any":
                case "All":
                    return methodName.ToUpperInvariant();

                default:
                    throw new NotImplementedException();
            }
        }

        public static BinaryExpression GetBinaryExpression(Expression expression)
        {
            var binaryExpression = expression as BinaryExpression;
            var body = binaryExpression ?? Expression.MakeBinary(ExpressionType.Equal, expression, expression.NodeType == ExpressionType.Not ? Expression.Constant(false) : Expression.Constant(true));
            return body;
        }

        public static Func<PropertyInfo, bool> GetPrimitivePropertiesPredicate()
        {
            return p => p.CanWrite && (p.PropertyType.IsValueType() || p.PropertyType == typeof(string) || p.PropertyType == typeof(byte[]));
        }

        public static object GetValuesFromCollection(MethodCallExpression callExpr)
        {
            var expr = callExpr.Object as MemberExpression;

            if (!(expr?.Expression is ConstantExpression))
                throw new NotImplementedException($"{callExpr.Method.Name} is not implemented");

            var constExpr = (ConstantExpression)expr.Expression;

            var constExprType = constExpr.Value.GetType();
            return constExprType.GetField(expr.Member.Name).GetValue(constExpr.Value);
        }


        public static MemberExpression GetMemberExpression(Expression expression)
        {
            var expr = expression as MethodCallExpression;
            if (expr != null)
                return (MemberExpression)expr.Arguments[0];

            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
                return memberExpression;

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
                return (MemberExpression)unaryExpression.Operand;

            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
            {
                var binaryExpr = binaryExpression;

                var left = binaryExpr.Left as UnaryExpression;
                if (left != null)
                    return (MemberExpression)left.Operand;

                //should we take care if right operation is memberaccess, not left ?
                return (MemberExpression)binaryExpr.Left;
            }

            var expression1 = expression as LambdaExpression;
            if (expression1 != null)
            {
                var lambdaExpression = expression1;

                var body = lambdaExpression.Body as MemberExpression;
                if (body != null)
                    return body;

                var expressionBody = lambdaExpression.Body as UnaryExpression;
                if (expressionBody != null)
                    return (MemberExpression)expressionBody.Operand;
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