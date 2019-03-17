namespace MicroOrm.Dapper.Repositories.SqlGenerator.QueryExpressions
{
    /// <summary>
    /// Abstract Query Expression
    /// </summary>
    internal abstract class QueryExpression
    {
        /// <summary>
        /// Query Expression Node Type
        /// </summary>
        public QueryExpressionType NodeType { get; set; }

        /// <summary>
        /// Operator OR/AND
        /// </summary>
        public string LinkingOperator { get; set; }

        public override string ToString()
        {
            return $"[NodeType:{this.NodeType}, LinkingOperator:{LinkingOperator}]";
        }
    }
}