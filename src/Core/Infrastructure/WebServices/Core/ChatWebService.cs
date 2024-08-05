using System.Net.Http.Headers;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;
using Infrastructure.WebServices;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.WebServices;

public sealed class ChatWebService(CoreClient client) : IChatWebService
{
    const string baseRoute = "api/chat";

    public async Task Save(ChatMessage chatMessage)
    {
        var route = Path.Combine(baseRoute, string.Empty);
        await client.PostAsync(route, chatMessage);
    }
}
