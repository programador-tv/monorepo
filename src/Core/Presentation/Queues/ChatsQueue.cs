using System.Text;
using System.Text.Json;
using Application.Logic;
using Domain.Entities;
using Domain.WebServices;
using Infrastructure;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Background;

public sealed class ChatsQueue(IServiceScopeFactory serviceScopeFactory) : IConsumer<ChatMessage>
{
    public async Task Consume(ConsumeContext<ChatMessage> context)
    {
        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var chatWebService = scope.ServiceProvider.GetRequiredService<IChatWebService>();

            var chatMessage = context.Message;
            await chatWebService.Save(chatMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
