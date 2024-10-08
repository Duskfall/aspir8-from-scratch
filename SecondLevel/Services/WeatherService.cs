using Grpc.Core;

using SecondLevel;

namespace SecondLevel.Services;

public class WeatherService : Weather.WeatherBase
{
    private readonly ILogger<WeatherService> _logger;
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public WeatherService(ILogger<WeatherService> logger)
    {
        _logger = logger;
    }

    public override Task<WeatherForecastReply> GetWeatherForecast(WeatherForecastRequest request, ServerCallContext context)
    {
        var rng = new Random();
        var forecast = Enumerable.Range(1, request.Days).Select(index => new WeatherForecast
        {
            Date = DateTimeOffset.Now.AddDays(index).ToUnixTimeSeconds(),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        }).ToArray();

        var reply = new WeatherForecastReply();
        reply.Forecast.AddRange(forecast);

        return Task.FromResult(reply);
    }
}