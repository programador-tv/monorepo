using Domain.Entities;

namespace Application.Logic;

public interface IJoinTimeBusinessLogic
{
    public Task<Dictionary<JoinTime, TimeSelection>> GetOldFreeTimesAwaitingForConclusion();
    public Task UpdateOldJoinTimes();
}
