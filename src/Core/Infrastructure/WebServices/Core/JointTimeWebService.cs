using System.Net.Http.Headers;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;
using Infrastructure.WebServices;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.WebServices;

public sealed class JoinTimeWebService(CoreClient client) : IJoinTimeWebService
{
    const string baseRoute = "api/joinTimes";

    public async Task UpdateOldJoinTimes()
    {
        var route = Path.Combine(baseRoute, "UpdateOldJoinTimes");  
        await client.GetAsync(route);
    }
}