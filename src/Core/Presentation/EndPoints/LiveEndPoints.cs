using System.Web;
using Application.Logic;
using Domain.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Presentation.EndPoints;

public static class LiveEndPoints
{
    public static void AddLiveEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/lives");

        group.WithOpenApi();

        group.MapPost(string.Empty, Add);
        group.MapGet("{id}", GetLiveById);
        group.MapGet("NotifyUpcomingLives", NotifyUpcomingLives);
        group.MapGet("ByUrl/{url}", GetLiveByUrl);
        group.MapPost("thumbnail", UpdateThumbnail);
        group.MapGet("on/{id}", KeepLiveOn);
        group.MapPost("finish", FinishWithDuration);
        group.MapGet("close", Close);
        group.MapGet("key/{streamId}", GetKeyByStreamId);
        group.MapGet("titleAndDescriptionSugestion/{tags}", GetTitleAndDescriptionSugestion);
        group.MapGet("searchLives/{keyword}", SearchLives);
    }

    public static async Task<IResult> GetLiveByUrl(
        [FromServices] ILiveBusinessLogic _logic,
        string url
    )
    {
        url = HttpUtility.UrlEncode(url);
        try
        {
            return Results.Ok(await _logic.GetLiveByUrl(url));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> Add(
        [FromServices] ILiveBusinessLogic _logic,
        [FromBody] CreateLiveRequest _request
    )
    {
        try
        {
            return Results.Ok(await _logic.AddLive(_request));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetLiveById(
        [FromServices] ILiveBusinessLogic _logic,
        [FromRoute] Guid id
    )
    {
        try
        {
            return Results.Ok(await _logic.GetLiveById(id));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> NotifyUpcomingLives([FromServices] ILiveBusinessLogic _logic)
    {
        try
        {
            await _logic.NotifyUpcomingLives();
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> UpdateThumbnail(
        [FromServices] ILiveBusinessLogic _logic,
        UpdateLiveThumbnailRequest request
    )
    {
        try
        {
            await _logic.UpdateThumbnail(request);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> KeepLiveOn(
        [FromServices] ILiveBusinessLogic _logic,
        string id
    )
    {
        try
        {
            await _logic.KeepOn(Guid.Parse(id));
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> FinishWithDuration(
        [FromServices] ILiveBusinessLogic _logic,
        LiveThumbnailRequest request
    )
    {
        try
        {
            await _logic.FinishWithDuration(request.Id, request.FormatedDate);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> Close([FromServices] ILiveBusinessLogic _logic)
    {
        try
        {
            await _logic.Close();
            return Results.Ok();
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetKeyByStreamId(
        [FromServices] ILiveBusinessLogic _logic,
        string streamId
    )
    {
        try
        {
            var id = await _logic.GetKeyByStreamId(streamId);
            return Results.Ok(id);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetTitleAndDescriptionSugestion(
        [FromServices] ILiveBusinessLogic _logic,
        string tags
    )
    {
        try
        {
            var response = await _logic.GetTitleAndDescriptionSugestion(tags);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> SearchLives(
        [FromServices] ILiveBusinessLogic _logic,
        [FromRoute] string keyword
    )
    {
        try
        {
            var response = await _logic.SearchLives(keyword);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Results.BadRequest(ex.Message);
        }
    }
}
