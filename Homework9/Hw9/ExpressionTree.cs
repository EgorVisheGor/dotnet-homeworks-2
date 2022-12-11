using System.Linq.Expressions;
using Hw9.ErrorMessages;

namespace Hw9;

public class ExpressionTree
{
    public static Expression GenerateExpressionTree(string postfix)
    {
        var tokens = postfix.Split(' ');
        var nodes = new Stack<Expression>();
        foreach (var token in tokens)
        {
            if (double.TryParse(token, out var value))
            {
                nodes.Push(Expression.Constant(value, typeof(double)));
            }
            else
            {
                var right = nodes.Pop();
                var left = nodes.Pop();
                var operation = ConvertToOperation(left, right, token);
                nodes.Push(operation);
            }
        }

        return nodes.Pop();
    }

    private static Expression ConvertToOperation(Expression a, Expression b, string op) => op switch
    {
        "+" => Expression.Add(a, b),
        "-" => Expression.Subtract(a, b),
        "/" => Expression.Divide(a, b),
        "*" => Expression.Multiply(a, b),
        _ => throw new Exception(MathErrorMessager.UnknownCharacter)
    };
}