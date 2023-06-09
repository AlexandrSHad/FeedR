using FeedR.Shared.Streaming;

namespace FeedR.Feeds.Weather.Services;

internal sealed class WeatherBackgroundService : BackgroundService
{
    // Get IWeatherFeed from ServiceProvider with Scoped lifetime instead of
    // injecting it directly. Reason:
    //private readonly IWeatherFeed _weatherFeed;
    private readonly IServiceProvider _serviceProvider;
    private readonly IStreamPublisher _streamPublisher;

    private readonly ILogger<WeatherBackgroundService> _logger;

    public WeatherBackgroundService(
        IServiceProvider serviceProvider,
        //IWeatherFeed weatherFeed,
        ILogger<WeatherBackgroundService> logger, IStreamPublisher streamPublisher)
    {
        _serviceProvider = serviceProvider;
        //_weatherFeed = weatherFeed;
        _logger = logger;
        _streamPublisher = streamPublisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var weatherFeed = scope.ServiceProvider.GetRequiredService<IWeatherFeed>();
        
        await foreach (var weather in weatherFeed.SubscribeAsync(location: "Prague", stoppingToken))
        {
            _logger.LogInformation($"{weather.Location}: {weather.Temperature} C, {weather.Humidity} %, " +
                                   $"{weather.WindSpeed} km/h, [{weather.Condition}]");

            await _streamPublisher.PublishAsync("weather", weather);
        }
    }
}