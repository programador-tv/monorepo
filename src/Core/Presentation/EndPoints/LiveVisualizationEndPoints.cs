using Application.Logic;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints
{
	public static class LiveVisualizationEndPoints
	{
		public static void AddLiveVisualizationEndPoints(this IEndpointRouteBuilder app)
		{
			var group = app.MapGroup("api/livevisualization");

			group.MapPost("/", GetLiveVisualization);
		}

		public static async Task<IResult> GetLiveVisualization([FromServices] ILiveVisualizationBusinessLogic logic, [FromBody] List<Guid> liveIds)
		{
			try
			{
				return Results.Ok(await logic.GetLiveVisualizations(liveIds));
			}
			catch (System.Exception ex)
			{
				return Results.BadRequest(ex.Message);
			}
		}

	}
}

