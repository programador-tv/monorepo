using Domain.WebServices;

namespace Infrastructure.WebServices;

public sealed class BocaSujanWebService(CoreClient client) : IBocaSujaWebService
{
    const string baseRoute = "api/v1";

    public async Task<string> Validate(string text, Guid id)
    {
        var route = Path.Combine(baseRoute, $"validate?id={id}&text={text}");
        var result = await client.GetAsync(route);

        return result;
    }
}
