using Hw10.DbModels;
using Hw10.Dto;
using Hw10.Services.MathCalculator;

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
		var solvingExpression = _dbContext.SolvingExpressions
			.FirstOrDefault(exp => exp.Expression == expression);

		if (solvingExpression is null)
		{
			var resultDto = await _simpleCalculator.CalculateMathExpressionAsync(expression);

			if (!resultDto.IsSuccess) 
				return resultDto;

			await _dbContext.SolvingExpressions.AddAsync(new SolvingExpression(expression!, resultDto.Result));

			await _dbContext.SaveChangesAsync();

			return resultDto;
		}

		return new CalculationMathExpressionResultDto(solvingExpression.Result);	
	}
}