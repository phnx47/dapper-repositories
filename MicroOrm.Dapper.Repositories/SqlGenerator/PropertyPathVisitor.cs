using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{

    /*
     * 
     *  taken from : 
     *      http://www.thomaslevesque.com/2010/10/03/entity-framework-using-include-with-lambda-expressions/
     * 
     * */

    /// <summary>
    /// 
    /// </summary>
    public class PropertyPathVisitor : ExpressionVisitor
    {
        private Stack<MemberInfo> _stack;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public MemberInfo GetPropertyPath(Expression expression)
        {
            _stack = new Stack<MemberInfo>();
            Visit(expression);
            return _stack.LastOrDefault();
        }

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression expression)
        {
            _stack?.Push(expression.Member);
            return base.VisitMember(expression);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (IsLinqOperator(expression.Method))
            {
                for (var i = expression.Arguments.Count-1; i >= 0; i--)
                {
                    Visit(expression.Arguments[i]);
                }
                return expression;
            }
            return base.VisitMethodCall(expression);
        }

        private static bool IsLinqOperator(MethodInfo method)
        {
            if (method.DeclaringType != typeof(Enumerable))
                return false;

            return method.GetCustomAttribute<ExtensionAttribute>() != null;
        }
    }
}