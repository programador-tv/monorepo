using System.Globalization;
using System.Net.Http.Headers;
using Domain.WebServices;

namespace Infrastructure.WebServices;

public sealed class OpenaiWebService(OpenaiClient client) : IOpenaiWebService
{
    const string completationRoute = "engines/text-davinci-003/completions";

    public async Task<string> GetCompletationResponse(string prompt)
    {
        var body =
            "{\n \"prompt\": \""
            + prompt
            + "\",\n  \"temperature\": "
            + ("0.5".ToString(CultureInfo.InvariantCulture))
            + ",\n  \"max_tokens\": "
            + 1500
            + ",\n   \"top_p\": "
            + 1
            + ",\n  \"frequency_penalty\": "
            + 0
            + ",\n  \"presence_penalty\": "
            + 0
            + "\n}";

        var content = new StringContent(body, MediaTypeHeaderValue.Parse("application/json"));

        return await client.PostAsync<string>(completationRoute, content);
    }
}
