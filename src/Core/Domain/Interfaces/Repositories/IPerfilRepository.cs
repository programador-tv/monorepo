using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPerfilRepository
    {
        public Task<List<Perfil>> GetAllAsync();
        public Task<Perfil?> GetByIdAsync(Guid id);
        public Task<List<Perfil>> GetByIdsAsync(List<Guid> ids);
        public Task<Perfil?> GetByTokenAsync(string token);
        public Task<Perfil?> GetByUsernameAsync(string username);
        Task<List<Perfil>> GetWhenContainsAsync(string keyword);
        public Task AddAsync(Perfil perfil);
        public Task UpdateAsync(Perfil perfil);
    }
}
