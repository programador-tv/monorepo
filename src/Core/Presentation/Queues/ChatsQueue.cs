using System.Text;
using System.Text.Json;
using Application.Logic;
using Domain.Entities;
using Domain.WebServices;
using Infrastructure;
using Infrastructure.Repositories;
using MassTransit;

namespace Background;

public sealed class ChatsQueue(IChatWebService chatWebService) : IConsumer<ChatMessage>
{
    public async Task Consume(ConsumeContext<ChatMessage> context)
    {
        try
        {
            var chatMessage = context.Message;
            await chatWebService.Save(chatMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
