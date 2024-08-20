using Application.Logic;
using Domain.Contracts;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class HelpResponseEndPoints
{
    public static void AddHelpResponseEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/help-response");

        group.WithOpenApi();

        group.MapGet("all/{timeSelectionId}", GetAll);
        group.MapPost(string.Empty, Save);
        group.MapGet("{helpResponseId}", Update);
    }

    public static async Task<IResult> GetAll(
        [FromServices] IHelpResponseBusinessLogic _logic,
        Guid timeSelectionId
    )
    {
        try
        {
            return Results.Ok(await _logic.GetAll(timeSelectionId));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> Save(
        [FromServices] IHelpResponseBusinessLogic _logic,
        [FromBody] CreateHelpResponse helpResponse
    )
    {
        try
        {
            await _logic.Add(helpResponse);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> Update(
        [FromServices] IHelpResponseBusinessLogic _logic,
        Guid helpResponseId
    )
    {
        try
        {
            await _logic.Delete(helpResponseId);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
