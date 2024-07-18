using Domain.Entities;

namespace Domain.Rules;

public sealed class RuleForJoinTimeMatchedGivenTimeSelecton
{
    public static bool IsValid(TimeSelection timeSelectiom, JoinTime joinTime) =>
        joinTime.TimeSelectionId == timeSelectiom.Id
        && RuleForSchedulePendingOrFinished.IsValid(joinTime);
}
