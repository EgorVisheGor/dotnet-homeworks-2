using System.Linq.Expressions;

namespace Hw9;

public class ExpressionConverter
{
    private readonly Dictionary<Expression, Expression[]> _expressionDictionary = new();

    public Dictionary<Expression, Expression[]> ExpressionDictionary(Expression expression)
    {
        Visit(expression);
        return _expressionDictionary;
    }

    private void Visit(Expression expression)
    {
        if(!_expressionDictionary.ContainsKey(expression))
            switch (expression)
            {
                case BinaryExpression binaryExpression:
                    BinaryVisit(binaryExpression);
                    break;
                case ConstantExpression constantExpression:
                    ConstantVisit(constantExpression);
                    break;
            }
    }

    private void BinaryVisit(BinaryExpression binaryExpression)
    {
        _expressionDictionary.Add(binaryExpression, new []{binaryExpression.Left, binaryExpression.Right});
        Visit(binaryExpression.Left);
        Visit(binaryExpression.Right);
    }

    private void ConstantVisit(ConstantExpression constantExpression)
    {
        _expressionDictionary.Add(constantExpression, Array.Empty<Expression>());
    }
}