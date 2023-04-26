using FeedR.Notifier.Events.External;
using FeedR.Shared.Messaging;

namespace FeedR.Notifier.Services;

internal sealed class NotifierMessagingBackgroundService : BackgroundService
{
    private readonly IMessageSubscriber _messageSubscriber;
    private readonly ILogger<NotifierMessagingBackgroundService> _logger;

    public NotifierMessagingBackgroundService(
        IMessageSubscriber messageSubscriber,
        ILogger<NotifierMessagingBackgroundService> logger)
    {
        _messageSubscriber = messageSubscriber;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageSubscriber.SubscribeAsync<OrderPlaced>("orders", messageEnvelope =>
        {
            _logger.LogInformation(
                "Order with Id: '{id}' for symbol '{symbol}' has been placed. Correlation Id: '{correlationId}'",
                messageEnvelope.Message.OrderId, messageEnvelope.Message.Symbol, messageEnvelope.CorrelationId);
        });
        
        return Task.CompletedTask;
    }
}