using Domain.Entities;

namespace Domain.RequestModels;

public sealed class ScheduleFreeTimeForTimeSelectionRequestModel
{
    public FreeTimeBackstage? TimeSelectionBackstage { get; set; } = new();
}
