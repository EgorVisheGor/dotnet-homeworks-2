using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Hw11.Exceptions;
using static Hw11.ErrorMessages.MathErrorMessager;

namespace Hw11.Parser;

public class ExpressionParser
{
    private readonly string[] _operations = {"+", "-", "*", "/"};

    private readonly string[]? _tokens;
    private int _position;

    private static readonly Regex InputSplit = new ("(?<=[−+*/\\(\\)])|(?=[−+*/\\(\\)])");
    private static readonly Regex Numbers = new("[0-9]+");

    public ExpressionParser(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new NullReferenceException(EmptyString);
        _tokens = InputSplit.Split(expression)
                .SelectMany(str => str.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToArray();
    }

    public Expression Parse() 
    {
        var result = PlusOrMinus();
        if (_position == _tokens?.Length) return result;
        var message = _tokens?[_position] == ")" ? IncorrectBracketsNumber : 
            UnknownCharacterMessage(_tokens![_position].ToCharArray()[0]);
        throw new ArgumentException(message);
    }

    private Expression PlusOrMinus() 
    {
        var firstExpression= MultiplyOrDivide();
        while (_position < _tokens?.Length)
        {
            var token = _tokens[_position];
            if (!token.Equals("+") && !token.Equals("-")) break;
            _position++;
            var secondExpression = MultiplyOrDivide();
            firstExpression = token.Equals("+")
                    ? Expression.Add(firstExpression, secondExpression)
                    : Expression.Subtract(firstExpression, secondExpression);
        }

        return firstExpression;
    }


    private Expression MultiplyOrDivide() 
    {
        var firstExpression = Brackets();
        while (_position < _tokens?.Length) 
        {
            var token = _tokens[_position];
            if (!token.Equals("*") && !token.Equals("/")) break;
            _position++;
            var secondExpression = Brackets();
            firstExpression = token.Equals("*")
                    ? Expression.Multiply(firstExpression, secondExpression)
                    : Expression.Divide(firstExpression, secondExpression);
        }

        return firstExpression;
    }

    private Expression Brackets() 
    {
        var next = _position < _tokens?.Length ? _tokens[_position] : string.Empty;
        var previous = _position - 1 >= 0 ? _tokens?[_position - 1] : string.Empty;
        if (next.Equals("(")) {
            _position++;
            var result = PlusOrMinus();
            if (_position >= _tokens?.Length)
                throw new ArgumentException(IncorrectBracketsNumber);

            _position++;
            return result;
        }

        return CheckAllSituations(next, previous);
    }

    private Expression CheckAllSituations(string next, string? previous)
    {
        if (double.TryParse(next, out var res))
        {
            _position++;
            return Expression.Constant(res);
        }
        string message;

        if (_position == 0 && _operations.Contains(next)) 
            message = StartingWithOperation;
        else if (next == ")" && _operations.Contains(previous))
                message = OperationBeforeParenthesisMessage(previous ?? string.Empty);
        else if (previous == "(" && _operations.Contains(next)) 
            message = InvalidOperatorAfterParenthesisMessage(next);
        else if (_operations.Contains(previous) && _operations.Contains(next)) 
            message = TwoOperationInRowMessage(previous ?? string.Empty, next);
        else if (Numbers.IsMatch(next))
            message = NotNumberMessage(next);
        else if (_operations.Contains(previous) && next == string.Empty) 
            message = EndingWithOperation;
        else 
            throw new InvalidSymbolException(UnknownCharacterMessage(next.ToCharArray()[0]));
        throw new InvalidSyntaxException(message);
    }
}