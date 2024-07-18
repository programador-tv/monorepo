using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class ChatEndPoints
{
    public static void AddChatEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/chat");

        group.WithOpenApi();

        group.MapPost(string.Empty, Save);
    }

    public static async Task<IResult> Save(
        [FromServices] IChatBusinessLogic _logic,
        ChatMessage chatMessage
    )
    {
        try
        {
            await _logic.SaveAsync(chatMessage);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
