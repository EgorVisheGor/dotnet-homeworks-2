using Hw10.ErrorMessages;

namespace Hw10.Expression;

public class ExpressionTree
{
    public static System.Linq.Expressions.Expression GenerateExpressionTree(string postfix)
    {
        var tokens = postfix.Split(' ');
        var nodes = new Stack<System.Linq.Expressions.Expression>();
        foreach (var token in tokens)
        {
            if (double.TryParse(token, out var value))
            {
                nodes.Push(System.Linq.Expressions.Expression.Constant(value, typeof(double)));
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

    private static System.Linq.Expressions.Expression ConvertToOperation(System.Linq.Expressions.Expression a, System.Linq.Expressions.Expression b, string op) => op switch
    {
        "+" => System.Linq.Expressions.Expression.Add(a, b),
        "-" => System.Linq.Expressions.Expression.Subtract(a, b),
        "/" => System.Linq.Expressions.Expression.Divide(a, b),
        "*" => System.Linq.Expressions.Expression.Multiply(a, b),
        _ => throw new Exception(MathErrorMessager.UnknownCharacter)
    };
}