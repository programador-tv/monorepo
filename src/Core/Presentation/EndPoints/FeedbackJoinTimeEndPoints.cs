using Application.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints
{
    public static class FeedbackJoinTimeEndpoints
    {
        public static void AddFeedbackJoinTimeEndPoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/feedbackjointime");

            group.WithOpenApi();
            group.MapPost("/", PostFeedbackJoinTime);
        }

        public static async Task<IResult> PostFeedbackJoinTime(
            [FromServices] IFeedbackJoinTimeBusinessLogic logic,
            [FromBody] Guid joinTimeId
        )
        {
            try
            {
                var response = await logic.CreateFeedbackJoinTime(joinTimeId);
                return Results.Ok(response);
            }
            catch (System.Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
