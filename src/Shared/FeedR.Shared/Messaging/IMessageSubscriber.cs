namespace FeedR.Shared.Messaging;

public interface IMessageSubscriber
{
    Task SubscribeAsync<TMessage>(string topic, Action<MessageEnvelope<TMessage>> handler)
        where TMessage : class, IMessage;
}