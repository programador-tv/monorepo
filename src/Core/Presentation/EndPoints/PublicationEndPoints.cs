using Application.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class PublicationEndPoints
{
    public static void AddPublicationEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/publication");

        group.WithOpenApi();

        group.MapGet("all/{perfilId}", GetPublicationPerfilById);
    }

    public static async Task<IResult> GetPublicationPerfilById(
        [FromServices] IPublicationBusinessLogic _logic,
        Guid perfilId
    )
    {
        try
        {
            return Results.Ok(await _logic.GetPublicationPerfilById(perfilId));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
