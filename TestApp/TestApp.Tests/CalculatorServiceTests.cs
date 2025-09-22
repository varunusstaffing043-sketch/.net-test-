using TestApp.Services;
using Xunit;

namespace TestApp.Tests
{
    public class CalculatorServiceTests
    {
        [Fact]
        public void Add_ReturnsCorrectSum()
        {
            var service = new CalculatorService();
            var result = service.Add(10, 20);
            Assert.Equal(30, result.Sum);
        }
    }
}
