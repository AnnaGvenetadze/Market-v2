using System.Linq.Expressions;
using System.Text;

namespace Market.Extensions;

public partial class ExpressionTranslator<T> : ExpressionVisitor
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

    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.NodeType == ExpressionType.Not)
        {
            if (node.Operand is MemberExpression member && member.Expression is ParameterExpression)
            {
                _sql.Append($"({member.Member.Name} = 0)");
                return node;
            }

            _sql.Append("NOT (");
            Visit(node.Operand);
            _sql.Append(")");
            return node;
        }

        return base.VisitUnary(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        if (IsNullConstant(node.Right) || IsNullConstant(node.Left))
        {
            var memberExpr = IsNullConstant(node.Right) ? node.Left : node.Right;

            _sql.Append("(");
            Visit(memberExpr);

            if (node.NodeType == ExpressionType.Equal)
            {
                _sql.Append(" IS NULL)");
            }
            else if (node.NodeType == ExpressionType.NotEqual)
            {
                _sql.Append(" IS NOT NULL)");
            }

            return node;
        }
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

    private static bool IsNullConstant(Expression expr)
    {
        if (expr is ConstantExpression constExpr)
        {
            return constExpr.Value == null;
        }

        return false;
    }
}