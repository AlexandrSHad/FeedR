using FeedR.Aggregator.Services.Models;
using FeedR.Shared.Streaming;

namespace FeedR.Aggregator.Services;

internal sealed class PricingStreamBackgroundService : BackgroundService
{
    private readonly IStreamSubscriber _streamSubscriber;
    private readonly IPricingHandler _pricingHandler;
    private readonly ILogger<PricingStreamBackgroundService> _logger;

    public PricingStreamBackgroundService(
        IStreamSubscriber streamSubscriber,
        IPricingHandler pricingHandler,
        ILogger<PricingStreamBackgroundService> logger)
    {
        _streamSubscriber = streamSubscriber;
        _pricingHandler = pricingHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _streamSubscriber.SubscribeAsync<CurrencyPair>("pricing", currencyPair =>
        {
            _logger.LogInformation("Pricing '{symbol}' = {pricing:F}, timestamp: {timestamp}",
                currencyPair.Symbol, currencyPair.Value, currencyPair.Timestamp);

            _ = _pricingHandler.HandleAsync(currencyPair);
        });
    }

}