using System;
using System.Collections.Generic;
using System.Linq;
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
                throw new NullReferenceException("Field is required");

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
                    expr = (MemberExpression) expression.Operand;
                else
                    throw new ArgumentException("Expression" + field + " is not supported.", nameof(field));
            }

            return expr.Member.Name;
        }

        public static object GetValue(Expression member)
        {
            UnaryExpression objectMember = Expression.Convert(member, typeof(object));
            Expression<Func<object>> getterLambda = Expression.Lambda<Func<object>>(objectMember);
            Func<object> getter = getterLambda.Compile();
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

        public static string GetSqlOperator(string methodName)
        {
            switch (methodName)
            {
                case "Contains":
                    return "IN";

                case "Any":
                    return "ANY";

                case "All":
                    return "ALL";

                default:
                    throw new NotImplementedException();
            }
        }

        public static BinaryExpression GetBinaryExpression(Expression expression)
        {
            var binaryExpression = expression as BinaryExpression;
            BinaryExpression body = binaryExpression ?? Expression.MakeBinary(ExpressionType.Equal, expression, expression.NodeType == ExpressionType.Not ? Expression.Constant(false) : Expression.Constant(true));
            return body;
        }

        public static Func<PropertyInfo, bool> GetPrimitivePropertiesPredicate()
        {
            return p => p.CanWrite && (p.PropertyType.IsValueType() || p.PropertyType == typeof(string) || p.PropertyType == typeof(byte[]));
        }

        public static object GetValuesFromCollection(MethodCallExpression callExpr)
        {
            MemberExpression expr = callExpr.Object as MemberExpression;

            if (expr != null && expr.Expression is ConstantExpression)
            {
                ConstantExpression constExpr = (ConstantExpression) expr.Expression;

                Type constExprType = constExpr.Value.GetType();
                return constExprType.GetField(expr.Member.Name).GetValue(constExpr.Value);
            }

            throw new NotImplementedException($"{callExpr.Method.Name} is not implemented");
        }


        public static MemberExpression GetMemberExpression(Expression expression)
        {
            if (expression is MethodCallExpression)
            {
                var methodExpr = (MethodCallExpression) expression;
                return (MemberExpression) methodExpr.Arguments[0];
            }
               
            if (expression is MemberExpression)
                return (MemberExpression) expression;
            if (expression is UnaryExpression)
                return (MemberExpression)((UnaryExpression)expression).Operand;
            if (expression is BinaryExpression)
            {
                var binaryExpr = (BinaryExpression) expression;
                if (binaryExpr.Left is UnaryExpression)
                    return (MemberExpression)((UnaryExpression)binaryExpr.Left).Operand;

                //should we take care if right operation is memberaccess, not left ?
                return (MemberExpression)binaryExpr.Left;
            }
            if (expression is LambdaExpression)
            {
                var lambdaExpression = expression as LambdaExpression;
                if (lambdaExpression.Body is MemberExpression)
                    return (MemberExpression) lambdaExpression.Body;
                if (lambdaExpression.Body is UnaryExpression)
                    return (MemberExpression) ((UnaryExpression) lambdaExpression.Body).Operand;
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
            MemberExpression memberExpression = GetMemberExpression(expr);
            int count = 0;  
            do
            {
                count++;
                if (path.Length > 0)
                    path.Insert(0, "");
                path.Insert(0, memberExpression.Member.Name);
                memberExpression = GetMemberExpression(memberExpression.Expression);
            } while (memberExpression != null);

            if(count > 2)
                throw new ArgumentException("Only one degree of nesting is supported");

            nested = count == 2;

            return path.ToString();
        }
    }
}