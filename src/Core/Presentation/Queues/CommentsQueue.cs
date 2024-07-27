using System.Text;
using System.Text.Json;
using Application.Logic;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories;
using MassTransit;
using Domain.WebServices;

namespace Background;

public sealed class CommentsQueue(ICommentWebService commentWebService) : IConsumer<string>
{
    public async Task Consume(ConsumeContext<string> context)
    {
        try
        {
            var content = context.Message;
            await commentWebService.ValidateComment(content);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
