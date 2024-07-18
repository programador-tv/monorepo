using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Domain.WebServices;

public interface ILiveWebService
{
    Task<Live> GetLiveByUrl(string url);
    Task<CreateLiveResponse> Add(CreateLiveRequest request);
    Task<Live> GetLiveById(Guid id);
    Task NotifyUpcomingLives();
    Task UpdateThumbnail(UpdateLiveThumbnailRequest request);
    Task KeepLiveOn(string id);
    Task FinishWithDuration(LiveThumbnailRequest request);
    Task Close();
    Task<Guid> GetKeyByStreamId(string streamId);
    Task<string> GetTitleAndDescriptionSugestion(string tags);
}
