using System.Text;
using System.Text.Json;
using Application.Logic;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories;
using MassTransit;

namespace Background;

public sealed class ChatsQueue(IHttpClientFactory factory) : IConsumer<ChatMessage>
{
    public async Task Consume(ConsumeContext<ChatMessage> context)
    {
        using var client = factory.CreateClient("CoreAPI");

        var json = JsonSerializer.Serialize(context.Message);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var SaveChat = client.PostAsync("api/chats", content);

        try
        {
            await SaveChat;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
