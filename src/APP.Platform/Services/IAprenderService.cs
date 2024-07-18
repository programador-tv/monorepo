using Domain.Entities;
using Domain.Models.ViewModels;

namespace Platform.Services;

public interface IAprenderService
{
    public bool ShouldAddToOldList(TimeSelection e);
    public bool SetActionNeeded(TimeSelection e);
    public List<TimeSelectionForCalendarSectionViewModel> CastTimeSelectionIntoCalendarSectionViewModel(
        List<TimeSelection> timeSeletions
    );
    public List<BadFinishedTimeSelectionForCalendarSectionViewModel> CastTimeSelectionIntoBadFinishedCalendarSectionViewModel(
        List<TimeSelection> timeSeletions
    );
    public void GetMyEvents(
        Dictionary<TimeSelection, List<JoinTime>> associatedTimeSelectionAndJoinTimes,
        Guid userId,
        string _meetUrl,
        Dictionary<JoinTime, TimeSelection>? MyEvents,
        Dictionary<JoinTime, TimeSelection> OldMyEvents,
        string? userNome
    );
    public Dictionary<TimeSelection, List<JoinTime>> GetMyTimeSelectionAndJoinTimes(
        Guid userId,
        string _meetUrl
    );
}
