namespace FeedR.Shared.Messaging;

internal sealed class DefaultMessageSubscriber : IMessageSubscriber
{
    public Task SubscribeAsync<TMessage>(string topic, Action<MessageEnvelope<TMessage>> handler) where TMessage : class, IMessage
        => Task.CompletedTask;
}