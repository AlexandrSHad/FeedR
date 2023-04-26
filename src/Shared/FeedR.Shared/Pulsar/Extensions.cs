using FeedR.Shared.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FeedR.Shared.Pulsar;

public static class Extensions
{
    public static IServiceCollection AddPulsar(this IServiceCollection services)
        => services
            .AddSingleton<IMessagePublisher, PulsarMessagePublisher>()
            .AddSingleton<IMessageSubscriber, PulsarMessageSubscriber>();
}