using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Rules;

public sealed class RuleForVideoEstaVisivel
{
    public static bool IsValid(Live live)
    {
        return !live.LiveEstaAberta && live.Visibility;
    }
}
