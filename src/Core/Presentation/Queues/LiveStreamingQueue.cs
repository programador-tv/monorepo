using System.Net.Http.Json;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;
using Infrastructure;
using Infrastructure.FileHandling;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;

namespace Background;

public sealed class LiveStreamingQueue(
    IVideoHandling handler,
    IMemoryCache cache,
    ILiveWebService webservice
) : IConsumer<LiveChunkMessage>
{
    private readonly TimeSpan cacheDuration = TimeSpan.FromSeconds(60);

    public async Task Consume(ConsumeContext<LiveChunkMessage> context)
    {
        try
        {
            var id = Guid.Parse(context.Message.Id);

            var liveCacheKey = $"Live_{context.Message.Id}";
            var live =
                await cache.GetOrCreateAsync(
                    liveCacheKey,
                    async entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                        return await webservice.GetLiveById(id);
                    }
                ) ?? throw new Exception("live não encontrada");

            await handler.ProcessChunkAsync(id, context.Message.Chunk, 0);

            var onCacheKey = $"LiveOn_{live.Id}";
            if (!cache.TryGetValue(onCacheKey, out DateTime lastOnCall))
            {
                await webservice.KeepLiveOn(context.Message.Id);
                cache.Set(onCacheKey, DateTime.UtcNow, cacheDuration);
            }
            else if (DateTime.UtcNow - lastOnCall >= cacheDuration)
            {
                await webservice.KeepLiveOn(context.Message.Id);
                cache.Set(onCacheKey, DateTime.UtcNow, cacheDuration);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
