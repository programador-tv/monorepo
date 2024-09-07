using Domain.Contracts;
using Domain.WebServices;

namespace Infrastructure.WebServices;

public sealed class PublicationWebService(CoreClient client) : IPublicationWebService
{
    const string baseRoute = "api/publication";

    public async Task Add(CreatePublicationRequest request)
    {
        var route = Path.Combine(baseRoute, string.Empty);

        try
        {
            await client.PostAsync(route, request);
        }
        catch
        {
            throw new Exception(
                "Erro! Não foi possível adicionar sua publicação. Verifique a URL ou se o formato do arquivo JSON está correto."
            );
        }
    }
}
