using Domain.Contracts;
using Domain.Entities;

namespace Application.Logic;

public interface ILiveBusinessLogic
{
    Task UpdateThumbnail(UpdateLiveThumbnailRequest updateLiveThumbnailRequest);
    Task<CreateLiveResponse> AddLive(CreateLiveRequest createLiveRequest);
    Task<Live> GetLiveById(Guid id);
    Task NotifyUpcomingLives();
    Task<Live> GetLiveByUrl(string url);
    Task KeepOn(Guid id);
    Task FinishWithDuration(Guid id, string duration);
    Task Close();
    Task<Guid> GetKeyByStreamId(string streamId);
    Task<string> GetTitleAndDescriptionSugestion(string tags);
    Task<List<Live>> SearchLives(string keyword);
}
