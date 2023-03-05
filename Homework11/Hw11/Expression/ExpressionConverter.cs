using System.Linq.Expressions;

namespace Hw11;

public class ExpressionConverter
{
    private readonly Dictionary<Expression, Expression[]> _expressionDictionary = new();

    public Dictionary<Expression, Expression[]> ExpressionDictionary(Expression expression)
    {
        Visit(expression as dynamic);
        return _expressionDictionary;
    }

    private void Visit(BinaryExpression binaryExpression)
    {
        _expressionDictionary.Add(binaryExpression, new []{binaryExpression.Left, binaryExpression.Right});
        Visit(binaryExpression.Left as dynamic);
        Visit(binaryExpression.Right as dynamic);
    }

    private void Visit(ConstantExpression constantExpression)
    {
        _expressionDictionary.Add(constantExpression, Array.Empty<Expression>());
    }
}