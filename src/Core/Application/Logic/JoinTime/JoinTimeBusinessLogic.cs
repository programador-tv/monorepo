using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Queue;

namespace Application.Logic;

public sealed class JoinTimeBusinessLogic(
    ITimeSelectionRepository timeSelectionRepository,
    IJoinTimeRepository joinTimeRepository,
    IMessagePublisher _messagePublisher
) : IJoinTimeBusinessLogic
{
    public async Task<Dictionary<JoinTime, TimeSelection>> GetOldFreeTimesAwaitingForConclusion() =>
        await joinTimeRepository.GetFreeTimeMarcadosAntigos();

#warning na verdade UpdateOldJoinTimes deveria ter um nome associado a passar jt para conclusão pendente e notificar isso
    public async Task UpdateOldJoinTimes()
    {
        var JtsAndTss = await GetOldFreeTimesAwaitingForConclusion();
        var jtsToBeNotified = new Dictionary<JoinTime, TimeSelection>();

        foreach (var jtAndTs in JtsAndTss)
        {
            if (jtAndTs.Value.Variacao != Variacao.OneToOne)
            {
                jtAndTs.Key.ChangeStatus(StatusJoinTime.Concluído);
            }
            else
            {
                jtAndTs.Key.ChangeStatus(StatusJoinTime.ConclusaoPendente);
                jtsToBeNotified[jtAndTs.Key] = jtAndTs.Value;
            }
        }

        await joinTimeRepository.UpdateRange([.. JtsAndTss.Keys]);

        var jtIds = jtsToBeNotified.Select(ts => ts.Key.Id).ToList();

        var perfilGeradorHash =
            await timeSelectionRepository.GetTimeSelectionPerfilIdsByJoinTimeIds(jtIds);
        foreach (var jtAndTs in jtsToBeNotified)
        {
            var perfilGerador = perfilGeradorHash[jtAndTs.Value.Id];
            var notification = jtAndTs.Key.BuildPendingNotification(
                perfilGerador,
                jtAndTs.Value.TituloTemporario ?? string.Empty
            );
            await _messagePublisher.PublishAsync("NotificationsQueue", notification);
        }
    }
}
