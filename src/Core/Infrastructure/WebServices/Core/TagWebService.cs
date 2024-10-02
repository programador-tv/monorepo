using Domain.Contracts;
using Domain.WebServices;

namespace Infrastructure.WebServices;

public sealed class TagWebService(CoreClient client) : ITagWebService
{
    const string baseRoute = "api/tag";

    public async Task CreateTagsForLiveAndFreeTime(
        List<CreateTagForLiveAndFreeTimeRequest> tagsForLiveAndFreeTimeRequest
    )
    {
        var route = Path.Combine(baseRoute, string.Empty);
        await client.PostAsync(route, tagsForLiveAndFreeTimeRequest);
    }
}
