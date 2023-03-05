using System.Data;
using System.Linq.Expressions;
using Hw11.ErrorMessages;

namespace Hw11;

public class CalculatorVisitor
{
    private Dictionary<Expression, Lazy<Task<double>>> _dictionary = new ();
    public async Task<double> VisitDictionary(Dictionary<Expression, Expression[]> dictionary)
    {
        var first = dictionary.Keys.First();
        foreach (var (current,before) in dictionary)
        {
            _dictionary[current] = new Lazy<Task<double>>(async () =>
            {
                await Task.WhenAll(before.Select(x => _dictionary[x].Value));
                await Task.Yield();

                return await CalculateExpression(current as dynamic);
            });
        }

        var result = await _dictionary[first].Value;
        _dictionary.Clear();

        return result;
    }

    private static double Add(double arg1, double arg2)
        => arg1 + arg2;

    private static double Subtract(double arg1, double arg2)
        => arg1 - arg2;

    private static double Multiply(double arg1, double arg2)
        => arg1 * arg2;

    private static double Divide(double arg1, double arg2)
        => arg2 == 0.0 ? throw new DivideByZeroException(MathErrorMessager.DivisionByZero): arg1 / arg2;

    private async Task<double> CalculateExpression(Expression expression)
    {
        if (expression is ConstantExpression constantExpression)
            return await Task.FromResult((double) constantExpression.Value!);

        await Task.Delay(1000);

        var binaryExpr = expression as BinaryExpression;
        var arg1 = await _dictionary[binaryExpr.Left].Value;var arg2 = await _dictionary[binaryExpr.Right].Value;

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