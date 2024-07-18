using Domain.Contracts;
using Domain.Entities;

namespace Application.Logic;

public interface ITimeSelectionBusinessLogic
{
    Task<TimeSelection> GetById(Guid id);
    Task UpdateOldTimeSelections();
    Task NotifyUpcomingTimeSelectionAndJoinTime();
    Task<BuildOpenGraphImage> BuildOpenGraphImage(Guid id);
    Task UpdateTimeSelectionImage(UpdateTimeSelectionPreviewRequest request);
}
