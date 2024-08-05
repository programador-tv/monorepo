using System.Text;
using System.Text.Json;
using Domain.Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Domain.WebServices;

namespace Background;

public sealed class LiveThumbnailQueue(IServiceScopeFactory serviceScopeFactory) : IConsumer<LiveThumbnailMessage>
{
    public async Task Consume(ConsumeContext<LiveThumbnailMessage> context)
    {
        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var liveWebService =
                    scope.ServiceProvider.GetRequiredService<ILiveWebService>();

            var request = new UpdateLiveThumbnailRequest(
                Guid.Parse(context.Message.Id),
                context.Message.Image
            );
            await liveWebService.UpdateThumbnail(request);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
