using System.Net.Http.Headers;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;
using Infrastructure.WebServices;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.WebServices;

public sealed class FeedbackJoinTimeWebService(CoreClient client) : IFeedbackJoinTimeWebService
{
    const string baseRoute = "api/feedbackjointime";

    public async Task Add(Guid joinTimeId)
    {
        var route = Path.Combine(baseRoute, string.Empty);
        await client.PostAsync(route, joinTimeId);
    }
}
