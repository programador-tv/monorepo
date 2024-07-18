using System.Text;
using System.Text.Json;
using Application.Logic;
using Contracts;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories;
using MassTransit;

namespace Background;

public sealed class EmailsQueue(IEmailHandling emailHandling) : IConsumer<BuildedEmail>
{
    public async Task Consume(ConsumeContext<BuildedEmail> context)
    {
        try
        {
            var mail = emailHandling.LoadTemplate(context.Message);
            await emailHandling.SendAsync(context.Message, mail);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
