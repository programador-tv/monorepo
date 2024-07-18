using System.Linq;
using System.Text;
using System.Text.Json;
using APP.Platform.Services;
using Background;
using Domain.Entities;
using Domain.Enums;
using Domain.Models.ViewModels;
using Domain.RequestModels;
using Domain.WebServices;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Platform.Services;
using Queue;
using tags;

namespace APP.Platform.Pages;

public class IndexModel : CustomPageModel
{
    public List<PresentesOpenRoom>? PresentesOpenRoom { get; set; }

    private readonly OpenAiService _openAiService;
    private new readonly ApplicationDbContext _context;
    protected readonly IHttpClientFactory _httpClientFactory;
    protected new readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IAprenderService _AprenderService;

    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly ILiveService _liveService;
    private IPerfilWebService _perfilWebService { get; set; }
    public List<RoomViewModel> Rooms = new();
    public Dictionary<JoinTime, TimeSelection>? MyEvents { get; set; } = new();
    public Dictionary<JoinTime, TimeSelection> OldMyEvents { get; set; } = new();
    public Dictionary<string, List<string>> RelatioTags { get; set; }

    [BindProperty]
    public ScheduleTimeSelectionRequestModel? ScheduleTimeSelection { get; set; }

    [BindProperty]
    public ScheduleLiveForTimeSelection? ScheduleLiveForTimeSelection { get; set; }

    [BindProperty]
    public ScheduleFreeTimeForTimeSelectionRequestModel? ScheduleFreeTimeForTimeSelection { get; set; }

    [BindProperty]
    public JoinTime? JoinTime { get; set; }

    [BindProperty]
    public string? Joined { get; set; }

    [BindProperty]
    public List<string> TagsSelected { get; set; } = new();

    public IndexModel(
        ILiveService liveService,
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        OpenAiService openAiService,
        IMessagePublisher messagePublisher,
        IAprenderService aprenderService,
        IPerfilWebService perfilWebService,
        Settings settings
    )
        : base(context, httpClientFactory, httpContextAccessor, settings)
    {
        _httpClientFactory = httpClientFactory;
        _liveService = liveService;
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _perfilWebService = perfilWebService;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _messagePublisher = messagePublisher;
        _openAiService = openAiService;
        _AprenderService = aprenderService;
    }

