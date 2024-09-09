using System.Net.Http.Headers;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;

namespace Infrastructure.WebServices;

public sealed class LikeWebService(CoreClient client) : ILikeWebService
{
    const string baseRoute = "api/like";

    public async Task CreateLike(CreateLikeRequest createLikeRequest)
    {
        var route = Path.Combine(baseRoute, $"createLike");
        await client.PostAsync(route, createLikeRequest);
    }
}
