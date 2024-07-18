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
}
