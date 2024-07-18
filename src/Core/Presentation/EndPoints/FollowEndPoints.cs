using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class FollowEndPoints
{
    public static void AddFollowEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/follow");

        group.WithOpenApi();

        group.MapGet("toggleFollow/{followerId}/{followingId}", ToggleFollow);
        group.MapGet("getFollowInformation/{userId}", GetFollowInformation);
        group.MapPost("getFollowersCount", GetFollowersCount);
    }

    public static async Task<IResult> ToggleFollow(
        [FromServices] IFollowBusinessLogic _logic,
        Guid followerId,
        Guid followingId
    )
    {
        try
        {
            var response = await _logic.ToggleFollow(followerId, followingId);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetFollowInformation(
        [FromServices] IFollowBusinessLogic _logic,
        Guid userId
    )
    {
        try
        {
            var response = await _logic.GetFollowInformation(userId);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetFollowersCount(
        [FromServices] IFollowBusinessLogic _logic,
        [FromBody] FollowersRequest request
    )
    {
        try
        {
            var response = await _logic.GetFollowersCount(request.UsersId);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
