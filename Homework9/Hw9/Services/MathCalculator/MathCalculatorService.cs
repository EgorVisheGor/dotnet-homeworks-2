using Hw9.Dto;

namespace Hw9.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        try
        {
            var postfix = PostfixParser.ConvertToPostfix(expression);
            var expressionTree = ExpressionTree.GenerateExpressionTree(postfix);
            var treeVisited = await new ExpressionTreeVisitor().VisitAsync(expressionTree);
            return new CalculationMathExpressionResultDto(treeVisited);
        }
        catch (Exception e)
        {
            return new CalculationMathExpressionResultDto(e.Message);
        }
    }
}