using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class TimeSelectionEndPoints
{
    public static void AddTimeSelectionEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/timeSelections");

        group.WithOpenApi();

        group.MapGet("{id}", GetById);

        group.MapGet("UpdateOldTimeSelections", UpdateOldTimeSelections);
        group.MapGet("GetBuildOpenGraphImage/{id}", GetBuildOpenGraphImage);
        group.MapPost("UpdateOpenGraphImage", UpdateOpenGraphImage);
        group.MapGet(
            "NotifyUpcomingTimeSelectionAndJoinTime",
            NotifyUpcomingTimeSelectionAndJoinTime
        );
    }

    public static async Task<IResult> GetById(
        [FromServices] ITimeSelectionBusinessLogic _logic,
        Guid id
    )
    {
        try
        {
            var ts = await _logic.GetById(id);
            return Results.Ok(ts);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> UpdateOldTimeSelections(
        [FromServices] ITimeSelectionBusinessLogic _logic
    )
    {
        try
        {
            await _logic.UpdateOldTimeSelections();
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> NotifyUpcomingTimeSelectionAndJoinTime(
        [FromServices] ITimeSelectionBusinessLogic _logic
    )
    {
        try
        {
            await _logic.NotifyUpcomingTimeSelectionAndJoinTime();
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetBuildOpenGraphImage(
        [FromServices] ITimeSelectionBusinessLogic _logic,
        Guid id
    )
    {
        try
        {
            var build = await _logic.BuildOpenGraphImage(id);
            return Results.Ok(build);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> UpdateOpenGraphImage(
        [FromServices] ITimeSelectionBusinessLogic _logic,
        [FromBody] UpdateTimeSelectionPreviewRequest request
    )
    {
        try
        {
            await _logic.UpdateTimeSelectionImage(request);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
