namespace MicroOrm.Dapper.Repositories.SqlGenerator.QueryExpressions;


/// <summary>
/// Class that models the data structure in coverting the expression tree into SQL and Params.
/// `Parameter` Query Expression
/// </summary>
internal class QueryParameterExpression : QueryExpression
{
    private QueryParameterExpression()
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
        string propertyName, object? propertyValue,
        string queryOperator, bool nestedProperty) : this()
    {
        LinkingOperator = linkingOperator;
        PropertyName = propertyName;
        PropertyValue = propertyValue;
        QueryOperator = queryOperator;
        NestedProperty = nestedProperty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterExpression " /> class for a column-to-column comparison.
    /// </summary>
    /// <param name="linkingOperator">The linking operator.</param>
    /// <param name="propertyName">Name of the left property.</param>
    /// <param name="queryOperator">The query operator.</param>
    /// <param name="nestedProperty">Signilize if the left property is nested.</param>
    /// <param name="propertyValueColumn">Name of the right property being compared against.</param>
    /// <param name="propertyValueColumnNested">Signilize if the right property is nested.</param>
    internal QueryParameterExpression(string linkingOperator,
        string propertyName, string queryOperator, bool nestedProperty,
        string propertyValueColumn, bool propertyValueColumnNested) : this()
    {
        LinkingOperator = linkingOperator;
        PropertyName = propertyName;
        QueryOperator = queryOperator;
        NestedProperty = nestedProperty;
        PropertyValueColumn = propertyValueColumn;
        PropertyValueColumnNested = propertyValueColumnNested;
    }

    public string? PropertyName { get; set; }
    public object? PropertyValue { get; set; }
    public string? QueryOperator { get; set; }
    public bool NestedProperty { get; set; }

    /// <summary>
    /// Name of the right-hand property when comparing a column against another column.
    /// </summary>
    public string? PropertyValueColumn { get; set; }

    /// <summary>
    /// Signilize if the right-hand column is a nested (join) property.
    /// </summary>
    public bool PropertyValueColumnNested { get; set; }

    /// <summary>
    /// <c>true</c> when this expression compares two columns instead of a column and a value.
    /// </summary>
    public bool IsColumnComparison => PropertyValueColumn != null;

    public override string ToString()
    {
        return
            $"[{base.ToString()}, PropertyName:{PropertyName}, PropertyValue:{PropertyValue}, QueryOperator:{QueryOperator}, NestedProperty:{NestedProperty}, PropertyValueColumn:{PropertyValueColumn}, PropertyValueColumnNested:{PropertyValueColumnNested}]";
    }
}
