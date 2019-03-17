namespace MicroOrm.Dapper.Repositories.SqlGenerator.QueryExpressions
{
    /// <inheritdoc />
    /// <summary>
    /// Class that models the data structure in coverting the expression tree into SQL and Params.
    /// `Parameter` Query Expression
    /// </summary>
    internal class QueryParameterExpression : QueryExpression
    {
        public QueryParameterExpression()
        {
            NodeType = QueryExpressionType.Parameter;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryParameterExpression " /> class.
        /// </summary>
        /// <param name="linkingOperator">The linking operator.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="queryOperator">The query operator.</param>
        /// <param name="nestedProperty">Signilize if it is nested property.</param>
        internal QueryParameterExpression(string linkingOperator,
            string propertyName, object propertyValue,
            string queryOperator, bool nestedProperty) : this()
        {
            LinkingOperator = linkingOperator;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            QueryOperator = queryOperator;
            NestedProperty = nestedProperty;
        }

        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
        public string QueryOperator { get; set; }
        public bool NestedProperty { get; set; }

        public override string ToString()
        {
            return
                $"[{base.ToString()}, PropertyName:{PropertyName}, PropertyValue:{PropertyValue}, QueryOperator:{QueryOperator}, NestedProperty:{NestedProperty}]";
        }
    }
}
