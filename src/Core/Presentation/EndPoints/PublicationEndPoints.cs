using Application.Logic;
using Domain.Contracts;
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

        group.MapPost(string.Empty, Add);
    }

    public static async Task<IResult> Add(
        [FromServices] IPublicationBusinessLogic _logic,
        [FromBody] CreatePublicationRequest _request
    )
    {
        try
        {
            await _logic.AddPublication(_request);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
