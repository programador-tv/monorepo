using System.Diagnostics;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface ILiveRepository
    {
        Task<Live> AddAsync(Live live);
        Task<Live> GetByIdAsync(Guid id);
        Task<Dictionary<Live, List<NotifyUserLiveEarly>>> GetUpcomingLives();
        Task UpdateRangeNotifyUserLiveEarly(List<NotifyUserLiveEarly> notifyUsers);
        Task<Live> GetByUrlAsync(string url);
        Task UpdateAsync(Live live);
        Task UpdateRangeAsync(List<Live> lives);
        Task<Guid> GetKeyByStreamId(string streamId);
        Task<List<Live>> SearchBySpecificTitle(string keyword);
        Task<List<Live>> SearchByTitleContaining(string keyword);
        Task<List<Live>> SearchByDescriptionContaining(string keyword);
        Task<List<Live>> SearchByTagContaining(List<Tag> tags);
        Task<List<Live>> SearchByListPerfilId(List<Perfil> perfils);
        Task<List<Guid>> CloseNonUpdatedLiveRangeAsync();
    }
}
