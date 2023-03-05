using Hw10.Dto;
using Hw10.Parser;

namespace Hw10.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        try
        {
            var parseExpression = new ExpressionParser(expression).Parse();

            var converteExpressionDictionary = new ExpressionConverter().ExpressionDictionary(parseExpression);

            var result = await new CalculatorVisitor().VisitDictionary(converteExpressionDictionary);

            return new CalculationMathExpressionResultDto(result);
        }
        catch (Exception e)
        {
            return new CalculationMathExpressionResultDto(e.Message);
        }
    }
}