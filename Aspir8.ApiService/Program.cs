using SecondLevel;
using Grpc.Net.Client;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC service defaults
builder.AddGrpcServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add gRPC client
builder.Services.AddGrpcClient<SecondLevel.Weather.WeatherClient>((services, options) =>
{
    var grpcServerAddress = builder.Configuration["GrpcServerAddress"] ?? "http://localhost:5109";
    options.Address = new Uri(grpcServerAddress);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet("/weatherforecast", async (SecondLevel.Weather.WeatherClient client) =>
{
    try
    {
        var request = new WeatherForecastRequest { Days = 5 };
        var reply = await client.GetWeatherForecastAsync(request);

        var forecast = reply.Forecast.Select(f => new WeatherForecast(
            DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(f.Date).DateTime),
            f.TemperatureC,
            f.Summary
        )).ToArray();

        return Results.Ok(forecast);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error fetching weather forecast: {ex.Message}");
    }
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}