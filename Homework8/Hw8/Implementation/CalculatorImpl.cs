using Hw8.Calculator;

namespace Hw8.Implementation;

public class CalculatorImpl : ICalculator
{
    public double Plus(double val1, double val2) => val1 + val2;

    public double Minus(double val1, double val2) => val1 - val2;

    public double Multiply(double val1, double val2) => val1 * val2;

    public double Divide(double firstValue, double secondValue)
    {
        if (secondValue == 0) throw new InvalidOperationException(Messages.DivisionByZeroMessage);
        else return firstValue / secondValue;
    }

    public double Calculate(double val1, Operation op, double val2)
    {
        return op switch
        {
            Operation.Plus => Plus(val1, val2),
            Operation.Minus => Minus(val1, val2),
            Operation.Multiply => Multiply(val1, val2),
            Operation.Divide => Divide(val1, val2)
        };
    }
}