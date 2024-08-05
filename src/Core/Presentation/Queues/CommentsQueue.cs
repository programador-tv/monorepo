using System.Text;
using System.Text.Json;
using Application.Logic;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Domain.WebServices;

namespace Background;

public sealed class CommentsQueue(IServiceScopeFactory serviceScopeFactory) : IConsumer<string>
{
    public async Task Consume(ConsumeContext<string> context)
    {
        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var commentWebService =
                    scope.ServiceProvider.GetRequiredService<ICommentWebService>();
                    
            var content = context.Message;
            await commentWebService.ValidateComment(content);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
