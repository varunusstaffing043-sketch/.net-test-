using TestApp.Models;

namespace TestApp.Services
{
    public class CalculatorService
    {
        public CalculationResult Add(int a, int b)
        {
            return new CalculationResult
            {
                Sum = a + b
            };
        }
    }
}
