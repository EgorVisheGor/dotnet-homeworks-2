using System.Data;
using System.Linq.Expressions;
using Hw10.ErrorMessages;

namespace Hw10;

public class CalculatorVisitor
{
    public async Task<double> VisitDictionary(Dictionary<Expression, Expression[]> dictionary)
    {
        var first = dictionary.Keys.First();
        var lazy = new Dictionary<Expression, Lazy<Task<double>>>();
        foreach (var (current,before) in dictionary)
        {
            lazy[current] = new Lazy<Task<double>>(async () =>
            {
                await Task.WhenAll(before.Select(x => lazy[x].Value));
                await Task.Yield();

                if (current is BinaryExpression binaryExpression)
                    return await CalculateExpression(current, await lazy[binaryExpression.Left].Value,
                        await lazy[binaryExpression.Right].Value);
                return await CalculateExpression(current, 0.0, 0.0);
            });
        }

        return await lazy[first].Value;
    }

    private static double Add(double arg1, double arg2)
        => arg1 + arg2;

    private static double Subtract(double arg1, double arg2)
        => arg1 - arg2;

    private static double Multiply(double arg1, double arg2)
        => arg1 * arg2;

    private static double Divide(double arg1, double arg2)
        => arg2 == 0.0 ? throw new DivideByZeroException(MathErrorMessager.DivisionByZero): arg1 / arg2;

    private static async Task<double> CalculateExpression(Expression expression, double arg1, double arg2)
    {
        if (expression is ConstantExpression constantExpression)
            return await Task.FromResult((double) constantExpression.Value!);

        await Task.Delay(1000);

        return expression.NodeType switch
        {
            ExpressionType.Add => Add(arg1, arg2),
            ExpressionType.Subtract => Subtract(arg1, arg2),
            ExpressionType.Multiply => Multiply(arg1, arg2),
            ExpressionType.Divide => Divide(arg1, arg2),
            _ => throw new InvalidExpressionException()
        };
    }
}