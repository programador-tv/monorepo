using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;

namespace Infrastructure.WebServices;

public sealed class HelpResponseWebService(CoreClient client) : IHelpResponseWebService
{
    const string baseRoute = "api/help-response";

    public async Task<HelpResponse> Add(CreateHelpResponse helpResponse)
    {
        var route = Path.Combine(baseRoute, string.Empty);
        var helpResponseCreated = await client.PostAsync<HelpResponse>(route, helpResponse);
        return helpResponseCreated;
    }

    public async Task Update(Guid helpResponseId)
    {
        var route = Path.Combine(baseRoute, $"{helpResponseId}");
        await client.GetAsync(route);
    }

    public async Task<List<HelpResponse>> GetAll(Guid timeSelectionId)
    {
        var route = Path.Combine(baseRoute, "all", $"{timeSelectionId}");
        return await client.GetAsync<List<HelpResponse>>(route);
    }

    public async Task<HelpResponse> GetById(Guid id)
    {
        var route = Path.Combine(baseRoute, $"{id}");
        return await client.GetAsync<HelpResponse>(route);
    }
}
