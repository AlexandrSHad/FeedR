using FeedR.Shared.Streaming;

namespace FeedR.Aggregator.Services;

internal sealed class WeatherStreamBackgroundService : BackgroundService
{
    private readonly IStreamSubscriber _streamSubscriber;
    private readonly ILogger<PricingStreamBackgroundService> _logger;

    public WeatherStreamBackgroundService(
        IStreamSubscriber streamSubscriber,
        ILogger<PricingStreamBackgroundService> logger)
    {
        _streamSubscriber = streamSubscriber;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _streamSubscriber.SubscribeAsync<WeatherData>("weather", weather =>
        {
            _logger.LogInformation($"{weather.Location}: {weather.Temperature} C, {weather.Humidity} %, " +
                                   $"{weather.WindSpeed} km/h, [{weather.Condition}]");
        });
    }

    private record WeatherData(
        string Location,
        double Temperature,
        double Humidity,
        double WindSpeed,
        string Condition);
}