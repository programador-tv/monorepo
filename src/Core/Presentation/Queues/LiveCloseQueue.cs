using System.Text;
using System.Text.Json;
using Application.Logic;
using Contracts;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;
using Infrastructure;
using Infrastructure.FileHandling;
using Infrastructure.Repositories;
using Infrastructure.WebServices;
using MassTransit;
using Queue;

namespace Background;

public sealed class LiveCloseQueue(IVideoHandling handler, ILiveWebService webservice)
    : IConsumer<StopLiveProcessMessage>
{
    public async Task Consume(ConsumeContext<StopLiveProcessMessage> context)
    {
        try
        {
            await handler.StopAsync(context.Message.Id);
            var duration = VideoHandlingSingleton.GetM3U8DurationById(context.Message.Id);

            await webservice.FinishWithDuration(
                 new LiveThumbnailRequest(context.Message.Id, duration)
            );
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
