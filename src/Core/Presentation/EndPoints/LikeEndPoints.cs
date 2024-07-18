using Application.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class LikeEndPoints
{
    public static void AddLikeEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/like");

        group.WithOpenApi();

        group.MapGet("getLikesByLiveId/{liveId}", GetLikesLiveById);
    }

    public static async Task<IResult> GetLikesLiveById(
        [FromServices] ILikeBusinessLogic _logic,
        Guid liveId
    )
    {
        try
        {
            var response = await _logic.GetLikesByLiveId(liveId);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
