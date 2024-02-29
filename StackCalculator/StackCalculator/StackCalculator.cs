namespace StackCalculator;

/// <summary>
/// Class of calculating postfix expressionn.
/// </summary>
public class Calculator
{
    /// <summary>
    /// Stack that contains numbers that we will need for operations.
    /// </summary>
    private readonly IStack stack;

    /// <summary>
    /// Initializes a new instance of the <see cref="Calculator"/> class.
    /// </summary>
    /// <param name="stack">Stack for counting.</param>
    /// <exception cref="ArgumentNullException">Stack can't be null.</exception>
    public Calculator(IStack stack)
    {
        this.stack = stack ?? throw new ArgumentNullException(nameof(stack), "Can't be null");
    }

    /// <summary>
    /// Calculate expression in postfix format.
    /// </summary>
    /// <param name="expression">String with math expression in postfix format.</param>
    /// <returns>(0, false) if expression contains division by zero else (result of calculating, true).</returns>
    /// <exception cref="ArgumentNullException">Expression can't be null.</exception>
    /// <exception cref="ArgumentException">Not valid expression.</exception>
    public (double, bool) CalculatePostfixExpression(string expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression), "Can't be null");
        }

        if (expression == string.Empty)
        {
            throw new ArgumentException(nameof(expression), "Can't be empty");
        }

        var expressionElements = expression.Split();

        foreach (var element in expressionElements)
        {
            if (double.TryParse(element, out double result))
            {
                this.stack.Push(result);
            }
            else
            {
                if (!this.IsOperation(element[0]))
                {
                    throw new ArgumentException(nameof(expression), "Can't contains anything except numbers and operations");
                }

                double firstNumber;
                double secondNumber;

                try
                {
                    secondNumber = this.stack.Pop();
                    firstNumber = this.stack.Pop();
                }
                catch
                {
                    throw new ArgumentException("Expression is invalid", nameof(expression));
                }

                var (operationResult, success) = this.CalculateOperation(firstNumber, secondNumber, element[0]);

                if (!success)
                {
                    return (0D, false);
                }

                this.stack.Push(operationResult);
            }
        }

        double answer;
        try
        {
            answer = this.stack.Pop();
        }
        catch
        {
            throw new ArgumentException("Expression is invalid", nameof(expression));
        }

        if (!this.stack.IsEmpty())
        {
            throw new ArgumentException("Expression is invalid", nameof(expression));
        }

        return (answer, true);
    }

    /// <summary>
    /// Check is symbol operation or not.
    /// </summary>
    /// <param name="symbol">Symbol that we checks.</param>
    /// <returns>true if symbol is operation else false.</returns>
    private bool IsOperation(char symbol)
        => symbol == '+' || symbol == '-' || symbol == '*' || symbol == '/';

    /// <summary>
    /// Calculate expression with 1 operation.
    /// </summary>
    /// <param name="firstNumber">first number in expression.</param>
    /// <param name="secondNumber">second number in expression.</param>
    /// <param name="operation">Arithmetic operation.</param>
    /// <returns>(0, false) if it is division by zero else (result, true).</returns>
    /// <exception cref="ArgumentException">operation should be real math operation.</exception>
    private (double, bool) CalculateOperation(double firstNumber, double secondNumber, char operation)
    {
        switch (operation)
        {
            case '+':
                return (firstNumber + secondNumber, true);

            case '-':
                return (firstNumber - secondNumber, true);

            case '*':
                return (firstNumber * secondNumber, true);

            case '/':
            {
                return Math.Abs(secondNumber) < 0.0000000000001D ? (0D, false) : (firstNumber / secondNumber, true);
            }

            default:
                throw new ArgumentException("Not operation sign", nameof(operation));
        }
    }
}