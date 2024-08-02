using System.Net.Http.Headers;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;
using Infrastructure.WebServices;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.WebServices;

public sealed class CommentWebService(CoreClient client) : ICommentWebService
{
    const string baseRoute = "api/comments";

    public async Task ValidateComment(string id)
    {
        var route = Path.Combine(baseRoute, $"validate/{id}");
        await client.PutAsync(route, id);
    }
}
