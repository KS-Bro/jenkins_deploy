using Jenkins_Deploy.Controllers;
using Microsoft.Extensions.Logging.Abstractions;

namespace Jenkins_deploy_Test
{
    public class WeatherForecastControllerTests
    {
        private WeatherForecastController CreateController()
        {
            return new WeatherForecastController(NullLogger<WeatherForecastController>.Instance);
        }

        // ---------------------- Positive scenarios ----------------------

        [Fact]
        public void Get_ReturnsExactlyFiveForecasts()
        {
            // Positive scenario
            var controller = CreateController();

            var result = controller.Get();

            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void Get_ReturnsForecasts_WithFutureDates()
        {
            // Positive scenario
            var controller = CreateController();
            var today = DateOnly.FromDateTime(DateTime.Now);

            var result = controller.Get();

            Assert.All(result, forecast => Assert.True(forecast.Date > today));
        }

        [Fact]
        public void Get_ReturnsForecasts_WithNonEmptySummary()
        {
            // Positive scenario
            var controller = CreateController();

            var result = controller.Get();

            Assert.All(result, forecast => Assert.False(string.IsNullOrWhiteSpace(forecast.Summary)));
        }

        [Fact]
        public void Get_ReturnsForecasts_WithValidSummaryFromKnownList()
        {
            // Positive scenario
            var controller = CreateController();
            var validSummaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            var result = controller.Get();

            Assert.All(result, forecast => Assert.Contains(forecast.Summary, validSummaries));
        }

        // ---------------------- Negative scenarios ----------------------

        [Fact]
        public void Get_ReturnsForecasts_WithTemperatureWithinExpectedRange()
        {
            // Negative scenario: ensure no out-of-range values slip through (-20 to 54 inclusive)
            var controller = CreateController();

            var result = controller.Get();

            Assert.All(result, forecast =>
            {
                Assert.True(forecast.TemperatureC >= -20, $"TemperatureC {forecast.TemperatureC} is below the minimum expected value.");
                Assert.True(forecast.TemperatureC < 55, $"TemperatureC {forecast.TemperatureC} is at or above the maximum expected value.");
            });
        }

        [Fact]
        public void Get_DoesNotReturnNull()
        {
            // Negative scenario: result should never be null
            var controller = CreateController();

            var result = controller.Get();

            Assert.NotNull(result);
        }

        [Fact]
        public void Get_DoesNotReturnDuplicateDates()
        {
            // Negative scenario: dates are sequential (index 1-5), so duplicates indicate a bug
            var controller = CreateController();

            var result = controller.Get().ToList();
            var distinctDateCount = result.Select(f => f.Date).Distinct().Count();

            Assert.Equal(result.Count, distinctDateCount);
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsOrAllowsGracefully()
        {
            // Negative scenario: passing a null logger should not crash unexpectedly when Get() is called.
            // ASP.NET Core controllers commonly accept a null logger without throwing at construction time.
            var controller = new WeatherForecastController(null!);

            var exception = Record.Exception(() => controller.Get().ToList());

            Assert.Null(exception);
        }
    }
}
