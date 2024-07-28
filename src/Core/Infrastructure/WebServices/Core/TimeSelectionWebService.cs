using System.Net.Http.Headers;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;
using Infrastructure.WebServices;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.WebServices;

public sealed class TimeSelectionWebService(CoreClient client) : ITimeSelectionWebService
{
    const string baseRoute = "api/timeSelections";

    public async Task<BuildOpenGraphImage> GetBuildOpenGraphImage(Guid id)
    {
        var route = Path.Combine(baseRoute, $"GetBuildOpenGraphImage/{id}");
        return await client.GetAsync<BuildOpenGraphImage>(route);
    }

    public async Task UpdatePreview(UpdateTimeSelectionPreviewRequest request)
    {
        var route = Path.Combine(baseRoute, "UpdateOpenGraphImage");
        await client.PostAsync(route, request);
    }

    public async Task UpdateOldTimeSelections()
    {
        var route = Path.Combine(baseRoute, "UpdateOldTimeSelections");
        await client.GetAsync(route);
    }
    
    public async Task  NotifyUpcomingTimeSelectionAndJoinTime()
    {
        var route = Path.Combine(baseRoute, "NotifyUpcomingTimeSelectionAndJoinTime");
        await client.GetAsync(route);
    }
}
