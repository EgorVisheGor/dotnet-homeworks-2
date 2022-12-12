using System.Linq.Expressions;
using Hw9.ErrorMessages;

namespace Hw9;

public class ExpressionTreeVisitor
{
    public async Task<double> VisitAsync(Expression expression)
    {
        if (expression is BinaryExpression binaryExpression)
        {
            await Task.Delay(1000);
            var task1 = Task.Run(() => VisitAsync(binaryExpression.Left));
            var task2 = Task.Run(() => VisitAsync(binaryExpression.Right));
            var res = await Task.WhenAll(task1, task2);
            await Task.Yield();
            return GetResult(expression, res[0], res[1]);
        }

        var constantExpression = expression as ConstantExpression;
        return (double)constantExpression.Value;}

    public static double GetResult(Expression binaryExpr, double value1, double value2)
    {
        return binaryExpr.NodeType switch
        {
            ExpressionType.Add => value1 + value2,
            ExpressionType.Subtract => value1 - value2,
            ExpressionType.Multiply => value1 * value2,
            ExpressionType.Divide => (value2 < Double.Epsilon)
                ? throw new Exception(MathErrorMessager.DivisionByZero)
                : value1 / value2,
            _ => throw new Exception(MathErrorMessager.UnknownCharacter)
        };
    }
}