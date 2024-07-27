using Domain.Contracts;

namespace Domain.WebServices;

public interface ITimeSelectionWebService
{
    Task<BuildOpenGraphImage> GetBuildOpenGraphImage(Guid id);
    Task UpdatePreview(UpdateTimeSelectionPreviewRequest request);
    Task UpdateOldTimeSelections();
    Task NotifyUpcomingTimeSelectionAndJoinTime();
}
