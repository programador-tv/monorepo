using Domain.Entities;
using Domain.Enums;
using Domain.Models.ViewModels;
using Domain.WebServices;
using Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Platform.Services;

public class LiveService(ApplicationDbContext context, IPerfilWebService perfilWs) : ILiveService
{
    public LiveViewModel BuildLiveViewModels(Live live, Perfil perfil, int views)
    {
        return new LiveViewModel()
        {
            CodigoLive = live.Id.ToString(),
            Titulo = live.Titulo,
            Descricao = live.Descricao,
            DataCriacao = live.DataCriacao,
            Thumbnail = live.Thumbnail,
            NomeCriador = perfil.Nome ?? "Anônimo",
            UserNameCriador = perfil.UserName ?? string.Empty,
            FotoCriador = perfil.Foto ?? string.Empty,
            LiveEstaAberta = live.LiveEstaAberta,
            StatusLive = live.StatusLive,
            FormatedDuration = live.FormatedDuration,
            QuantidadeDeVisualizacoes = views,
            UrlAlias = live.UrlAlias,
        };
    }

    public List<PrivateLiveViewModel> RenderPrivateLives(Perfil perfilOwner, Guid perfilLogInId)
    {
        var lives = context.Lives.Where(e => e.PerfilId == perfilOwner.Id).ToList();

        var privateLives = new List<PrivateLiveViewModel>();
        var isUsrCanal = perfilLogInId == perfilOwner.Id;

        var liveIds = lives.Select(e => e.Id);
        var liveVisualizations = context
            .Visualizations.Where(e => liveIds.Contains(e.LiveId))
            .ToList();

        for (int i = lives.Count - 1; i >= 0; i--)
        {
            var live = lives[i];
            var vizualizations = liveVisualizations.Where(i => i.LiveId == live.Id).Count();

            privateLives.Add(
                new PrivateLiveViewModel
                {
                    CodigoLive = live.Id.ToString(),
                    Titulo = live.Titulo,
                    Descricao = live.Descricao,
                    Thumbnail = live.Thumbnail,
                    Visibility = live.Visibility,
                    NomeCriador = perfilOwner?.Nome ?? "Anônimo",
                    UserNameCriador = perfilOwner?.UserName ?? string.Empty,
                    FotoCriador = perfilOwner?.Foto,
                    QuantidadeDeVisualizacoes = vizualizations,
                    FormatedDuration = live.FormatedDuration,
                    StatusLive = live.StatusLive,
                    IsUsrCanal = isUsrCanal,
                    DataCriacao = live.DataCriacao,
                    UrlAlias = live.UrlAlias,
                }
            );
        }
        return privateLives;
    }

    public List<LiveSchedulePreviewViewModel> RenderPreviewLiveSchedule(
        Perfil perfilOwner,
        Guid perfilLogInId
    )
    {
        var isUsrCanal = perfilLogInId == perfilOwner.Id;
        var lives = context.Lives.AsNoTracking().Where(e => e.PerfilId == perfilOwner.Id).ToList();

        if (lives.Count == 0)
        {
            return new();
        }

        var liveIds = lives.Select(e => e.Id).ToList();
        var liveBackstages = context
            .LiveBackstages.AsNoTracking()
            .Where(e => liveIds.Contains(e.LiveId))
            .ToList();

        if (liveBackstages.Count == 0)
        {
            return new();
        }

        var liveBackstageTimeSelectionIds = liveBackstages.Select(e => e.TimeSelectionId).ToList();

        var timeSelectionTypeLive = context
            .TimeSelections.AsNoTracking()
            .Where(e => liveBackstageTimeSelectionIds.Contains(e.Id))
            .ToList();

        timeSelectionTypeLive = timeSelectionTypeLive
            .Where(e => e.StartTime > DateTime.Now && e.Status != StatusTimeSelection.Cancelado)
            .ToList();

        if (timeSelectionTypeLive.Count == 0)
        {
            return new();
        }

        var timeSelectionTypeLiveIds = timeSelectionTypeLive.Select(e => e.Id).ToList();
        var liveScheduleIds = liveBackstages
            .Where(e => timeSelectionTypeLiveIds.Contains(e.TimeSelectionId))
            .Select(e => e.LiveId)
            .ToList();
        var liveSchedules = lives.Where(e => liveScheduleIds.Contains(e.Id)).ToList();
        var liveSchedulePreviews = new List<LiveSchedulePreviewViewModel>();

        for (int i = 0; i < liveSchedules.Count; i++)
        {
            var live = liveSchedules[i];

            liveSchedulePreviews.Add(
                new()
                {
                    CodigoLive = live.Id.ToString(),
                    Titulo = live.Titulo,
                    Descricao = live.Descricao,
                    Thumbnail = live.Thumbnail,
                    Visibility = live.Visibility,
                    NomeCriador = perfilOwner?.Nome ?? "Anônimo",
                    UserNameCriador = perfilOwner?.UserName ?? string.Empty,
                    FotoCriador = perfilOwner?.Foto,
                    FormatedDuration = "Em breve",
                    StatusLive = live.StatusLive,
                    IsUsrCanal = isUsrCanal,
                    DataCriacao = live.DataCriacao,
                    IsTimeSelection = true,
                    UrlAlias = live.UrlAlias,
                }
            );
        }

        return liveSchedulePreviews;
    }
}
