using FeedR.Feeds.Quotes.Pricing.Requests;
using FeedR.Shared.Streaming;

namespace FeedR.Feeds.Quotes.Pricing.Services;

internal class PricingBackgroundService : BackgroundService
{
    private int _runningStatus;
    private readonly IPricingGenerator _pricingGenerator;
    private readonly PricingRequestsChannel _requestsChannel;
    private readonly IStreamPublisher _streamPublisher;
    private readonly ILogger<PricingBackgroundService> _logger;

    public PricingBackgroundService(
        IPricingGenerator pricingGenerator,
        PricingRequestsChannel requestsChannel,
        IStreamPublisher streamPublisher,
        ILogger<PricingBackgroundService> logger)
    {
        _pricingGenerator = pricingGenerator;
        _requestsChannel = requestsChannel;
        _streamPublisher = streamPublisher;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Pricing background service has started.");
        
        await foreach (var request in _requestsChannel.Requests.Reader.ReadAllAsync(stoppingToken))
        {
            _logger.LogInformation("Pricing background service has received the request: {requestType}.",
                request.GetType().Name);
            
            var _ = request switch
            {
                StartPricing => StartGeneratorAsync(),
                StopPricing => StopGeneratorAsync(),
                _ => Task.CompletedTask
            };
        }
        
        _logger.LogInformation("Pricing background service has stopped.");
    }

    private async Task StartGeneratorAsync()
    {
        if (Interlocked.Exchange(ref _runningStatus, 1) == 1)
        {
            _logger.LogInformation("Pricing generator is already started.");
            return;
        }
        
        // Redis Pub/Sub is good if you do not care if the data gets lost just for streaming the data
        // without persistence. This is different from business events. 
        // In this example we are just streaming actual data and do not care about the data 10 minutes old.
        // If you need a persistence or guarantee the data to be delivered use the message broker.
        await foreach (var currencyPair in _pricingGenerator.StartAsync())
        {
            _logger.LogInformation("Publishing the currency pair...");
            await _streamPublisher.PublishAsync("pricing", currencyPair);
        }
    }

    private async Task StopGeneratorAsync()
    {
        if (Interlocked.Exchange(ref _runningStatus, 0) == 0)
        {
            _logger.LogInformation("Pricing generator is not started.");
            return;
        }
        
        await _pricingGenerator.StopAsync();
    }
}