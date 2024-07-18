using Domain.Models.ViewModels;
using Domain.RequestModels;

public class FormLiveSchedule
{
    public ScheduleTimeSelectionRequestModel ScheduleTimeSelection { get; set; } = new();
    public ScheduleLiveForTimeSelection? ScheduleLiveForTimeSelection { get; set; }
    public ScheduleFreeTimeForTimeSelectionRequestModel? ScheduleFreeTimeForTimeSelection { get; set; }
    public PreLiveViewModel? Live { get; set; }
    public List<string>? TagsSelected { get; set; }
    public Dictionary<string, List<string>>? RelatioTags { get; set; }
}
