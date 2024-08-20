using System.Linq;
using System.Text;
using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using Domain.Models.ViewModels;
using Domain.WebServices;
using Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Platform.Services;

public static class GetFreeTimeService
{
    public static List<string> ObtemTagsRelacionadas(
        string[] ForFindTags,
        ApplicationDbContext _context
    )
    {
        return _context
                ?.Tags?.Where(e => e.FreeTimeRelacao != null && ForFindTags.Contains(e.Titulo))
                .Select(e => e.FreeTimeRelacao ?? string.Empty)
                .ToList() ?? new List<string> { };
    }

    public static async Task<List<MentorFreeTime>> ObtemPerfisRelacionados(
        IEnumerable<
            IGrouping<string?, TimeSelectionForMentorFreeTimeViewModel>
        > timeSelectionGroupByPerfilId,
        ApplicationDbContext _context,
        IPerfilWebService _perfilWebService
    )
    {
        var mentorsFreeTime = new List<MentorFreeTime>();
        List<string?> timeSelectionIds = new();
        List<Tag>? tags;

        var perfilIds = timeSelectionGroupByPerfilId
            .Select(item => item.Key)
            .Where(id => id != null && Guid.TryParse(id, out _))
            .Select(id => Guid.Parse(id))
            .ToList();

        var perfis = await _perfilWebService.GetAllById(perfilIds) ?? new();

        var perfisLegacy = new List<Domain.Entities.Perfil>();

        foreach (var perfil in perfis)
        {
            var perfilLegacy = new Domain.Entities.Perfil
            {
                Id = perfil.Id,
                Nome = perfil.Nome,
                Foto = perfil.Foto,
                Token = perfil.Token,
                UserName = perfil.UserName,
                Linkedin = perfil.Linkedin,
                GitHub = perfil.GitHub,
                Bio = perfil.Bio,
                Email = perfil.Email,
                Descricao = perfil.Descricao,
                Experiencia = (Domain.Entities.ExperienceLevel)perfil.Experiencia
            };
            perfisLegacy.Add(perfilLegacy);
        }

        foreach (var PerfilTimeSelection in timeSelectionGroupByPerfilId)
        {
            var key = PerfilTimeSelection.Key ?? Guid.Empty.ToString();
            if (key != Guid.Empty.ToString())
            {
                var mentor = perfisLegacy.First(p => p.Id.ToString() == key);
                if (mentor == null)
                {
                    continue;
                }
                var mentorFreeTime = new MentorFreeTime
                {
                    TimeSelections = PerfilTimeSelection
                        .Select(e => new TimeSelectionForMentorFreeTimeViewModel()
                        {
                            TimeSelectionId = e.TimeSelectionId,
                            PerfilId = e.PerfilId.ToString(),
                            StartTime = e.StartTime,
                            EndTime = e.EndTime,
                            Titulo = e.Titulo,
                            Variacao = e.Variacao
                        })
                        .ToList(),
                    Perfils = mentor
                };
                timeSelectionIds.AddRange(
                    mentorFreeTime.TimeSelections.Select(s => s.TimeSelectionId).ToList()
                );

                mentorsFreeTime.Add(mentorFreeTime);
            }
        }

        tags = _context.Tags.Where(t => timeSelectionIds.Contains(t.FreeTimeRelacao)).ToList();

        var timeSelectionsDictionary = mentorsFreeTime
            .SelectMany(mft => mft.TimeSelections)
            .ToDictionary(ts => ts.TimeSelectionId);

        foreach (var ts in timeSelectionsDictionary.Values)
        {
            ts.Tags = tags.Where(t => t.FreeTimeRelacao == ts.TimeSelectionId).ToList();
        }

        var freeTimeBackstages = _context
            .FreeTimeBackstages.Where(e => timeSelectionIds.Contains(e.TimeSelectionId.ToString()))
            .ToList();

        var confirmedJoinTimes = _context
            .JoinTimes.Where(e => timeSelectionIds.Contains(e.TimeSelectionId.ToString()))
            .ToList();

        SetParticipantesAndInteressados(mentorsFreeTime, freeTimeBackstages, confirmedJoinTimes);

        return mentorsFreeTime;
    }

