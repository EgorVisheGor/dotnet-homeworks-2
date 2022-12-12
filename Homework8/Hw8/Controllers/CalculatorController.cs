using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Hw8.Calculator;
using Hw8.Implementation;
using Hw8.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static Hw8.Calculator.Messages;

namespace Hw8.Controllers;

public class CalculatorController : Controller
{

    private IParser _parser;

    public CalculatorController(IParser parser)
    {
        _parser = parser;
    }

    public ActionResult<double> Calculate([FromServices] ICalculator calculator,
        [FromQuery]string val1,
        [FromQuery]string operation,
        [FromQuery]string val2)
    {
        var message = _parser.TryParseValues(val1, operation, val2, out var result);

        return message switch
        {
            "OK" => calculator.Calculate(result.firstValue, result.operation, result.secondValue),
            InvalidNumberMessage => BadRequest(InvalidNumberMessage),
            InvalidOperationMessage => BadRequest(InvalidOperationMessage),
            DivisionByZeroMessage => BadRequest(DivisionByZeroMessage),
           _ => BadRequest(InvalidArgumentsMessage)
        };
    }
    
    [ExcludeFromCodeCoverage]
    public IActionResult Index()
    {
        return Content(
            "Заполните val1, operation(plus, minus, multiply, divide) и val2 здесь '/calculator/calculate?val1= &operation= &val2= '\n" +
            "и добавьте её в адресную строку.");
    }
}