using System.Net.Http.Headers;
using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;
using Infrastructure.WebServices;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.WebServices;

public sealed class PerfilWebService(CoreClient client) : IPerfilWebService
{
    const string baseRoute = "api/perfils";

    public async Task Add(CreatePerfilRequest request)
    {
        var route = Path.Combine(baseRoute, string.Empty);
        await client.PostAsync(route, request);
    }

    public async Task Update(UpdatePerfilRequest request)
    {
        var route = Path.Combine(baseRoute, string.Empty);
        await client.PutAsync(route, request);
    }

    public async Task UpdateFoto(Guid id, IFormFile formFile)
    {
        var route = Path.Combine(baseRoute, $"UpdateFoto/{id}");

        using var request = new MultipartFormDataContent();
        var stream = formFile.OpenReadStream();
        using var content = new StreamContent(stream);

        content.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);

        request.Add(content, "file", Path.GetFileName(formFile.FileName));

        await client.PutAsync(route, request);
    }

    public async Task<List<Perfil>> GetAll()
    {
        var route = Path.Combine(baseRoute, "All");
        return await client.GetAsync<List<Perfil>>(route);
    }

    public async Task<Perfil> GetById(Guid id)
    {
        var route = Path.Combine(baseRoute, $"{id}");
        return await client.GetAsync<Perfil>(route);
    }

    public async Task<List<Perfil>> GetAllById(List<Guid> ids)
    {
        var route = Path.Combine(baseRoute, "AllByIds");
        return await client.PostAsync<List<Perfil>>(route, ids);
    }

    public async Task<Perfil> GetByUsername(string username)
    {
        var route = Path.Combine(baseRoute, $"ByUsername/{username}");
        return await client.GetAsync<Perfil>(route);
    }

    public async Task<Perfil> GetByToken(string token)
    {
        var route = Path.Combine(baseRoute, $"ByToken/{token}");
        return await client.GetAsync<Perfil>(route);
    }

    public async Task<List<Perfil>> GetByKeyword(string keyword)
    {
        var route = Path.Combine(baseRoute, $"Contains/{keyword}");
        return await client.GetAsync<List<Perfil>>(route);
    }
}
