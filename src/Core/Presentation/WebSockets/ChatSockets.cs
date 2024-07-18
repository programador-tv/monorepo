using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Queue;

namespace Presentation.Websockets;

public static class ChatSockets
{
    public static void AddChatSockets(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("ws/chat");

        group.WithOpenApi();
    }
}
