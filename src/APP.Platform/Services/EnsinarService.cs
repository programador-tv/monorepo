using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.ViewModels;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models;

namespace Platform.Services;

public class EnsinarService : IEnsinarService
{
    private readonly ApplicationDbContext _context;

    private readonly PerfilDbContext _perfilContext;

    public EnsinarService(ApplicationDbContext context, PerfilDbContext perfilDbContext)
    {
        _context = context;
    }

    public void SetRoomProfileIdFromUserProfile(Room Room, Perfil UserProfile)
    {
        Room.PerfilId = UserProfile.Id;
    }

    public void RemoveRoomProfileIdFromModelState(ModelStateDictionary ModelState)
    {
        ModelState.Remove("Room.PerfilId");
    }

    public void CreateAndAddRoomToContext(Room Room)
    {
        Room.CodigoSala = GenerateNewRoomCode();
        Room.EstaAberto = true;
        Room.DataCriacao = DateTime.Now;
        Room.UltimaAtualizacao = DateTime.Now;
        Room.TipoSala = EnumTipoSalas.Livre;

        _context.Rooms?.Add(Room);
        _context.SaveChanges();
    }

    public string GenerateNewRoomCode()
    {
        return Guid.NewGuid().ToString().Replace("-", "")
            + "-"
            + Guid.NewGuid().ToString().Replace("-", "");
    }

    public void AddTagsToRoom(Room Room, List<string> TagsSelected)
    {
        foreach (var t in TagsSelected)
        {
            var tag = new Tag { Titulo = t, RoomRelacao = Room.CodigoSala };
            _context.Tags?.Add(tag);
        }
        _context.SaveChanges();
    }

    public List<TimeSelectionForCalendarSectionViewModel> CastTimeSelectionIntoCalendarSectionViewModel(
        List<TimeSelection> timeSeletions
    )
    {
        return timeSeletions
            .OrderBy(x => x.StartTime)
            .Select(ts => new TimeSelectionForCalendarSectionViewModel()
            {
                Id = ts.Id,
                Title = ts.TituloTemporario,
                Start = ts.StartTime,
                End = ts.EndTime,
                Status = ts.Status,
                Tipo = ts.Tipo,
                ActionNeeded = ts.ActionNeeded,
            })
            .ToList();
    }

    public List<BadFinishedTimeSelectionForCalendarSectionViewModel> CastTimeSelectionIntoBadFinishedCalendarSectionViewModel(
        List<TimeSelection> timeSeletions
    )
    {
        return timeSeletions
            .OrderBy(x => x.StartTime)
            .Select(ts => new BadFinishedTimeSelectionForCalendarSectionViewModel()
            {
                Id = ts.Id,
                Title = ts.TituloTemporario,
                Start = ts.StartTime,
                End = ts.EndTime,
            })
            .ToList();
    }

    public void GetTimeSelectionItem(
        Guid timeSelectionId,
        Perfil userProfile,
        string _meetUrl,
        Dictionary<TimeSelection, List<Perfil>> TimeSelectionsCheckedUsers,
        List<TimeSelection> oldTimeSelectionList,
        Dictionary<TimeSelection, List<JoinTimeViewModel>> timeSelectionDictionary
    )
    {
        var timeSelection =
            _context?.TimeSelections.Where(e => e.Id == timeSelectionId).FirstOrDefault()
            ?? throw new NullReferenceException("TimeSelection not found");

        HandleTimeSelection(
            timeSelection,
            userProfile,
            _meetUrl,
            TimeSelectionsCheckedUsers,
            oldTimeSelectionList,
            timeSelectionDictionary
        );
    }

    public void GetTimeSelectionList(
        Perfil userProfile,
        string _meetUrl,
        Dictionary<TimeSelection, List<Perfil>> TimeSelectionsCheckedUsers,
        List<TimeSelection> oldTimeSelectionList,
        Dictionary<TimeSelection, List<JoinTimeViewModel>> timeSelectionDictionary
    )
    {
        var timeSelectionList = GetTimeSelections(userProfile);

        timeSelectionList.ForEach(e =>
        {
            HandleTimeSelection(
                e,
                userProfile,
                _meetUrl,
                TimeSelectionsCheckedUsers,
                oldTimeSelectionList,
                timeSelectionDictionary
            );
        });
    }

    public List<TimeSelection> GetTimeSelections(Perfil userProfile)
    {
        var timeSelectionList = _context
            ?.TimeSelections.Where(e =>
                e.PerfilId == userProfile.Id && e.Status != StatusTimeSelection.Cancelado
            )
            .OrderBy(e => e.StartTime)
            .ToList();

        return timeSelectionList ?? new List<TimeSelection>();
    }

