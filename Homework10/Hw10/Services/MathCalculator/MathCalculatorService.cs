using Hw10.Dto;
using Hw10.Expression;
using Hw10.Parser;

namespace Hw10.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        try
        {
            var expressionTree = ExpressionTree.GenerateExpressionTree(PostfixParser.ConvertToPostfix(expression));
            var visitedTree = await new ExpressionTreeVisitor().VisitAsync(expressionTree);
            return new CalculationMathExpressionResultDto(visitedTree);
        }
        catch (Exception e)
        {
            return new CalculationMathExpressionResultDto(e.Message);
        }
    }
}