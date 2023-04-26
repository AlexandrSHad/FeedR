namespace FeedR.Shared.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(string topic, TMessage message) where TMessage : class, IMessage;
}