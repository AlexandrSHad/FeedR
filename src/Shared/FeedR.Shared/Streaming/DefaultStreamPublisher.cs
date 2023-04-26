namespace FeedR.Shared.Streaming;

class DefaultStreamPublisher : IStreamPublisher
{
    public Task PublishAsync<T>(string topic, T data) where T : class => Task.CompletedTask;
}