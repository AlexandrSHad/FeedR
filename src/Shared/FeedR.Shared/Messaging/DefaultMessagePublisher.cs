namespace FeedR.Shared.Messaging;

internal sealed class DefaultMessagePublisher : IMessagePublisher
{
    public Task PublishAsync<TMessage>(string topic, TMessage message) where TMessage : class, IMessage
        => Task.CompletedTask;
}