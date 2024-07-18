using Domain.Entities;
using Domain.Enums;

namespace Domain.Rules;

public sealed class RuleForSchedulePendingOrFinished
{
    public static bool IsValid(JoinTime joinTime) =>
        joinTime.StatusJoinTime == StatusJoinTime.Marcado
        || joinTime.StatusJoinTime == StatusJoinTime.ConclusaoPendente
        || joinTime.StatusJoinTime == StatusJoinTime.Concluído;
}
