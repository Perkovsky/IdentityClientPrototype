using IdentityClientPrototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityClientPrototype.Controllers
{
    [Authorize]
    [ApiController]
    [Route("weather-forecast/")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("use-jwt")]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(401)]
        public ActionResult GetWeatherForecast()
        {
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var result = Enumerable.Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
            .ToArray();

            return Ok(result);
        }

        [HttpGet("allow-anonymous")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(401)]
        public ActionResult TestAllowAnonymous()
        {
            return Ok("Access is allowed.");
        }
    }
}