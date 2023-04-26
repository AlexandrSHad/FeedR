using System.Reflection;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using FeedR.Shared.Messaging;
using FeedR.Shared.Serialization;
using Microsoft.Extensions.Logging;
using IMessage = FeedR.Shared.Messaging.IMessage;

namespace FeedR.Shared.Pulsar;

internal sealed class PulsarMessageSubscriber : IMessageSubscriber
{
    private readonly ISerializer _serializer;
    private readonly ILogger<PulsarMessageSubscriber> _logger;
    private readonly IPulsarClient _client;
    private readonly string _consumerName;

    public PulsarMessageSubscriber(ISerializer serializer, ILogger<PulsarMessageSubscriber> logger)
    {
        _serializer = serializer;
        _logger = logger;
        _client = PulsarClient.Builder().Build();
        // the name of process executable, for example FeedR.Notifier
        _consumerName =
            Assembly.GetEntryAssembly()?.FullName?.Split(",")[0]?.ToLowerInvariant() ?? string.Empty;
    }

    public async Task SubscribeAsync<TMessage>(string topic, Action<MessageEnvelope<TMessage>> handler)
        where TMessage : class, IMessage
    {
        var subscription = $"{_consumerName}_{topic}";
        var consumer = _client.NewConsumer()
            .SubscriptionName(subscription)
            .Topic($"persistent://public/default/{topic}")
            .Create();

        await foreach (var message in consumer.Messages())
        {
            var customId = message.Properties["custom_id"];
            var producer = message.Properties["producer"];
            var correlationId = message.Properties["correlationId"];


            _logger.LogInformation(
                "Received message with Id: '{id}' from {producer} with correlation Id: '{correlationId}'",
                message.MessageId, producer, correlationId);
            var payload = _serializer.DeserializeBytes<TMessage>(message.Data.FirstSpan.ToArray());
            if (payload is not null)
            {
                var json = _serializer.Serialize(payload);
                _logger.LogInformation(json);

                handler(new MessageEnvelope<TMessage>(payload, correlationId));
            }

            await consumer.Acknowledge(message);
        }
    }
}