    public IActionResult OnPostJoinOpenRoom()
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("/Perfil");
        }
        else if (!IsAuth)
        {
            return Redirect("/Identity/Account/Login");
        }

        return Redirect(_meetUrl + "?name=openroom-openroom&usr=" + UserProfile.UserName);
    }

    public IActionResult OnPostJoinRoom()
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("/Perfil");
        }
        else if (!IsAuth)
        {
            return Redirect("/Identity/Account/Login");
        }

        var room = _context?.Rooms?.Where(e => e.CodigoSala == Joined).FirstOrDefault();

        if (room != null)
        {
            return Redirect(_meetUrl + "?name=" + room.CodigoSala + "&usr=" + UserProfile.UserName);
        }
        return Page();
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("/Perfil");
        }

        PresentesOpenRoom = _context.PresentesOpenRooms.Where(e => e.EstaPresente).ToList();

        var rooms = _context.Rooms.Where(e => e.EstaAberto).ToList();

        var roomsCodigoSalaList = rooms.Select(e => e.CodigoSala).ToList();
        var roomsId = rooms.Select(e => e.Id).ToList();

        var associatedTags = _context
            .Tags.Where(e => roomsCodigoSalaList.Contains(e.RoomRelacao))
            .ToList();

        var associatedOwnerId = rooms.Select(e => e.PerfilId).ToList();

        var associatedOwners = await _perfilWebService.GetAllById(associatedOwnerId) ?? new();

        var associatedPresentes = _context
            .Presentes.Where(e => roomsId.Contains(e.RoomId) && e.EstaPresente)
            .ToList();

        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            var tags = associatedTags
                .Where(e => e.RoomRelacao == rooms[i].CodigoSala)
                .Select(e => e.Titulo ?? string.Empty)
                .ToList();

            var owner = associatedOwners.Find(e => e.Id == rooms[i].PerfilId);
            var presentes = associatedPresentes.Where(e => e.RoomId == rooms[i].Id).ToList();

            var roomViewModel = new RoomViewModel
            {
                CodigoSala = rooms[i].CodigoSala,
                EstaAberto = rooms[i].EstaAberto,
                NomeCriador = owner?.Nome,
                FotoCriador = owner?.Foto,
                DataCriacao = rooms[i].DataCriacao,
                Tags = tags,
                Titulo = rooms[i].Titulo ?? string.Empty,
                TipoSala = rooms[i].TipoSala,
                Presentes = presentes,
            };
            Rooms.Add(roomViewModel);
        }

        RelatioTags = DataTags.GetTags();

        return Page();
    }

    public async Task<ActionResult> OnGetAfterLoadLives()
    {
        var visibleVideos =
            await _context
                .Lives.Where(e => e.LiveEstaAberta)
                .AsNoTracking()
                .OrderByDescending(e => e.DataCriacao)
                .ToListAsync() ?? [];

        var PerfilIds = visibleVideos.Select(e => e.PerfilId).ToList();
        var liveIds = visibleVideos.Select(e => e.Id).ToList();

        var perfils = await _perfilWebService.GetAllById(PerfilIds);

        var liveVisualizations = _context
            .Visualizations.Where(e => liveIds.Contains(e.LiveId))
            .ToList();

        var lives = new List<LiveViewModel> { };

        foreach (var live in visibleVideos)
        {
            var perfil = perfils.Find(e => e.Id == live.PerfilId);

            if (perfil == null)
            {
                continue;
            }
            var oldPerfil = new Domain.Entities.Perfil
            {
                Nome = perfil.Nome,
                UserName = perfil.UserName,
                Foto = perfil.Foto,
            };
            var views = liveVisualizations.Count(e => e.LiveId == live.Id);

            var liveViewModel = _liveService.BuildLiveViewModels(live, oldPerfil, views);

            lives.Add(liveViewModel);
        }
        var livesHtml = await RenderViewAsync("Components/_VideosGroup", lives);

        return new JsonResult(new { Lives = livesHtml });
    }

    public async Task<ActionResult> OnGetAfterLoadSavedVideos()
    {
        var visibleVideos =
            await _context
                .Lives.Where(e => !e.LiveEstaAberta && e.Visibility)
                .AsNoTracking()
                .OrderByDescending(e => e.DataCriacao)
                .Take(20)
                .ToListAsync() ?? [];

        var PerfilIds = visibleVideos.Select(e => e.PerfilId).ToList();
        var liveIds = visibleVideos.Select(e => e.Id).ToList();

        var perfils = await _perfilWebService.GetAllById(PerfilIds);

        var liveVisualizations = _context
            .Visualizations.Where(e => liveIds.Contains(e.LiveId))
            .ToList();

        var savedVideos = new List<LiveViewModel> { };

        foreach (var live in visibleVideos)
        {
            var perfil = perfils.Find(e => e.Id == live.PerfilId);

            if (perfil == null)
            {
                continue;
            }

            var oldPerfil = new Domain.Entities.Perfil
            {
                Nome = perfil.Nome,
                UserName = perfil.UserName,
                Foto = perfil.Foto,
            };
            var views = liveVisualizations.Count(e => e.LiveId == live.Id);

            var liveViewModel = _liveService.BuildLiveViewModels(live, oldPerfil, views);

            savedVideos.Add(liveViewModel);
        }

        var savedVideosHtml = await RenderViewAsync("Components/_VideosGroup", savedVideos);

        return new JsonResult(new { Saved = savedVideosHtml });
    }

    public async Task<ActionResult> OnGetAfterLoadLivePreview()
    {
        var scheduledLiveOnFuture = _context
            .TimeSelections.Where(slof =>
                slof.Tipo == EnumTipoTimeSelection.Live
                && slof.Status != StatusTimeSelection.Cancelado
                && slof.StartTime > DateTime.Now
            )
            .AsNoTracking()
            .ToList();

        var timeSelectionsId = scheduledLiveOnFuture.Select(ts => ts.Id).ToList();

        var backstages = _context
            .LiveBackstages.Where(bs => timeSelectionsId.Contains(bs.TimeSelectionId))
            .AsNoTracking()
            .ToList();

        var livesId = backstages.Select(bs => bs.LiveId).ToList();

        var livesSchedules = _context
            .Lives.Where(l => livesId.Contains(l.Id))
            .OrderBy(o => o.DataCriacao)
            .AsNoTracking()
            .ToList();

        var PerfilIds = livesSchedules.Select(e => e.PerfilId).ToList();
        var liveIds = livesSchedules.Select(e => e.Id).ToList();

        var perfils = await _perfilWebService.GetAllById(PerfilIds);

        var liveVisualizations = _context
            .Visualizations.Where(e => liveIds.Contains(e.LiveId))
            .ToList();

        var savedVideos = new List<LiveViewModel> { };

        foreach (var live in livesSchedules)
        {
            var perfil = perfils.Find(e => e.Id == live.PerfilId);

            if (perfil == null)
            {
                continue;
            }
            var oldPerfil = new Domain.Entities.Perfil
            {
                Nome = perfil.Nome,
                UserName = perfil.UserName,
                Foto = perfil.Foto,
            };
            var views = liveVisualizations.Count(e => e.LiveId == live.Id);

            var liveViewModel = _liveService.BuildLiveViewModels(live, oldPerfil, views);
            var liveId = liveViewModel.CodigoLive;

            var timeSelectionId = backstages
                .First(e => e.LiveId.ToString() == liveId)
                .TimeSelectionId;

            var timeSelection = scheduledLiveOnFuture.Find(e => e.Id == timeSelectionId);

            if (timeSelection == null)
            {
                continue;
            }

            liveViewModel.DataCriacao = timeSelection.StartTime;

            savedVideos.Add(liveViewModel);
        }

        var savedVideosHtml = await RenderViewAsync("Components/_LivesPreviewGroup", savedVideos);

        return new JsonResult(new { Preview = savedVideosHtml });
    }

    public async Task<ActionResult> OnGetAfterLoadMentores()
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("../Perfil");
        }

        var myTimeSelectionAndJoinTimes = _AprenderService.GetMyTimeSelectionAndJoinTimes(
            UserProfile.Id,
            _meetUrl
        );

        _AprenderService.GetMyEvents(
            myTimeSelectionAndJoinTimes,
            UserProfile.Id,
            _meetUrl,
            MyEvents,
            OldMyEvents,
            UserProfile.Nome
        );

        HashSet<TimeSelection> valueSet = new();

        if (MyEvents != null)
        {
            valueSet = new HashSet<TimeSelection>(MyEvents.Values);
        }

        var timeSelections = GetFreeTimeService.ObtemTimeSelectionSetForHome(_context, valueSet);

        var timeSelectionGroupByPerfilId = timeSelections.GroupBy(e => e.PerfilId);

        var MentorsFreeTime = await GetFreeTimeService.ObtemPerfisRelacionados(
            timeSelectionGroupByPerfilId,
            _context,
            _perfilWebService
        );
        bool isLoggedUsr = true;
        if (!IsAuth)
        {
            isLoggedUsr = false;
        }
        return new JsonResult(new { mentores = MentorsFreeTime, isLogged = isLoggedUsr });
    }

    public async Task<ActionResult> OnGetAfterLoadRequestedHelp()
    {
        var myTimeSelectionAndJoinTimes = _AprenderService.GetMyTimeSelectionAndJoinTimes(
            UserProfile.Id,
            _meetUrl
        );

        _AprenderService.GetMyEvents(
            myTimeSelectionAndJoinTimes,
            UserProfile.Id,
            _meetUrl,
            MyEvents,
            OldMyEvents,
            UserProfile.Nome
        );

        HashSet<TimeSelection> valueSet = [];

        if (MyEvents != null)
        {
            valueSet = new HashSet<TimeSelection>(MyEvents.Values);
        }

        var timeSelections = GetFreeTimeService.ObtemTimeSelectionSetForHomeRequestedHelp(
            _context,
            valueSet
        );

        var timeSelectionGroupByPerfilId = timeSelections.GroupBy(e => e.PerfilId);

        var pedidos = await GetFreeTimeService.PrepareViewModelForRenderRequestedHelpBoard(
            timeSelectionGroupByPerfilId,
            _context,
            _httpClientFactory,
            _perfilWebService
        );

        return new JsonResult(new { pedidos = pedidos, isLogged = IsAuth });
    }

    public async Task<ActionResult> OnPostTrySetMentor()
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("../Perfil");
        }
        else if (!IsAuth)
        {
            return Redirect("/Identity/Account/Login");
        }
        if (JoinTime == null)
        {
            return Redirect("/.");
        }

        var timeSelection = _context
            .TimeSelections.Where(e => e.Id == JoinTime.TimeSelectionId)
            .FirstOrDefault();

        if (timeSelection == null)
        {
            return Redirect("/.");
        }
        if (timeSelection.Status != StatusTimeSelection.Pendente)
        {
            return Redirect("/.");
        }
        if (timeSelection.PerfilId == UserProfile.Id)
        {
            return Redirect("/.");
        }

        JoinTime.PerfilId = UserProfile.Id;

         var freeTimeBackstage = _context
                .FreeTimeBackstages.AsNoTracking()
                .FirstOrDefault(e => e.TimeSelectionId == timeSelection.Id);

        if(freeTimeBackstage?.Ilimitado ?? false){
            JoinTime.StatusJoinTime = StatusJoinTime.Marcado;
        }else{
            JoinTime.StatusJoinTime = StatusJoinTime.Pendente;
        }
        _context.JoinTimes.Add(JoinTime);

        var feedback = new FeedbackJoinTime
        {
            Id = Guid.NewGuid(),
            JoinTimeId = JoinTime.Id,
            DataTentativaMarcacao = DateTime.Now
        };
        _context.FeedbackJoinTimes?.Add(feedback);

        _context.SaveChanges();

        if (timeSelection.Tipo == EnumTipoTimeSelection.FreeTime)
        {
            var notification = new Notification
            {
                DestinoPerfilId = timeSelection.PerfilId ?? Guid.Empty,
                GeradorPerfilId = UserProfile.Id,
                TipoNotificacao = TipoNotificacao.NovoInteressadoMentoria,
                DataCriacao = DateTime.Now,
                Conteudo =
                    $@" está interessado
                        em receber mentoria {timeSelection.TituloTemporario}
                        no dia {timeSelection.StartTime.ToString("dd/MM/yyyy")}
                    ",
                ActionLink = "./?event=" + JoinTime.TimeSelectionId
            };

            await _messagePublisher.PublishAsync(typeof(NotificationsQueue).Name, notification);
        }
        else if (timeSelection.Tipo == EnumTipoTimeSelection.RequestHelp)
        {
            var notification = new Notification
            {
                DestinoPerfilId = timeSelection.PerfilId ?? Guid.Empty,
                GeradorPerfilId = UserProfile.Id,
                TipoNotificacao = TipoNotificacao.NovoInteressadoMentoria,
                DataCriacao = DateTime.Now,
                Conteudo =
                    $@" está interessado
                        em oferecer orientação para: {timeSelection.TituloTemporario}
                        no dia {timeSelection.StartTime.ToString("dd/MM/yyyy")}
                    ",
                ActionLink = "./?event=" + JoinTime.TimeSelectionId
            };
            await _messagePublisher.PublishAsync(typeof(NotificationsQueue).Name, notification);
        }

        return Redirect("./?event=" + JoinTime.TimeSelectionId);
    }

    public async Task<string> RenderViewAsync<TModel>(string viewName, TModel model)
    {
        return await RenderVideosService.RenderVideos(
            viewName,
            model,
            _viewEngine,
            PageContext,
            _tempDataProvider
        );
    }

    public async Task<ActionResult> OnGetChatGPT(string entrada)
    {
        try
        {
            var response = await _openAiService.GetTitleAndDescriptionSugestion(entrada);

            if (!string.IsNullOrEmpty(response?.Titulo))
            {
                return new JsonResult(response);
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> OnPostSetAluno(Guid joinId)
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("../Perfil");
        }
        var join = _context.JoinTimes.Where(e => e.Id == joinId).FirstOrDefault();
        if (join == null)
        {
            return Redirect("./");
        }
        var ts = _context.TimeSelections.Where(e => e.Id == join.TimeSelectionId).FirstOrDefault();
        if (ts == null)
        {
            return Redirect("./");
        }
        join.StatusJoinTime = StatusJoinTime.Marcado;

        _context.TimeSelections.Update(ts);
        _context.JoinTimes.Update(join);

        var feedback = _context
            .FeedbackTimeSelections.Where(e => e.TimeSelectionId == ts.Id)
            .FirstOrDefault();

        if (feedback != null)
        {
            feedback.Aceite = DateTime.Now;
            _context.FeedbackTimeSelections.Update(feedback);
        }
        _context.SaveChanges();
        FinalizeTimeSelectionAsMarked(ts.Id, join.PerfilId);
        Notification notification = null;
        if (ts.Tipo == EnumTipoTimeSelection.FreeTime)
        {
            notification = new Notification
            {
                DestinoPerfilId = join.PerfilId ?? Guid.Empty,
                GeradorPerfilId = UserProfile.Id,
                TipoNotificacao = TipoNotificacao.AlunoAceitoNaMentoria,
                DataCriacao = DateTime.Now,
                Conteudo =
                    $@" marcou com você a mentoria {ts.TituloTemporario}
                    no dia {ts.StartTime:dd/MM/yyyy}
                ",
                ActionLink = "./?event=" + join.TimeSelectionId
            };
        }
        else if (ts.Tipo == EnumTipoTimeSelection.RequestHelp)
        {
            notification = new Notification
            {
                DestinoPerfilId = join.PerfilId ?? Guid.Empty,
                GeradorPerfilId = UserProfile.Id,
                TipoNotificacao = TipoNotificacao.OrientadorAceito,
                DataCriacao = DateTime.Now,
                Conteudo =
                    $@" marcou com você a orientação para o pedido de ajuda: {ts.TituloTemporario}
                    no dia {ts.StartTime:dd/MM/yyyy}
                ",
                ActionLink = "./?event=" + join.TimeSelectionId
            };
        }
        if (notification != null)
        {
            await _messagePublisher.PublishAsync(typeof(NotificationsQueue).Name, notification);
        }
        return Redirect("./?event=" + join.TimeSelectionId);
    }

    public void FinalizeTimeSelectionAsMarked(Guid tsId, Guid? joinPerfilId)
    {
        var vagasTotal = _context
            ?.FreeTimeBackstages.Where(e => e.TimeSelectionId == tsId)
            .FirstOrDefault();

        var alunosTotal = _context
            ?.JoinTimes.Where(e =>
                e.TimeSelectionId == tsId && e.StatusJoinTime == StatusJoinTime.Marcado
            )
            .ToList();

        alunosTotal ??= new List<JoinTime>();

        var totalVagasOcupada = alunosTotal.Count;

        if (totalVagasOcupada >= vagasTotal?.MaxParticipants)
        {
            OnPostTimeSelectionStatusMarcado(tsId, joinPerfilId ?? Guid.Empty);
        }
    }

    public IActionResult OnPostTimeSelectionStatusMarcado(Guid id, Guid PerfilId = default(Guid))
    {
        var ts = _context?.TimeSelections.Where(e => e.Id == id).FirstOrDefault();
        if (ts == null)
        {
            return Redirect("./");
        }
        var rejecteds = _context
            ?.JoinTimes.Where(e =>
                e.TimeSelectionId == ts.Id
                // && (PerfilId != Guid.Empty ? e.PerfilId != PerfilId : true)
                && (PerfilId != Guid.Empty && e.PerfilId != PerfilId)
                && e.StatusJoinTime != StatusJoinTime.Marcado
            )
            .ToList();
        if (rejecteds != null)
        {
            foreach (var rejected in rejecteds)
            {
                rejected.StatusJoinTime = StatusJoinTime.Rejeitado;
                _context?.JoinTimes.Update(rejected);
            }
        }
        ts.Status = StatusTimeSelection.Marcado;
        ts.RoomId = CreatePrivateRoom(ts, ts.PerfilId ?? Guid.Empty);
        _context?.TimeSelections.Update(ts);
        _context?.SaveChanges();
        return Redirect("?event=" + ts.Id);
    }

    private Guid CreatePrivateRoom(TimeSelection ts, Guid perfilId)
    {
        var codigoSala =
            Guid.NewGuid().ToString().Replace("-", "")
            + "-"
            + Guid.NewGuid().ToString().Replace("-", "");

        var room = new Room
        {
            Id = new Guid(),
            CodigoSala = codigoSala,
            PerfilId = perfilId,
            EstaAberto = false,
            Titulo = ts.TituloTemporario,
            DataCriacao = DateTime.Now,
            UltimaAtualizacao = DateTime.Now,
            TipoSala = EnumTipoSalas.Mentoria,
            Privado = true
        };
        _context?.Rooms?.Add(room);

        foreach (var t in TagsSelected)
        {
            var tag = new Tag { Titulo = t, RoomRelacao = room.CodigoSala, };
            _context?.Tags?.Add(tag);
        }
        return room.Id;
    }
}
