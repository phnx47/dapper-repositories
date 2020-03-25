using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MicroOrm.Dapper.Repositories.SqlGenerator.QueryExpressions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity>
        where TEntity : class
    {
        /// <summary>
        ///     Get query properties
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <param name="linkingType">Type of the linking.</param>
        private QueryExpression GetQueryProperties(Expression expr, ExpressionType linkingType)
        {
            var isNotUnary = false;
            if (expr is UnaryExpression unaryExpression)
            {
                if (unaryExpression.NodeType == ExpressionType.Not && unaryExpression.Operand is MethodCallExpression)
                {
                    expr = unaryExpression.Operand;
                    isNotUnary = true;
                }
            }

            if (expr is MethodCallExpression methodCallExpression)
            {
                var methodName = methodCallExpression.Method.Name;
                var exprObj = methodCallExpression.Object;
                MethodLabel:
                switch (methodName)
                {
                    case "Contains":
                    {
                        if (exprObj != null
                            && exprObj.NodeType == ExpressionType.MemberAccess
                            && exprObj.Type == typeof(string))
                        {
                            methodName = "StringContains";
                            goto MethodLabel;
                        }

                        var propertyName = ExpressionHelper.GetPropertyNamePath(methodCallExpression, out var isNested);

                        if (!SqlProperties.Select(x => x.PropertyName).Contains(propertyName) &&
                            !SqlJoinProperties.Select(x => x.PropertyName).Contains(propertyName))
                            throw new NotSupportedException("predicate can't parse");

                        var propertyValue = ExpressionHelper.GetValuesFromCollection(methodCallExpression);
                        var opr = ExpressionHelper.GetMethodCallSqlOperator(methodName, isNotUnary);
                        var link = ExpressionHelper.GetSqlOperator(linkingType);
                        return new QueryParameterExpression(link, propertyName, propertyValue, opr, isNested);
                    }
                    case "StringContains":
                    case "CompareString":
                    case "Equals":
                    case "StartsWith":
                    case "EndsWith":
                    {
                        if (exprObj == null
                            || exprObj.NodeType != ExpressionType.MemberAccess)
                        {
                            goto default;
                        }

                        var propertyName = ExpressionHelper.GetPropertyNamePath(exprObj, out bool isNested);

                        if (!SqlProperties.Select(x => x.PropertyName).Contains(propertyName) &&
                            !SqlJoinProperties.Select(x => x.PropertyName).Contains(propertyName))
                            throw new NotSupportedException("predicate can't parse");

                        var propertyValue = ExpressionHelper.GetValuesFromStringMethod(methodCallExpression);
                        var likeValue = ExpressionHelper.GetSqlLikeValue(methodName, propertyValue);
                        var opr = ExpressionHelper.GetMethodCallSqlOperator(methodName, isNotUnary);
                        var link = ExpressionHelper.GetSqlOperator(linkingType);
                        return new QueryParameterExpression(link, propertyName, likeValue, opr, isNested);
                    }
                    default:
                        throw new NotSupportedException($"'{methodName}' method is not supported");
                }
            }

            if (expr is BinaryExpression binaryExpression)
            {
                if (binaryExpression.NodeType != ExpressionType.AndAlso && binaryExpression.NodeType != ExpressionType.OrElse)
                {
                    var propertyName = ExpressionHelper.GetPropertyNamePath(binaryExpression, out var isNested);
                    bool checkNullable = isNested && propertyName.EndsWith("HasValue");

                    if (!checkNullable)
                    {
                        if (!SqlProperties.Select(x => x.PropertyName).Contains(propertyName) &&
                            !SqlJoinProperties.Select(x => x.PropertyName).Contains(propertyName))
                            throw new NotSupportedException("predicate can't parse");
                    }
                    else
                    {
                        var prop = SqlProperties.FirstOrDefault(x => x.IsNullable && x.PropertyName + "HasValue" == propertyName);
                        if (prop == null)
                        {
                            prop = SqlJoinProperties.FirstOrDefault(x => x.IsNullable && x.PropertyName + "HasValue" == propertyName);
                            if (prop == null)
                                throw new NotSupportedException("predicate can't parse");
                        }
                        else
                        {
                            isNested = false;
                        }

                        propertyName = prop.PropertyName;
                    }

                    var propertyValue = ExpressionHelper.GetValue(binaryExpression.Right);
                    var nodeType = checkNullable ? ((bool) propertyValue == false ? ExpressionType.Equal : ExpressionType.NotEqual) : binaryExpression.NodeType;
                    if (checkNullable)
                    {
                        propertyValue = null;
                    }
                    var opr = ExpressionHelper.GetSqlOperator(nodeType);
                    var link = ExpressionHelper.GetSqlOperator(linkingType);

                    return new QueryParameterExpression(link, propertyName, propertyValue, opr, isNested);
                }

                var leftExpr = GetQueryProperties(binaryExpression.Left, ExpressionType.Default);
                var rightExpr = GetQueryProperties(binaryExpression.Right, binaryExpression.NodeType);

                switch (leftExpr)
                {
                    case QueryParameterExpression lQPExpr:
                        if (!string.IsNullOrEmpty(lQPExpr.LinkingOperator) && !string.IsNullOrEmpty(rightExpr.LinkingOperator)) // AND a AND B
                        {
                            switch (rightExpr)
                            {
                                case QueryBinaryExpression rQBExpr:
                                    if (lQPExpr.LinkingOperator == rQBExpr.Nodes.Last().LinkingOperator) // AND a AND (c AND d)
                                    {
                                        var nodes = new QueryBinaryExpression
                                        {
                                            LinkingOperator = leftExpr.LinkingOperator,
                                            Nodes = new List<QueryExpression> {leftExpr}
                                        };

                                        rQBExpr.Nodes[0].LinkingOperator = rQBExpr.LinkingOperator;
                                        nodes.Nodes.AddRange(rQBExpr.Nodes);

                                        leftExpr = nodes;
                                        rightExpr = null;
                                        // AND a AND (c AND d) => (AND a AND c AND d)
                                    }

                                    break;
                            }
                        }

                        break;

                    case QueryBinaryExpression lQBExpr:
                        switch (rightExpr)
                        {
                            case QueryParameterExpression rQPExpr:
                                if (rQPExpr.LinkingOperator == lQBExpr.Nodes.Last().LinkingOperator) //(a AND b) AND c
                                {
                                    lQBExpr.Nodes.Add(rQPExpr);
                                    rightExpr = null;
                                    //(a AND b) AND c => (a AND b AND c)
                                }

                                break;

                            case QueryBinaryExpression rQBExpr:
                                if (lQBExpr.Nodes.Last().LinkingOperator == rQBExpr.LinkingOperator) // (a AND b) AND (c AND d)
                                {
                                    if (rQBExpr.LinkingOperator == rQBExpr.Nodes.Last().LinkingOperator) // AND (c AND d)
                                    {
                                        rQBExpr.Nodes[0].LinkingOperator = rQBExpr.LinkingOperator;
                                        lQBExpr.Nodes.AddRange(rQBExpr.Nodes);
                                        // (a AND b) AND (c AND d) =>  (a AND b AND c AND d)
                                    }
                                    else
                                    {
                                        lQBExpr.Nodes.Add(rQBExpr);
                                        // (a AND b) AND (c OR d) =>  (a AND b AND (c OR d))
                                    }

                                    rightExpr = null;
                                }

                                break;
                        }

                        break;
                }

                var nLinkingOperator = ExpressionHelper.GetSqlOperator(linkingType);
                if (rightExpr == null)
                {
                    leftExpr.LinkingOperator = nLinkingOperator;
                    return leftExpr;
                }

                return new QueryBinaryExpression
                {
                    NodeType = QueryExpressionType.Binary,
                    LinkingOperator = nLinkingOperator,
                    Nodes = new List<QueryExpression> {leftExpr, rightExpr},
                };
            }

            return GetQueryProperties(ExpressionHelper.GetBinaryExpression(expr), linkingType);
        }
    }
}
