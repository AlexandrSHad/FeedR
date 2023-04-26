namespace FeedR.Shared.Streaming;

class DefaultStreamSubscriber : IStreamSubscriber
{
    public Task SubscribeAsync<T>(string topic, Action<T> handler) where T : class => Task.CompletedTask;
}