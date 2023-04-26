using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FeedR.Shared.Redis;

public static class Extensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("redis");
        var redisOptions = section.Get<RedisOptions>();
        services.Configure<RedisOptions>(section);
        // application start fail if Redis is down, use another way to initiate it with retries for example
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions.ConnectionString!));
        
        return services;
    }
}