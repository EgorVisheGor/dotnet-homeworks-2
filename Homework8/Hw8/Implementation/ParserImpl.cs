using System.Globalization;
using Hw8.Calculator;
using Hw8.DTO;
using Hw8.Interfaces;

namespace Hw8.Implementation;

public class ParserImpl : IParser
{
    public string TryParseValues(string? firstValue, string? operation, string? secondValue, out Values result)
    {
        result = new Values();
        if (firstValue == null || operation == null || secondValue == null) return Messages.InvalidArgumentsMessage;

        if (!Double.TryParse(firstValue, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out var val1)
            || !Double.TryParse(secondValue, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo,
                out var val2))
            return Messages.InvalidNumberMessage;
        
        if (!Enum.TryParse<Operation>(operation, out var op) || op == Operation.Invalid)
            return Messages.InvalidOperationMessage;


        if (val2 == 0 && op == Operation.Divide) return Messages.DivisionByZeroMessage;
            
            result = new Values { firstValue = val1, operation = op, secondValue = val2 };
        return "OK";
    }
}