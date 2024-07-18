using System.Net.Http.Headers;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;

namespace Infrastructure.WebServices;

public sealed class LiveWebService(CoreClient client) : ILiveWebService
{
    const string baseRoute = "api/lives";

    public async Task<Live> GetLiveByUrl(string url)
    {
        var route = Path.Combine(baseRoute, $"ByUrl/{url}");
        return await client.GetAsync<Live>(route);
    }

    public async Task<CreateLiveResponse> Add(CreateLiveRequest request)
    {
        var route = Path.Combine(baseRoute, string.Empty);
        return await client.PostAsync<CreateLiveResponse>(route, request);
    }

    public async Task<Live> GetLiveById(Guid id)
    {
        var route = Path.Combine(baseRoute, $"{id}");
        return await client.GetAsync<Live>(route);
    }

    public async Task NotifyUpcomingLives()
    {
        var route = Path.Combine(baseRoute, "NotifyUpcomingLives");
        await client.GetAsync(route);
    }

    public async Task UpdateThumbnail(UpdateLiveThumbnailRequest request)
    {
        var route = Path.Combine(baseRoute, "thumbnail");
        await client.PostAsync(route, request);
    }

    public async Task KeepLiveOn(string id)
    {
        var route = Path.Combine(baseRoute, $"on/{id}");
        await client.GetAsync(route);
    }

    public async Task FinishWithDuration(LiveThumbnailRequest request)
    {
        var route = Path.Combine(baseRoute, "finish");
        await client.PostAsync(route, request);
    }

    public async Task Close()
    {
        var route = Path.Combine(baseRoute, $"close");
        await client.GetAsync(route);
    }

    public async Task<Guid> GetKeyByStreamId(string streamId)
    {
        var route = Path.Combine(baseRoute, $"key/{streamId}");
        return await client.GetAsync<Guid>(route);
    }

    public async Task<string> GetTitleAndDescriptionSugestion(string tags)
    {
        var route = Path.Combine(baseRoute, $"titleAndDescriptionSugestion/{tags}");
        return await client.GetAsync(route);
    }
}
