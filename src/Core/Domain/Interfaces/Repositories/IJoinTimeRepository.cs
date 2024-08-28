using Domain.Entities;

namespace Domain.Repositories
{
    public interface IJoinTimeRepository
    {
        Task<Dictionary<Guid, Guid>> GetJoinTimePerfilIdsByTimeSelectionIds(List<Guid> ids);
        Task<Dictionary<JoinTime, TimeSelection>> GetFreeTimeMarcadosAntigos();
        Task UpdateRange(List<JoinTime> jts);
        Task<List<JoinTime>> GetJoinTimesAtivos(Guid timeId);

    }
}
