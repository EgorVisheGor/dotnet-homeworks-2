using Hw10.DbModels;
using Hw10.Dto;
using Hw10.Services.MathCalculator;
using Microsoft.EntityFrameworkCore;

namespace Hw10.Services.CachedCalculator;

public class MathCachedCalculatorService : IMathCalculatorService
{
	private readonly ApplicationContext _dbContext;
	private readonly IMathCalculatorService _simpleCalculator;

	public MathCachedCalculatorService(ApplicationContext dbContext, IMathCalculatorService simpleCalculator)
	{
		_dbContext = dbContext;
		_simpleCalculator = simpleCalculator;
	}

	public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
	{
		var result = await _dbContext.SolvingExpressions.FirstOrDefaultAsync(expr => expr.Expression == expression);
		if (result is not null)
		{
			await Task.Delay(1000);
			return new CalculationMathExpressionResultDto(result.Result);   
		}



		var calculation = await _simpleCalculator.CalculateMathExpressionAsync(expression);
		if (calculation.IsSuccess)
		{
			_dbContext.Add(new SolvingExpression(){ Expression = expression!, Result = calculation.Result });
			await _dbContext.SaveChangesAsync();
		}

		return calculation;
	}
}