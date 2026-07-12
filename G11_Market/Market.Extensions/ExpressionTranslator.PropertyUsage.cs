using System.Linq.Expressions;

namespace Market.Extensions;

public partial class ExpressionTranslator<T>
{
    public bool ContainsProperty(
        Expression<Func<T, bool>> expression,
        string propertyName)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        PropertyUsageVisitor visitor = new(propertyName);
        visitor.Visit(expression.Body);

        return visitor.IsUsed;
    }

    private sealed class PropertyUsageVisitor : ExpressionVisitor
    {
        private readonly string _propertyName;

        public bool IsUsed { get; private set; }

        public PropertyUsageVisitor(string propertyName) 
            => _propertyName = propertyName;

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is ParameterExpression &&
                node.Member.Name == _propertyName)
            {
                IsUsed = true;
            }

            return base.VisitMember(node);
        }
    }
}