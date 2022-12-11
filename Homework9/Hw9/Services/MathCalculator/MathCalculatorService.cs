using Hw9.Dto;

namespace Hw9.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        var calculatorVisitor = new CalcVisitorImpl();
        var expressionConverter = new ExpressionConverter();
        try
        {
            var parser = new RecursiveDescentParser(expression);
            var expressionTree = parser.Parse();
            var expressionMap = expressionConverter.ExpressionDictionary(expressionTree);
            var result = await calculatorVisitor.CalculatorVisitBinary(expressionMap);
            return new CalculationMathExpressionResultDto(result);
        }
        catch (Exception e)
        {
            return new CalculationMathExpressionResultDto(e.Message);
        }
    }
}