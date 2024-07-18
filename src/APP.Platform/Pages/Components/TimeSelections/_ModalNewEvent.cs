using Domain.RequestModels;

public class _ModalNewEvent
{
    public ScheduleTimeSelectionRequestModel ScheduleTimeSelection { get; set; } = new();
    public ScheduleLiveForTimeSelection? ScheduleLiveForTimeSelection { get; set; }
    public ScheduleFreeTimeForTimeSelectionRequestModel? ScheduleFreeTimeForTimeSelection { get; set; }
    public Dictionary<string, List<string>>? RelatioTags { get; set; }
}
