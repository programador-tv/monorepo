using System.Text;
using System.Text.Json;
using Domain.Contracts;
using MassTransit;
using Domain.WebServices;

namespace Background;

public sealed class LiveThumbnailQueue(ILiveWebService liveWebService) : IConsumer<LiveThumbnailMessage>
{
    public async Task Consume(ConsumeContext<LiveThumbnailMessage> context)
    {
        try
        {
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
