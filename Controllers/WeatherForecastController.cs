using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace OTELSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly Meter Meter = new("OTELSample.Metrics");
        private static readonly Counter<long> RequestCounter = Meter.CreateCounter<long>("forecast_requests_total");
        private static readonly ActivitySource ActivitySource = new("OTELSample.WeatherForecastController");
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            // forecast_requests_total Metrics
            RequestCounter.Add(1);

            // Nested logging in one session and scope
            using var activity = ActivitySource.StartActivity("GeneratingWeatherForecast");

            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            // Ilogger Sample
            _logger.LogInformation("Result has generated");
            return result;
        }
    }
}
