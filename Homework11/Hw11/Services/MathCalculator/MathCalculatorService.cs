using Hw11.Exceptions;
using Hw11.Parser;

namespace Hw11.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    private readonly IExceptionHandler _handler;

    public MathCalculatorService(IExceptionHandler handler)
    {
        _handler = handler;
    }

    public async Task<double> CalculateMathExpressionAsync(string? expression)
    {
        try
        {
            var parseExpression = new ExpressionParser(expression).Parse(); 
            var converteExpressionDictionary = new ExpressionConverter().ExpressionDictionary(parseExpression);

            var result = await new CalculatorVisitor().VisitDictionary(converteExpressionDictionary);

            return result;
        }
        catch (Exception e)
        {
            _handler.HandleException(e);
            throw;
        }
    }
}