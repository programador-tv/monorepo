using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;

namespace Infrastructure.WebServices;

public sealed class PublicationWebService(CoreClient client) : IPublicationWebService
{
    const string baseRoute = "api/publication";

    public async Task<List<Publication>> GetPublicationPerfilById(Guid perfilId)
    {
        var route = Path.Combine(baseRoute, "all", $"{perfilId}");
        return await client.GetAsync<List<Publication>>(route);
    }
}
