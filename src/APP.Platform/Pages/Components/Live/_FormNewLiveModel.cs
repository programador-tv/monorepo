using Domain.Entities;
using Domain.Models.ViewModels;
using Domain.RequestModels;

public sealed class _FormNewLiveModel
{
    public PreLiveViewModel? Live { get; set; }
    public List<string>? TagsSelected { get; set; }
    public Dictionary<string, List<string>>? RelatioTags { get; set; }
    public ScheduleTimeSelectionRequestModel ScheduleTimeSelection { get; set; } = new();
    public ScheduleLiveForTimeSelection? ScheduleLiveForTimeSelection { get; set; }
    public ScheduleFreeTimeForTimeSelectionRequestModel? ScheduleFreeTimeForTimeSelection { get; set; }
}
