using System.Linq.Expressions;
using System.Text;

namespace Market.Extensions;

public class ExpressionTranslator<T> : ExpressionVisitor
{
    private readonly StringBuilder _sql = new();
    private readonly Dictionary<string, object> _parameters = new();
    private int _paramCount = 0;

    public (string Sql, Dictionary<string, object> Parameters) Translate(Expression<Func<T, bool>> expression)
    {
        _sql.Clear();
        _parameters.Clear();
        Visit(expression.Body);
        return (_sql.ToString(), _parameters);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _sql.Append("(");

        Visit(node.Left);

        _sql.Append(node.NodeType switch
        {
            ExpressionType.Equal => " = ",
            ExpressionType.NotEqual => " <> ",
            ExpressionType.GreaterThan => " > ",
            ExpressionType.GreaterThanOrEqual => " >= ",
            ExpressionType.LessThan => " < ",
            ExpressionType.LessThanOrEqual => " <= ",
            ExpressionType.AndAlso => " AND ",
            ExpressionType.OrElse => " OR ",
            _ => throw new NotSupportedException($"Operator not supported: {node.NodeType}")
        });

        Visit(node.Right);

        _sql.Append(")");

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is ParameterExpression)
        {
            _sql.Append(node.Member.Name);
            return node;
        }
        var value = Expression.Lambda(node).Compile().DynamicInvoke();
        AddParameter(value);
        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        AddParameter(node.Value);
        return node;
    }

    private void AddParameter(object? value)
    {
        var paramName = $"@p{_paramCount++}";
        _parameters[paramName] = value ?? DBNull.Value;
        _sql.Append(paramName);
    }
}