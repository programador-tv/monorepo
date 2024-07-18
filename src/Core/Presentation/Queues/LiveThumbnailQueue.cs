using System.Text;
using System.Text.Json;
using Domain.Contracts;
using MassTransit;

namespace Background;

public sealed class LiveThumbnailQueue(IHttpClientFactory factory) : IConsumer<LiveThumbnailMessage>
{
    public async Task Consume(ConsumeContext<LiveThumbnailMessage> context)
    {
        try
        {
            using var client = factory.CreateClient("CoreAPI");
            var request = new UpdateLiveThumbnailRequest(
                Guid.Parse(context.Message.Id),
                context.Message.Image
            );

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync("api/lives/thumbnail", content);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
