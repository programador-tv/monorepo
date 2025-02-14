using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Logic;

public sealed class PerfilBusinessLogic(IPerfilRepository _repository) : IPerfilBusinessLogic
{
    public async Task<List<Perfil>> GetAll() => await _repository.GetAllAsync();

    public async Task<Perfil> GetById(Guid id) =>
        await _repository.GetByIdAsync(id)
        ?? throw new KeyNotFoundException("Perfil não encontrado");

    public async Task<List<Perfil>> GetByIds(List<Guid> ids) =>
        await _repository.GetByIdsAsync(ids)
        ?? throw new KeyNotFoundException("Perfil não encontrado");

    public async Task<List<Perfil>> GetWhenContainsAsync(string keyword) =>
        await _repository.GetWhenContainsAsync(keyword)
        ?? throw new KeyNotFoundException("Perfil não encontrado");

    public async Task<Perfil> GetByToken(string token) =>
        await _repository.GetByTokenAsync(token)
        ?? throw new KeyNotFoundException("Perfil não encontrado");

    public async Task<Perfil> GetByUsername(string username) =>
        await _repository.GetByUsernameAsync(username)
        ?? throw new KeyNotFoundException("Perfil não encontrado");

    public async Task AddPerfil(CreatePerfilRequest createPerfilRequest)
    {
        var perfil = Perfil.Create(createPerfilRequest);

        await _repository.AddAsync(perfil);
    }

    public async Task UpdatePerfil(UpdatePerfilRequest updatePerfilRequest)
    {
        var perfil =
            await _repository.GetByIdAsync(updatePerfilRequest.Id)
            ?? throw new KeyNotFoundException("Perfil não encontrado");

        perfil.Update(updatePerfilRequest);

        await _repository.UpdateAsync(perfil);
    }

    public async Task UpdateFotoPerfil(Guid id, string filePath)
    {
        var perfil =
            await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Perfil não encontrado");

        perfil.UpdateFoto(filePath);

        await _repository.UpdateAsync(perfil);
    }
}
