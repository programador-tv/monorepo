using Application.Logic;
using Domain.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class HelpBackstagEndPoints
{
    public static void AddHelpBackstageEndPoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/helpbackstage");

        // group.WithOpenApi();

        group.MapPost("{timeSelectionId}/{description}", PostNewHelpBackstage);
        group.MapPost("SaveImageFile/{timeSelectionId}", PostSaveImageFile).DisableAntiforgery();
        group.MapPost("AllByIds", GetAllByIds);
    }

    public static async Task<IResult> PostNewHelpBackstage(
        [FromServices] IHelpBackstageBusinessLogic _logic,
        Guid timeSelectionId,
        string description
    )
    {
        try
        {
            var request = new CreateHelpBackstageRequest(timeSelectionId, description);
            return Results.Ok(await _logic.ScheduleResquestedHelp(request));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> PostSaveImageFile(
        [FromServices] IHelpBackstageBusinessLogic _logic,
        Guid timeSelectionId,
        [FromForm] IFormFile file
    )
    {
        try
        {
            await _logic.SaveImageFile(timeSelectionId, file);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetAllByIds(
        [FromServices] IHelpBackstageBusinessLogic _logic,
        [FromBody] List<Guid> ids
    )
    {
        try
        {
            return Results.Ok(await _logic.GetByIds(ids));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
