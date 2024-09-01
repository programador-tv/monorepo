using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Domain.WebServices;

public interface IPerfilWebService
{
    Task Add(CreatePerfilRequest request);
    Task Update(UpdatePerfilRequest request);
    Task UpdateFoto(Guid id, IFormFile formFile);
    Task<List<Perfil>> GetAll();
    Task<Perfil> GetById(Guid id);
    Task<List<Perfil>> GetAllById(List<Guid> ids);
    Task<Perfil> GetByUsername(string username);
    Task<Perfil> GetByToken(string token);
    Task<List<Perfil>> GetByKeyword(string keyword);
    Task<CreateOrUpdatePerfilResponse> TryCreateOrUpdate(CreateOrUpdatePerfilRequest request);
}
