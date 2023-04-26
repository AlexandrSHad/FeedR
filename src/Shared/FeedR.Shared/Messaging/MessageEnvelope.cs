namespace FeedR.Shared.Messaging;

public record MessageEnvelope<TMessage>(TMessage Message, string CorrelationId) where TMessage : class, IMessage;