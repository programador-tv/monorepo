using System.Text;
using System.Text.Json;
using Application.Logic;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories;
using MassTransit;

namespace Background;

public sealed class CommentsQueue(IHttpClientFactory factory) : IConsumer<string>
{
    public async Task Consume(ConsumeContext<string> context)
    {
        using var client = factory.CreateClient("CoreAPI");

        try
        {
            var content = new StringContent(string.Empty);
            await client.PutAsync("api/comments/validate/" + context.Message, content);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
