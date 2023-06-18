using System.Collections.Generic;

namespace MicroOrm.Dapper.Repositories.SqlGenerator.QueryExpressions;


/// <summary>
/// `Binary` Query Expression
/// </summary>
internal class QueryBinaryExpression : QueryExpression
{
    public QueryBinaryExpression(List<QueryExpression> nodes)
    {
        Nodes = nodes;
        NodeType = QueryExpressionType.Binary;
    }

    public List<QueryExpression> Nodes { get; }

    public override string ToString()
    {
        return $"[{base.ToString()} ({string.Join(",", Nodes)})]";
    }
}