    public static async Task<
        List<RequestedHelpViewModel>
    > PrepareViewModelForRenderRequestedHelpBoard(
        IEnumerable<
            IGrouping<string?, TimeSelectionForRequestedHelpViewModel>
        > timeSelectionGroupByPerfilId,
        ApplicationDbContext _context,
        IHttpClientFactory httpClientFactory,
        IPerfilWebService _perfilWebService,
        IHelpResponseWebService _helpResponseWebService
    )
    {
        var RequestedHelp = new List<RequestedHelpViewModel>();
        List<string?> timeSelectionIds = [];
        List<Tag>? tags;

        var perfilIds = timeSelectionGroupByPerfilId
            .Select(item => item.Key)
            .Where(id => id != null && Guid.TryParse(id, out _))
            .Select(id => Guid.Parse(id))
            .ToList();

        var perfis = await _perfilWebService.GetAllById(perfilIds) ?? [];

        var perfisLegacy = new List<Domain.Entities.Perfil>();

        foreach (var perfil in perfis)
        {
            var perfilLegacy = new Domain.Entities.Perfil
            {
                Id = perfil.Id,
                Nome = perfil.Nome,
                Foto = perfil.Foto,
                Token = perfil.Token,
                UserName = perfil.UserName,
                Linkedin = perfil.Linkedin,
                GitHub = perfil.GitHub,
                Bio = perfil.Bio,
                Email = perfil.Email,
                Descricao = perfil.Descricao,
                Experiencia = (Domain.Entities.ExperienceLevel)perfil.Experiencia
            };
            perfisLegacy.Add(perfilLegacy);
        }

        foreach (var PerfilTimeSelection in timeSelectionGroupByPerfilId)
        {
            var key = PerfilTimeSelection.Key ?? Guid.Empty.ToString();
            if (key != Guid.Empty.ToString())
            {
                var requesterPerfils = perfisLegacy.First(p => p.Id.ToString() == key);
                if (requesterPerfils == null)
                {
                    continue;
                }
                var requesteds = new List<TimeSelectionForRequestedHelpViewModel>();
                foreach (var item in PerfilTimeSelection)
                {
                    var helpResponses =
                        await _helpResponseWebService.GetAll(Guid.Parse(item.TimeSelectionId))
                        ?? [];
                    requesteds.Add(
                        new TimeSelectionForRequestedHelpViewModel()
                        {
                            TimeSelectionId = item.TimeSelectionId,
                            PerfilId = item.PerfilId.ToString(),
                            StartTime = item.StartTime,
                            EndTime = item.EndTime,
                            Description = item.Description,
                            Variation = item.Variation,
                            Title = item.Title,
                            HelpResponses = helpResponses
                        }
                    );
                }
                var requestedHelp = new RequestedHelpViewModel
                {
                    TimeSelections = requesteds,
                    Perfils = requesterPerfils
                };
                timeSelectionIds.AddRange(
                    requestedHelp.TimeSelections.Select(s => s.TimeSelectionId).ToList()
                );

                RequestedHelp.Add(requestedHelp);
            }
        }

        tags = [.. _context.Tags.Where(t => timeSelectionIds.Contains(t.FreeTimeRelacao))];

        var client = httpClientFactory.CreateClient("CoreAPI");

        var json = JsonSerializer.Serialize(timeSelectionIds);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var responseHelpTask = await client.PostAsync($"api/helpbackstage/AllByIds", content);

        var helpBackstages =
            await responseHelpTask.Content.ReadFromJsonAsync<List<HelpBackstage>>() ?? [];

        var timeSelectionsDictionary = RequestedHelp
            .SelectMany(mft => mft.TimeSelections)
            .ToDictionary(ts => ts.TimeSelectionId);

        foreach (var ts in timeSelectionsDictionary.Values)
        {
            ts.Tags = tags.Where(t => t.FreeTimeRelacao == ts.TimeSelectionId).ToList();

            var backstage =
                helpBackstages.Find(t => t.TimeSelectionId.ToString() == ts.TimeSelectionId)
                ?? new();
            ts.Description = backstage.Descricao;
            ts.ImagePath = backstage.ImagePath;
        }

        return RequestedHelp;
    }

    public static List<TimeSelection> ObtemTimeSelectionsByTagsExcludingSet(
        List<string> tagRelations,
        ApplicationDbContext _context,
        HashSet<TimeSelection> valueSet
    )
    {
        return _context
                ?.TimeSelections.Where(e =>
                    tagRelations.Contains(e.Id.ToString())
                    && e.Status == StatusTimeSelection.Pendente
                    && e.Tipo == EnumTipoTimeSelection.FreeTime
                    && !valueSet.Contains(e)
                )
                .OrderBy((e => e.StartTime))
                .ToList() ?? new List<TimeSelection> { };
    }

    public static List<TimeSelection> ObtemTimeSelectionsSet(
        ApplicationDbContext _context,
        HashSet<TimeSelection> valueSet,
        EnumTipoTimeSelection timeSelectionType
    )
    {
        return _context
                ?.TimeSelections.Where(e =>
                    e.Status == StatusTimeSelection.Pendente
                    && e.Tipo == timeSelectionType
                    && !valueSet.Contains(e)
                )
                .OrderBy(e => e.StartTime)
                .ToList() ?? new List<TimeSelection> { };
    }

    public static List<TimeSelectionForMentorFreeTimeViewModel> ObtemTimeSelectionSetForHome(
        ApplicationDbContext _context,
        HashSet<TimeSelection> valueSet
    )
    {
        var obtemMentorias = _context
            .TimeSelections.AsNoTracking()
            .Where(e =>
                e.Status == StatusTimeSelection.Pendente
                && e.Tipo == EnumTipoTimeSelection.FreeTime
                && !valueSet.Contains(e)
            )
            .OrderBy(e => e.StartTime)
            .ToList();

        var obtemMentoriasProximas = obtemMentorias
            .Where(e => e.StartTime > DateTime.Now)
            .Select(e => new TimeSelectionForMentorFreeTimeViewModel()
            {
                TimeSelectionId = e.Id.ToString(),
                PerfilId = e.PerfilId.ToString() ?? Guid.Empty.ToString(),
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Titulo = e.TituloTemporario
            })
            .ToList();

        return obtemMentoriasProximas
            .GroupBy(e => e.PerfilId)
            .SelectMany(group => group.Take(1))
            .Take(3)
            .ToList();
    }

