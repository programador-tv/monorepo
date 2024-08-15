using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class CommentEndPoints
{
    public static void AddCommentEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/comments");

        group.WithOpenApi();

        group.MapPut("validate/{id}", ValidateComment);
        group.MapGet("/getAllByLiveIdAndPerfilId/{liveId}/{perfilId}", GetAllByLiveIdAndPerfilId);
    }

    public static async Task<IResult> ValidateComment(
        [FromServices] ICommentBusinessLogic _logic,
        string id
    )
    {
        try
        {
            await _logic.ValidateComment(id);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }


    public static async Task<IResult> GetAllByLiveIdAndPerfilId(
        [FromServices] ICommentBusinessLogic _logic,
        Guid liveId,
        Guid perfilId
    )
    {
        try
        {
            var comments = await _logic.GetAllByLiveIdAndPerfilId(liveId, perfilId);
            return Results.Ok(comments);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
