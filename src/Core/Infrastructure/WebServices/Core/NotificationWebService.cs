using System.Net.Http.Headers;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;
using Infrastructure.WebServices;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.WebServices;

public sealed class NotificationWebService(CoreClient client) : INotificationWebService
{
    const string baseRoute = "api/notifications";

    public async Task SaveAndSend(CreateNotificationRequest request)
    {
        var route = Path.Combine(baseRoute, string.Empty);
        await client.PostAsync(route, request);
    }

    public async Task<List<NotificationItemResponse>> GetById(Guid id)
    {
        var route = Path.Combine(baseRoute, $"{id}");
        return await client.GetAsync<List<NotificationItemResponse>>(route);
    }
}
