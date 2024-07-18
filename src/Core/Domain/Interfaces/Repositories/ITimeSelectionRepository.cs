using Domain.Entities;

namespace Domain.Repositories
{
    public interface ITimeSelectionRepository
    {
        Task<TimeSelection> GetById(Guid id);
        Task<List<TimeSelection>> GetFreeTimeMarcadosAntigos();
        Task<Dictionary<Guid, Guid>> GetTimeSelectionPerfilIdsByJoinTimeIds(List<Guid> ids);
        Task UpdateRange(List<TimeSelection> tss);
        Task<Dictionary<TimeSelection, List<JoinTime>>> GetUpcomingTimeSelectionAndJoinTime();
    }
}
