using Domain.Contracts;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Repositories;
using Queue;

namespace Application.Logic;

public sealed class TimeSelectionBusinessLogic(
    ITimeSelectionRepository timeSelectionRepository,
    IJoinTimeRepository joinTimeRepository,
    IPerfilRepository perfilRepository,
    ITagRepository tagRepository,
    IMessagePublisher _messagePublisher
) : ITimeSelectionBusinessLogic
{
    public async Task<TimeSelection> GetById(Guid id) => await timeSelectionRepository.GetById(id);

    public async Task UpdateOldTimeSelections()
    {
        var tss = await timeSelectionRepository.GetFreeTimeMarcadosAntigos();
        var tssToBeNotified = new List<TimeSelection>();

        foreach (var ts in tss)
        {
            if (ts.Variacao != Variacao.OneToOne)
            {
                ts.ChangeStatus(StatusTimeSelection.Concluído);
            }
            else
            {
                ts.ChangeStatus(StatusTimeSelection.ConclusaoPendente);
                tssToBeNotified.Add(ts);
            }
        }
        await timeSelectionRepository.UpdateRange(tss);

        var tsIds = tssToBeNotified.Select(ts => ts.Id).ToList();
        var perfilGeradorHash = await joinTimeRepository.GetJoinTimePerfilIdsByTimeSelectionIds(
            tsIds
        );
        foreach (var ts in tssToBeNotified)
        {
            var perfilGerador = perfilGeradorHash[ts.Id];
            var notification = ts.BuildPendingNotification(perfilGerador);
            await _messagePublisher.PublishAsync("NotificationsQueue", notification);
        }
    }

    public async Task NotifyUpcomingTimeSelectionAndJoinTime()
    {
        var tsAndJts = await timeSelectionRepository.GetUpcomingTimeSelectionAndJoinTime();

        foreach (var tsAndJt in tsAndJts)
        {
            var ts = tsAndJt.Key;
            var jts = tsAndJt.Value;

            ts.MarkAsNotified();
            jts.ForEach(jt => jt.MarkAsNotified());

            var tsNotification = ts.BuildUpcomingNotification(jts[0].PerfilId);
            await _messagePublisher.PublishAsync("NotificationsQueue", tsNotification);

            foreach (var jt in jts)
            {
                var jtNotification = jt.BuildUpcomingNotification(ts.PerfilId);
                await _messagePublisher.PublishAsync("NotificationsQueue", jtNotification);
            }
        }
        var allJoinTimes = tsAndJts.Values.SelectMany(e => e).ToList();

        await timeSelectionRepository.UpdateRange([.. tsAndJts.Keys]);
        await joinTimeRepository.UpdateRange(allJoinTimes);
    }

    public async Task<BuildOpenGraphImage> BuildOpenGraphImage(Guid id)
    {
        var timeSelection = await timeSelectionRepository.GetById(id);
        var perfil =
            await perfilRepository.GetByIdAsync(timeSelection.PerfilId)
            ?? throw new KeyNotFoundException(
                "Perfil não encontrado para o id " + timeSelection.PerfilId
            );
        var tags = await tagRepository.GetAllByFreetimeIdAsync(timeSelection.Id.ToString());

        var build = new BuildOpenGraphImage(
            perfil.Id,
            perfil.Nome,
            perfil.UserName,
            string.Empty,
            timeSelection.TituloTemporario ?? string.Empty,
            tags,
            timeSelection.StartTime,
            timeSelection.EndTime,
            string.Empty
        );

        return build;
    }

    public async Task UpdateTimeSelectionImage(UpdateTimeSelectionPreviewRequest request)
    {
        var timeSelection = await timeSelectionRepository.GetById(request.TimeSelectionId);
        timeSelection.ChangePreviewImage(request.Image);
        await timeSelectionRepository.UpdateRange([timeSelection]);
    }
}
