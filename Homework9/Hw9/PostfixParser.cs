using System.Text;
using System.Text.RegularExpressions;
using Hw9.ErrorMessages;

namespace Hw9;

public class PostfixParser
{
    private static readonly Regex Delimiters = new("(?<=[-+*/()])|(?=[-+*/()])");

    public static string ConvertToPostfix(string expression)
    {
        IsValid(expression);
        var postfix = new StringBuilder();
        var tokens = Delimiters.Split(expression.Replace(" ", "")).Where(c => c != String.Empty).ToArray();

        var stack = new Stack<string>();
        var lastIsBracket = false;
        for (var i = 0; i < tokens.Length; i++)
        {
            var token = tokens[i];
            if (double.TryParse(token, out var val))
            {
                postfix.Append(token + ' ');
            }

            if (token == "(")
            {
                lastIsBracket = true;
                stack.Push(token);
                continue;
            }

            if (token == ")")
            {
                while (stack.Peek() != "(")
                {
                    postfix.Append(stack.Pop() + ' ');
                }

                stack.Pop();
            }

            if (IsOperator(token))
            {
                if (lastIsBracket)
                {
                    stack.Push(token + tokens[++i]);
                }
                else
                {
                    while (stack.Count != 0 && Priority(stack.Peek()) >= Priority(token))
                    {
                        postfix.Append(stack.Pop() + ' ');
                    }

                    stack.Push(token);
                }
            }

            lastIsBracket = false;
        }

        while (stack.Count != 0)
        {
            postfix.Append(stack.Pop() + ' ');
        }

        return postfix.Remove(postfix.Length - 1, 1).ToString();
    }

    private static int Priority(string oper) => oper switch
    {
        "*" => 2,
        "/" => 2,
        "+" => 1,
        "-" => 1,
        _ => 0
    };

    private static bool IsOperator(string c)
    {
        return c == "+" || c == "-" || c == "*" || c == "/";
    }

    private static bool IsValid(string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new Exception(MathErrorMessager.EmptyString);

        if (input.ToCharArray().Count(c => c == '(') != input.ToCharArray().Count(c => c == ')'))
            throw new Exception(MathErrorMessager.IncorrectBracketsNumber);

        //Regex operators = new Regex(@"[\-+*/(). ]", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
        Regex onlyOperators = new Regex(@"[\-+*/]", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        var numbers = new Regex(@"^\d+");
        foreach (var c in input.Where(c =>
                     !numbers.IsMatch(c.ToString()) &&
                     !new[] { "+", "-", "/", "*", "(", ")", " ", "." }.Contains(c.ToString())))
            throw new Exception(MathErrorMessager.UnknownCharacterMessage(c));

        var splittedInput = Delimiters.Split(input.Replace(" ", "")).Where(c => c != String.Empty).ToArray();
        var tempString = splittedInput;
        while (tempString.Length > 1)
        {
            var compareString = tempString.Take(2).ToArray();
            if (onlyOperators.IsMatch(compareString[0]) &&
                onlyOperators.IsMatch(compareString[1]))
                throw new Exception(MathErrorMessager.TwoOperationInRowMessage(compareString[0],
                    compareString[1]));
            if (compareString[0] == "(" && compareString[1] == "-" && tempString.Length - 2 > 1)
            {
                compareString = tempString.Skip(2).Take(2).ToArray();
                if (!numbers.IsMatch(compareString[0]) || compareString[1] != ")")
                    throw new Exception(
                       MathErrorMessager.WrongNotationForANegativeNumber);
            }

            if (compareString[0] == "(" && onlyOperators.IsMatch(compareString[1]))
                throw new Exception(
                    MathErrorMessager.InvalidOperatorAfterParenthesisMessage(compareString[1]));
            if (onlyOperators.IsMatch(compareString[0]) && compareString[1] == ")")
                throw new Exception(MathErrorMessager.OperationBeforeParenthesisMessage(compareString[0]));
            tempString = tempString.Skip(1).ToArray();
        }

        if (onlyOperators.IsMatch(splittedInput[0]))
            throw new Exception(MathErrorMessager.StartingWithOperation);
        if (onlyOperators.IsMatch(splittedInput[splittedInput.Length - 1]))
            throw new Exception(MathErrorMessager.EndingWithOperation);

        var onlyNumbersArray = splittedInput.Where(c => !new[] { "+", "-", "/", "*", "(", ")" }.Contains(c));
        foreach (var c in onlyNumbersArray.Where(c =>
                     !double.TryParse(c.ToString(), out _)))
            throw new Exception(MathErrorMessager.NotNumberMessage(c));

        return true;
    }
}