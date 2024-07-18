using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Rules;

public sealed class RuleForLiveOuVideoEstaPublico
{
    public static Expression<Func<Live, bool>> IsValid()
    {
        return live => live.LiveEstaAberta || RuleForVideoEstaVisivel.IsValid(live);
    }
}
