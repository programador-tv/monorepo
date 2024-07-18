using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.ViewModels;
using Infrastructure.Data.Contexts;

namespace Platform.Services;

public class AprenderService : IAprenderService
{
    public readonly ApplicationDbContext _context;
    public readonly PerfilDbContext _perfilContext;

    public AprenderService(ApplicationDbContext context, PerfilDbContext perfilDbContext)
    {
        _context = context;
    }

    public List<BadFinishedTimeSelectionForCalendarSectionViewModel> CastTimeSelectionIntoBadFinishedCalendarSectionViewModel(
        List<TimeSelection> timeSeletions
    )
    {
        throw new NotImplementedException();
    }

    public List<TimeSelectionForCalendarSectionViewModel> CastTimeSelectionIntoCalendarSectionViewModel(
        List<TimeSelection> timeSeletions
    )
    {
        throw new NotImplementedException();
    }

    public void GetMyEvents(
        Dictionary<TimeSelection, List<JoinTime>> associatedTimeSelectionAndJoinTimes,
        Guid userId,
        string _meetUrl,
        Dictionary<JoinTime, TimeSelection>? MyEvents,
        Dictionary<JoinTime, TimeSelection> OldMyEvents,
        string? userNome
    )
    {
        foreach (var tsAndJts in associatedTimeSelectionAndJoinTimes)
        {
            SetEventsWithData(
                tsAndJts.Key,
                tsAndJts.Value.First(ts => ts.PerfilId == userId),
                _meetUrl,
                MyEvents,
                OldMyEvents,
                userNome
            );
        }
    }

    public Dictionary<TimeSelection, List<JoinTime>> GetMyTimeSelectionAndJoinTimes(
        Guid userId,
        string _meetUrl
    )
    {
        var myEvents = new Dictionary<TimeSelection, List<JoinTime>>();

        var myJoinTimes = _context
            .JoinTimes.Where(jt =>
                jt.PerfilId == userId
                && jt.StatusJoinTime != StatusJoinTime.Cancelado
                && jt.StatusJoinTime != StatusJoinTime.Rejeitado
            )
            .ToList();

        var timeSelectionIds = myJoinTimes.Select(jt => jt.TimeSelectionId).ToList();

        var timeSelections = _context
            .TimeSelections.Where(ts => timeSelectionIds.Contains(ts.Id))
            .ToList();

        var JoinTimeBrothers = _context
            .JoinTimes.Where(jt =>
                timeSelectionIds.Contains(jt.TimeSelectionId)
                && jt.PerfilId != userId
                && jt.StatusJoinTime != StatusJoinTime.Cancelado
                && jt.StatusJoinTime != StatusJoinTime.Rejeitado
            )
            .ToList();

        foreach (var ts in timeSelections)
        {
            var joins = new List<JoinTime>();
            var brotherTs = JoinTimeBrothers.Where(jt => jt.TimeSelectionId == ts.Id).ToList();

            joins.AddRange(brotherTs);
            joins.AddRange(myJoinTimes.Where(jt => jt.TimeSelectionId == ts.Id));

            myEvents.Add(ts, joins);
        }

        return myEvents;
    }

    public bool SetActionNeeded(TimeSelection e)
    {
        throw new NotImplementedException();
    }

    public bool ShouldAddToOldList(TimeSelection e)
    {
        throw new NotImplementedException();
    }

    private void SetEventsWithData(
        TimeSelection associatedTimeSelection,
        JoinTime item,
        string _meetUrl,
        Dictionary<JoinTime, TimeSelection>? myEvents,
        Dictionary<JoinTime, TimeSelection> oldMyEvents,
        string? userNome
    )
    {
        if (associatedTimeSelection.Status == StatusTimeSelection.Cancelado)
        {
            return;
        }
        if (
            associatedTimeSelection.EndTime < DateTime.Now
            && associatedTimeSelection.Status != StatusTimeSelection.Concluído
            && associatedTimeSelection.Status != StatusTimeSelection.Marcado
            && associatedTimeSelection.Status != StatusTimeSelection.ConclusaoPendente
        )
        {
            oldMyEvents[item] = associatedTimeSelection;
            return;
        }

        var code = _context
            ?.Rooms?.Where(r => r.Id == associatedTimeSelection.RoomId)
            .FirstOrDefault()
            ?.CodigoSala;

        if (code != null)
        {
            associatedTimeSelection.LinkSala = _meetUrl + "?name=" + code + "&usr=" + userNome;
        }

        var tags = _context
            ?.Tags?.Where(t => t.FreeTimeRelacao == associatedTimeSelection.Id.ToString())
            .ToList();

        if (tags != null)
        {
            associatedTimeSelection.Tags = tags;
        }

        var perfil = _perfilContext
            ?.Perfils?.Where(p => p.Id == associatedTimeSelection.PerfilId)
            .FirstOrDefault();

        if (perfil != null)
        {
            associatedTimeSelection.Perfil = perfil;
        }

        associatedTimeSelection.TempoRestante = Math.Floor(
                associatedTimeSelection.StartTime.Subtract(DateTime.Now).TotalHours
            )
            .ToString();

        if (
            (
                item.StatusJoinTime == StatusJoinTime.Marcado
                || item.StatusJoinTime == StatusJoinTime.ConclusaoPendente
            )
            && associatedTimeSelection.StartTime < DateTime.Now
        )
        {
            associatedTimeSelection.ActionNeeded = true;
        }

        myEvents ??= new();

        myEvents[item] = associatedTimeSelection;
    }
}
