using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Hw6;
using Xunit;

namespace Hw6Tests
{
    public class BasicTests : IClassFixture<CustomWebApplicationFactory<App.Startup>>
    {
        private readonly CustomWebApplicationFactory<App.Startup> _factory;
        private const decimal Epsilon = 0.001m;

        public BasicTests(CustomWebApplicationFactory<App.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(15, 5, "Plus", 20, HttpStatusCode.OK)]
        [InlineData(15, 5, "Minus", 10, HttpStatusCode.OK)]
        [InlineData(15, 5, "Multiply", 75, HttpStatusCode.OK)]
        [InlineData(15, 5, "Divide", 3, HttpStatusCode.OK)]
        public async Task TestAllOperationsInt(int value1, int value2, string operation, int expectedValue,
            HttpStatusCode statusCode) =>
            await RunTest(value1.ToString(), value2.ToString(), operation, expectedValue.ToString(), statusCode);

        [Theory]
        [InlineData(15.6, 5.6, "Plus", 21.2, HttpStatusCode.OK)]
        [InlineData(15.6, 5.6, "Minus", 10, HttpStatusCode.OK)]
        [InlineData(15.6, 5.6, "Multiply", 87.36, HttpStatusCode.OK)]
        [InlineData(15.6, 5.6, "Divide", 2.7857, HttpStatusCode.OK)]
        public async Task TestAllOperationsDouble(double value1, double value2, string operation,
            double expectedValue, HttpStatusCode statusCode) =>
            await RunTest(value1.ToString(CultureInfo.InvariantCulture), value2.ToString(CultureInfo.InvariantCulture),
                operation, expectedValue.ToString(CultureInfo.InvariantCulture), statusCode);

        [Theory]
        [InlineData(15.6, 5.6, "Plus", 21.2, HttpStatusCode.OK)]
        [InlineData(15.6, 5.6, "Minus", 10, HttpStatusCode.OK)]
        [InlineData(15.6, 5.6, "Multiply", 87.36, HttpStatusCode.OK)]
        [InlineData(15.6, 5.6, "Divide", 2.7857, HttpStatusCode.OK)]
        public async Task TestAllOperationsDecimal(decimal value1, decimal value2, string operation,
            decimal expectedValue, HttpStatusCode statusCode) =>
            await RunTest(value1.ToString(CultureInfo.InvariantCulture), value2.ToString(CultureInfo.InvariantCulture),
                operation, expectedValue.ToString(CultureInfo.InvariantCulture), statusCode);

        [Theory]
        [InlineData("biba", "5.6", "Plus", "Could not parse value 'biba'", HttpStatusCode.BadRequest)]
        [InlineData("15.6", "boba", "Plus", "Could not parse value 'boba'", HttpStatusCode.BadRequest)]
        [InlineData("biba", "boba", "Plus", "Could not parse value 'biba'", HttpStatusCode.BadRequest)]
        [InlineData("biba", "5.6", "Minus", "Could not parse value 'biba'", HttpStatusCode.BadRequest)]
        [InlineData("15.6", "boba", "Minus", "Could not parse value 'boba'", HttpStatusCode.BadRequest)]
        [InlineData("biba", "boba", "Minus", "Could not parse value 'biba'", HttpStatusCode.BadRequest)]
        [InlineData("biba", "5.6", "Multiply", "Could not parse value 'biba'", HttpStatusCode.BadRequest)]
        [InlineData("15.6", "boba", "Multiply", "Could not parse value 'boba'", HttpStatusCode.BadRequest)]
        [InlineData("biba", "boba", "Multiply", "Could not parse value 'biba'", HttpStatusCode.BadRequest)]
        [InlineData("biba", "5.6", "Divide", "Could not parse value 'biba'", HttpStatusCode.BadRequest)]
        [InlineData("15.6", "boba", "Divide", "Could not parse value 'boba'", HttpStatusCode.BadRequest)]
        [InlineData("biba", "boba", "Divide", "Could not parse value 'biba'", HttpStatusCode.BadRequest)]
        [InlineData("15.6.6", "5.6", "Divide", "Could not parse value '15.6.6'", HttpStatusCode.BadRequest)]
        public async Task TestAllOperationsIncorrectValues(string value1, string value2, string operation,
            string expectedValue, HttpStatusCode statusCode) =>
            await RunTest(value1, value2, operation, expectedValue, statusCode);
        
        [Theory]
        [InlineData("15.6", "5.6", "@", "Could not parse value '@'", HttpStatusCode.BadRequest)]
        [InlineData("15.6", "5.6", "$", "Could not parse value '$'", HttpStatusCode.BadRequest)]
        [InlineData("15.6", "5.6", "^", "Could not parse value '^'", HttpStatusCode.BadRequest)]
        public async Task TestIncorrectOperation(string value1, string value2, string operation,
            string expectedValue, HttpStatusCode statusCode) =>
            await RunTest(value1, value2, operation, expectedValue, statusCode);

        [Fact]
        public async Task TestParserDividingByZero() =>
            await RunTest("15.6", "0", "Divide", "DivideByZero", HttpStatusCode.OK, true);

        private async Task RunTest(string value1, string value2, string operation, string expectedValueOrError,
            HttpStatusCode statusCode, bool isDividingByZero = false)
        {
            // arrange
            var url = $"/calculate?value1={value1}&operation={operation}&value2={value2}";
            
            // act
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            // assert
            Assert.True(response.StatusCode == statusCode);
            if (statusCode == HttpStatusCode.OK && !isDividingByZero)
                Assert.True(Math.Abs(decimal.Parse(expectedValueOrError, CultureInfo.InvariantCulture) -
                                     decimal.Parse(result, CultureInfo.CurrentCulture)) < Epsilon);
            else
                Assert.Contains(expectedValueOrError, result);
        }
        
        [Theory]
        [InlineData("/calculate?value1=5&operation=Something&value2=6", HttpStatusCode.BadRequest)]
        [InlineData("/calculate?value1=5&", HttpStatusCode.BadRequest)]
        [InlineData("/calculate?first=1&operation=Divide&value2=2", HttpStatusCode.BadRequest)]
        [InlineData("/calculate?value1=1&action=Divide&value2=2", HttpStatusCode.BadRequest)]
        [InlineData("/calculate?value1=1&operation=Divide&second=2", HttpStatusCode.BadRequest)]
        [InlineData("/calculate?value1=1&operation=+&value2=2", HttpStatusCode.BadRequest)]
        public async Task TestClientByWrongData(string wrongUrl, HttpStatusCode statusCode) =>
            await TestConsoleClientCalculatorAsync(wrongUrl, statusCode);

        [Theory]
        [InlineData("/calculate?value1=5&operation=Plus&value2=6", HttpStatusCode.OK)]
        [InlineData("/calculate?value1=1&operation=Divide&value2=2", HttpStatusCode.OK)]
        [InlineData("/calculate?value1=10&operation=Plus&value2=11", HttpStatusCode.OK)]
        [InlineData("/calculate?value1=1&operation=Divide&value2=0", HttpStatusCode.OK)]
        [InlineData("/calculate?value1=1&operation=Plus&value2=2", HttpStatusCode.OK)]
        public async Task TestClientByRightData(string rightUrl, HttpStatusCode statusCode) =>
            await TestConsoleClientCalculatorAsync(rightUrl, statusCode);

        private async Task TestConsoleClientCalculatorAsync(string url, HttpStatusCode expectedStatusCode)
        {

            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);
            Assert.True(response.StatusCode == expectedStatusCode);

        }
        
    }
}