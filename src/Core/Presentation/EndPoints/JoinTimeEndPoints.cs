using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class JoinTimeEndPoints
{
    public static void AddJoinTimeEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/joinTimes");

        group.WithOpenApi();

        group.MapGet("/UpdateOldJoinTimes", UpdateOldJoinTimes);
    }

    public static async Task<IResult> UpdateOldJoinTimes(
        [FromServices] IJoinTimeBusinessLogic _logic
    )
    {
        try
        {
            await _logic.UpdateOldJoinTimes();
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
