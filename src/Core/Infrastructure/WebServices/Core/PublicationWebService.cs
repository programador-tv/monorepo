using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;

namespace Infrastructure.WebServices;

public sealed class PublicationWebService(CoreClient client) : IPublicationWebService
{
    const string baseRoute = "api/publication";

    public async Task<List<Publication>> GetPublicationPerfilById(Guid perfilId, int pageNumber)
    {
        var route = Path.Combine(baseRoute, "all", $"{perfilId}", $"{pageNumber}");
        return await client.GetAsync<List<Publication>>(route);
    }

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