    public void HandleTimeSelection(
        TimeSelection e,
        Perfil userProfile,
        string _meetUrl,
        Dictionary<TimeSelection, List<Perfil>> TimeSelectionsCheckedUsers,
        List<TimeSelection> oldTimeSelectionList,
        Dictionary<TimeSelection, List<JoinTimeViewModel>> timeSelectionDictionary
    )
    {
        if (ShouldAddToOldList(e))
        {
            oldTimeSelectionList.Add(e);
            return;
        }
        UpdateTimeSelection(e, userProfile, _meetUrl);
        var viewmodels = CreateJoinTimeViewModels(e);
        SetActionNeeded(e);
        AddToSelectionList(e, viewmodels, TimeSelectionsCheckedUsers, timeSelectionDictionary);
    }

    public bool ShouldAddToOldList(TimeSelection e)
    {
        return e.EndTime < DateTime.Now
            && e.Status != StatusTimeSelection.Concluído
            && e.Status != StatusTimeSelection.ConclusaoPendente
            && e.Status != StatusTimeSelection.Marcado;
    }

    public void UpdateTimeSelection(TimeSelection e, Perfil userProfile, string _meetUrl)
    {
        var code = _context.Rooms.Where(r => r.Id == e.RoomId).FirstOrDefault()?.CodigoSala;

        e.LinkSala = _meetUrl + "?name=" + code + "&usr=" + userProfile.UserName;
        e.TempoRestante = Math.Floor(e.StartTime.Subtract(DateTime.Now).TotalHours).ToString();
        e.Tags = _context.Tags.Where(t => t.FreeTimeRelacao == e.Id.ToString()).ToList();

        if (e.Tipo == EnumTipoTimeSelection.RequestHelp)
        {
            var backstage = _context
                .HelpBackstages.Where(b => b.TimeSelectionId == e.Id)
                .FirstOrDefault();

            if (backstage != null)
            {
                e.Descricao = backstage.Descricao;
                e.RequestedHelpImagePath = backstage.ImagePath;
            }
        }
    }

    public List<JoinTimeViewModel> CreateJoinTimeViewModels(TimeSelection e)
    {
        var viewmodels = new List<JoinTimeViewModel>() { };
        var joins = _context
            ?.JoinTimes.Where(j =>
                j.TimeSelectionId == e.Id && j.StatusJoinTime != StatusJoinTime.Cancelado
            )
            .ToList();

        joins?.ForEach(j =>
        {
            var perfil = _perfilContext?.Perfils?.Where(p => p.Id == j.PerfilId).FirstOrDefault();
            if (perfil != null)
            {
                viewmodels.Add(CreateJoinTimeViewModel(perfil, j.Id, j.StatusJoinTime));
            }
        });

        return viewmodels;
    }

    public bool SetActionNeeded(TimeSelection e)
    {
        return (
                e.Status == StatusTimeSelection.Marcado
                || e.Status == StatusTimeSelection.ConclusaoPendente
            )
            && e.StartTime < DateTime.Now;
    }

    public void AddToSelectionList(
        TimeSelection e,
        List<JoinTimeViewModel> viewmodels,
        Dictionary<TimeSelection, List<Perfil>> TimeSelectionsCheckedUsers,
        Dictionary<TimeSelection, List<JoinTimeViewModel>> timeSelectionDictionary
    )
    {
        if (e.Status == StatusTimeSelection.ConclusaoPendente)
        {
            var perfil = _perfilContext?.Perfils?.Where(p => p.Id == e.PerfilId).FirstOrDefault();
            if (perfil != null)
            {
                if (TimeSelectionsCheckedUsers.TryGetValue(e, out var checkedUsers))
                {
                    checkedUsers.Add(perfil);
                }
                else
                {
                    TimeSelectionsCheckedUsers[e] = new List<Domain.Entities.Perfil> { perfil };
                }
            }
        }
        timeSelectionDictionary[e] = viewmodels;
    }

    public JoinTimeViewModel CreateJoinTimeViewModel(
        Domain.Entities.Perfil perfil,
        Guid joinTimeId,
        StatusJoinTime statusJoinTime
    )
    {
        return new JoinTimeViewModel
        {
            Perfil = perfil,
            JoinTimeId = joinTimeId,
            StatusJoinTime = statusJoinTime,
        };
    }

    public bool CheckStatusJoinTimeIsConclusaoPendente(StatusJoinTime status)
    {
        return status == StatusJoinTime.ConclusaoPendente;
    }

    public void CheckActionNeedAndUpdateTime(
        KeyValuePair<TimeSelection, List<JoinTimeViewModel>> timeSelectionAndJoinTimes
    )
    {
        if (
            (
                timeSelectionAndJoinTimes.Key.Status == StatusTimeSelection.Marcado
                || timeSelectionAndJoinTimes.Key.Status == StatusTimeSelection.ConclusaoPendente
            )
            && timeSelectionAndJoinTimes.Key.StartTime < DateTime.Now
        )
        {
            timeSelectionAndJoinTimes.Key.ActionNeeded = true;
        }

        var start = timeSelectionAndJoinTimes.Key.StartTime;
        timeSelectionAndJoinTimes.Key.TempoRestante = GetFreeTimeService.GetTempoRestante(start);
    }
}
