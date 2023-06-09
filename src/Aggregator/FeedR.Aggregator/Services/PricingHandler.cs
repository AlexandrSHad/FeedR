using FeedR.Aggregator.Events;
using FeedR.Aggregator.Services.Models;
using FeedR.Shared.Messaging;
using Pipelines.Sockets.Unofficial.Arenas;

namespace FeedR.Aggregator.Services;

internal sealed class PricingHandler : IPricingHandler
{
    private int _counter;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<PricingHandler> _logger;

    public PricingHandler(IMessagePublisher messagePublisher, ILogger<PricingHandler> logger)
    {
        _messagePublisher = messagePublisher;
        _logger = logger;
    }
    
    public async Task HandleAsync(CurrencyPair currencyPair)
    {
        //TODO: implement some actual business logic
        if (ShouldPlaceOrder())
        {
            // just dummy implementation, here could be a logic to place order
            var orderId = Guid.NewGuid().ToString("N");

            _logger.LogInformation("Order with Id: {id} has been created for symbol: {symbol}."
                , orderId, currencyPair.Symbol);

            var integrationEvent = new OrderPlaced(orderId, currencyPair.Symbol);
            await _messagePublisher.PublishAsync("orders", integrationEvent);
        }
    }

    private bool ShouldPlaceOrder() => Interlocked.Increment(ref _counter) % 10 == 0;
}