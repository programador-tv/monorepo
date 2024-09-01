using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Logic;

public interface IPerfilBusinessLogic
{
    Task<List<Perfil>> GetAll();
    Task<Perfil> GetById(Guid id);
    Task<List<Perfil>> GetByIds(List<Guid> ids);
    Task<Perfil> GetByToken(string token);
    Task<Perfil> GetByUsername(string username);
    Task<List<Perfil>> GetWhenContainsAsync(string keyword);
    Task AddPerfil(CreatePerfilRequest createPerfilRequest);
    Task UpdatePerfil(UpdatePerfilRequest updatePerfilRequest);
    Task UpdateFotoPerfil(Guid id, string filePath);
    Task<CreateOrUpdatePerfilResponse> TryCreateOrUpdate(CreateOrUpdatePerfilRequest request);
}
