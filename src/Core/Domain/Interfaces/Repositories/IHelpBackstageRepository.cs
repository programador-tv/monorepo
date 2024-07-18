using Domain.Entities;

namespace Domain.Repositories;

public interface IHelpBackstageRepository
{
    public Task AddAsync(HelpBackstage backstage);
    public Task<HelpBackstage?> GetByTimeSelectionIdAsync(Guid id);
    public Task UpdateAsync(HelpBackstage backstage);
    public Task<List<HelpBackstage>> GetByIdsAsync(List<Guid> ids);
}
