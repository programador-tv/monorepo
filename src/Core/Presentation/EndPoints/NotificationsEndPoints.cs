using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class NotificationsEndPoints
{
    public static void AddNotificationsEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/notifications");

        group.WithOpenApi();

        group.MapGet("{destinationId}", GetNotificationsAndMarkAsViewed);
        group.MapPost(string.Empty, SaveAndSendNotification);
    }

    public static async Task<IResult> SaveAndSendNotification(
        [FromServices] INotificationBusinessLogic _logic,
        CreateNotificationRequest request
    )
    {
        try
        {
            var notification = await _logic.SaveNotification(request);
            await _logic.NotifyWithServices(notification);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetNotificationsAndMarkAsViewed(
        [FromServices] INotificationBusinessLogic _logic,
        Guid destinationId
    )
    {
        try
        {
            return Results.Ok(await _logic.GetNotificationsAndMarkAsViewed(destinationId));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
