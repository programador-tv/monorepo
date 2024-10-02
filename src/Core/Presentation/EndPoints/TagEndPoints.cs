using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class TagEndPoints
{
    public static void AddTagEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/tag");
        group.WithOpenApi();
        group.MapPost(string.Empty, CreateTagsForLiveAndFreeTime);
    }

    public static async Task<IResult> CreateTagsForLiveAndFreeTime(
        [FromServices] ITagBusinessLogic _logic,
        [FromBody] List<CreateTagForLiveAndFreeTimeRequest> tagsForLiveAndFreeTimeRequest
    )
    {
        try
        {
            await _logic.CreateTagsForLiveAndFreeTime(tagsForLiveAndFreeTimeRequest);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