    public static List<TimeSelectionForRequestedHelpViewModel> ObtemTimeSelectionSetForHomeRequestedHelp(
        ApplicationDbContext _context,
        HashSet<TimeSelection> valueSet
    )
    {
        var obtemPedidos = _context
            .TimeSelections.Where(e =>
                e.Status == StatusTimeSelection.Pendente
                && e.Tipo == EnumTipoTimeSelection.RequestHelp
                && !valueSet.Contains(e)
            )
            .OrderBy(e => e.StartTime)
            .ToList();

        var timeSelectionForRequestedHelp = obtemPedidos
            .Where(e => e.StartTime > DateTime.Now)
            .Select(e => new TimeSelectionForRequestedHelpViewModel()
            {
                TimeSelectionId = e.Id.ToString(),
                PerfilId = e.PerfilId.ToString() ?? Guid.Empty.ToString(),
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Variation = (int)e.Variacao,
                Title = e.TituloTemporario
            })
            .ToList();

        return timeSelectionForRequestedHelp
            .GroupBy(e => e.PerfilId)
            .SelectMany(group => group.Take(1))
            .Take(3)
            .ToList();
    }

    public static List<TimeSelection> ObtemTimeSelectionsByPerfilIdExcludingSet(
        Guid perfilId,
        HashSet<TimeSelection> valueSet,
        ApplicationDbContext _context
    )
    {
        return _context
                ?.TimeSelections.Where(e =>
                    e.PerfilId == perfilId
                    && e.Status == StatusTimeSelection.Pendente
                    && e.Tipo == EnumTipoTimeSelection.FreeTime
                    && !valueSet.Contains(e)
                )
                .OrderBy((e => e.StartTime))
                .ToList() ?? new List<TimeSelection> { };
    }

    public static List<TimeSelection> FiltraPelosNaoConflitantes(
        List<TimeSelection> timeSelections,
        ApplicationDbContext context,
        Dictionary<JoinTime, TimeSelection> MyEvents
    )
    {
        var timeSelectionsFinal = new List<TimeSelection>();
        timeSelections.ForEach(item =>
        {
            if (
                item.StartTime >= DateTime.Now
                && !MyEvents.Values.Any(e =>
                    e.StartTime >= item.StartTime && e.StartTime <= item.EndTime
                    || e.EndTime >= item.StartTime && e.EndTime <= item.EndTime
                    || e.StartTime <= item.StartTime && e.EndTime >= item.EndTime
                )
            )
            {
                timeSelectionsFinal.Add(item);
            }
        });
        return timeSelectionsFinal;
    }

    public static string GetTempoRestante(DateTime time)
    {
        DateTime actualTime = DateTime.Now;
        if (time > DateTime.Now)
        {
            TimeSpan diferencial = time - actualTime;

            return diferencial.ToString(@"hh\:mm");
        }

        return "";
    }

    public static void SetParticipantesAndInteressados(
        List<MentorFreeTime> mentorFreeTimes,
        List<FreeTimeBackstage> freeTimeBackstages,
        List<JoinTime> joinTimes
    )
    {
        var timeSelections = mentorFreeTimes
            .Where(e => e.TimeSelections != null)
            .SelectMany(e => e.TimeSelections ?? new())
            .ToList();

        for (int j = 0; j < timeSelections.Count; j++)
        {
            var tsFreeTime = timeSelections[j];

            var freeTimeBackstage =
                freeTimeBackstages.Find(e =>
                    e.TimeSelectionId.ToString() == tsFreeTime.TimeSelectionId
                ) ?? new();

            tsFreeTime.CountInteressados = joinTimes.Count(e =>
                e.TimeSelectionId.ToString() == tsFreeTime.TimeSelectionId
            );

            if (freeTimeBackstage.MaxParticipants == 0)
            {
                continue;
            }

            tsFreeTime.MaxParticipantes = freeTimeBackstage.MaxParticipants;
            tsFreeTime.CountInteressadosAceitos = joinTimes.Count(e =>
                e.TimeSelectionId.ToString() == tsFreeTime.TimeSelectionId
                && e.StatusJoinTime == StatusJoinTime.Marcado
            );
        }
    }

    public static List<TimeSelectionForRequestedHelpViewModel> CreateRequestedHelpViewModelList(
        List<TimeSelection> timeSelections
    )
    {
        return timeSelections
            .Select(e => new TimeSelectionForRequestedHelpViewModel()
            {
                TimeSelectionId = e.Id.ToString(),
                PerfilId = e.PerfilId.ToString() ?? Guid.Empty.ToString(),
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Variation = (int)e.Variacao,
                Title = e.TituloTemporario
            })
            .ToList();
    }
}
