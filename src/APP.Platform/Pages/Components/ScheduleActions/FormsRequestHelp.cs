using Domain.RequestModels;

public class FormsRequestHelp
{
    public ScheduleTimeSelectionRequestModel ScheduleTimeSelection { get; set; } = new();
    public ScheduleFreeTimeForTimeSelectionRequestModel? ScheduleFreeTimeForTimeSelection { get; set; }
    public Dictionary<string, List<string>>? RelatioTags { get; set; }
}
