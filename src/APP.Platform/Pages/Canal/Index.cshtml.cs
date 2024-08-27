using System.Net;
using System.Text;
using System.Text.Json;
using Background;
using ClassLib.Follow.Models.ViewModels;
using Domain.Entities;
using Domain.Enums;
using Domain.Models.ViewModels;
using Domain.RequestModels;
using Domain.WebServices;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Platform.Services;
using Queue;
using tags;

namespace APP.Platform.Pages;

public sealed class CanalIndexModel : CustomPageModel
{
    [BindProperty]
    public JoinTime? JoinTime { get; set; }
    public Dictionary<JoinTime, TimeSelection>? MyEvents { get; set; }
    public Dictionary<JoinTime, TimeSelection>? OldMyEvents { get; set; }
    private new readonly ApplicationDbContext _context;
    private new readonly PerfilDbContext _perfilContext;
    public List<PrivateLiveViewModel>? Lives { get; set; }

    [BindProperty]
    public bool IsUsrCanal { get; set; }

    [BindProperty]
    public ScheduleTimeSelectionRequestModel? ScheduleTimeSelection { get; set; }

    [BindProperty]
    public ScheduleLiveForTimeSelection? ScheduleLiveForTimeSelection { get; set; }

    [BindProperty]
    public ScheduleFreeTimeForTimeSelectionRequestModel? ScheduleFreeTimeForTimeSelection { get; set; }
    public Domain.Entities.Perfil? PerfilOwner { get; set; }
    private readonly IMessagePublisher _messagePublisher;
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private IPerfilWebService _perfilWebService { get; set; }
    private readonly ILiveService _liveService;
    public Dictionary<string, List<string>> RelatioTags { get; set; }
    public bool IsFollowing { get; set; }
    public int followersCount { get; set; }
    public int followingCount { get; set; }
    public IHttpClientFactory _httpClientFactory { get; set; }

    public CanalIndexModel(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IMessagePublisher messagePublisher,
        Settings settings,
        IPerfilWebService perfilWebService,
        ILiveService liveService
    )
        : base(context, httpClientFactory, httpContextAccessor, settings)
    {
        _httpClientFactory = httpClientFactory;
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _perfilWebService = perfilWebService;
        _context = context;
        _messagePublisher = messagePublisher;
        _liveService = liveService;
    }

    public async Task<IActionResult> OnGetAsync(string usr)
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("../Perfil");
        }

        var perfilResponse = await _perfilWebService.GetByUsername(usr);

        var perfilOwner = new Domain.Entities.Perfil
        {
            Id = perfilResponse.Id,
            Nome = perfilResponse.Nome,
            Foto = perfilResponse.Foto,
            Token = perfilResponse.Token,
            UserName = perfilResponse.UserName,
            Linkedin = perfilResponse.Linkedin,
            GitHub = perfilResponse.GitHub,
            Bio = perfilResponse.Bio,
            Email = perfilResponse.Email,
            Descricao = perfilResponse.Descricao,
            Experiencia = (Domain.Entities.ExperienceLevel)perfilResponse.Experiencia,
        };

        PerfilOwner = perfilOwner;

        var client = _httpClientFactory.CreateClient("CoreAPI");

        using var responseTaskFollow = await client.GetAsync(
            $"api/follow/getFollowInformation/{perfilOwner.Id}"
        );
        responseTaskFollow.EnsureSuccessStatusCode();

        var followInformation =
            await responseTaskFollow.Content.ReadFromJsonAsync<FollowInformationViewModel>();
        followersCount = followInformation.Followers;
        followingCount = followInformation.Following;

        RelatioTags = DataTags.GetTags();
        return Page();
    }

    public async Task<ActionResult> OnGetAfterloadCanal(
        string usr,
        bool isPrivate,
        int pageNumber = 1,
        int pageSize = 3
    )
    {
        var perfilResponse = await _perfilWebService.GetByUsername(usr);

        var perfilOwner = new Domain.Entities.Perfil
        {
            Id = perfilResponse.Id,
            Nome = perfilResponse.Nome,
            Foto = perfilResponse.Foto,
            Token = perfilResponse.Token,
            UserName = perfilResponse.UserName,
            Linkedin = perfilResponse.Linkedin,
            GitHub = perfilResponse.GitHub,
            Bio = perfilResponse.Bio,
            Email = perfilResponse.Email,
            Descricao = perfilResponse.Descricao,
            Experiencia = (Domain.Entities.ExperienceLevel)perfilResponse.Experiencia,
        };

        if (perfilOwner == null)
        {
            return BadRequest();
        }

        var pagedPrivateLives = await _liveService.RenderPrivateLives(
            perfilOwner,
            UserProfile.Id,
            isPrivate,
            pageNumber,
            pageSize
        );
        var liveSchedules = _liveService.RenderPreviewLiveSchedule(perfilOwner, UserProfile.Id);

        var savedVideosHtml = await RenderVideosService.RenderVideos(
            "Components/_PrivateVideosGroup",
            pagedPrivateLives,
            _viewEngine,
            PageContext,
            _tempDataProvider
        );

        var liveSchedulesHtml = await RenderVideosService.RenderVideos(
            "Components/_LiveSchedulePreviewGroup",
            liveSchedules,
            _viewEngine,
            PageContext,
            _tempDataProvider
        );

        return new JsonResult(
            new
            {
                privateLives = savedVideosHtml,
                liveSchedules = liveSchedulesHtml,
                isPrivateVideosChecked = isPrivate,
            }
        );
    }

    public IActionResult OnGetEditorAsync(string key)
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("../Perfil");
        }
        return Redirect("/canal/editor?key=" + key);
    }

    public async Task<IActionResult> OnGetFollow(string entityKey)
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return new JsonResult(new { });
        }

        var client = _httpClientFactory.CreateClient("CoreAPI");
        using var responseTask = await client.GetAsync(
            $"api/follow/toggleFollow/{UserProfile.Id}/{entityKey}"
        );

        responseTask.EnsureSuccessStatusCode();

        var isFollowing = await responseTask.Content.ReadFromJsonAsync<FollowToggleViewModel>();

        var result = isFollowing?.Active ?? false;

        return new JsonResult(new { IsFollowing = result });
    }

    //A partir daqui, a lógica de agendar horário.
    private async Task GetMyEvents()
    {
        MyEvents = new();
        OldMyEvents = new();

        var myJoins = _context
            .JoinTimes.AsNoTracking()
            .Where(e =>
                e.PerfilId == UserProfile.Id
                && e.StatusJoinTime != StatusJoinTime.Cancelado
                && e.StatusJoinTime != StatusJoinTime.Rejeitado
            )
            .ToList();

        var myJoinsTimeSelectionsIds = myJoins.Select(e => e.TimeSelectionId).ToList();

        var associatedTimeSelections = _context
            .TimeSelections.AsNoTracking()
            .Where(e => myJoinsTimeSelectionsIds.Contains(e.Id))
            .ToList();

        var timeSeletionIsFortags = myJoinsTimeSelectionsIds.Select(e => e.ToString()).ToList();
        var associatedTags = _context
            .Tags.AsNoTracking()
            .Where(e => timeSeletionIsFortags.Contains(e.LiveRelacao))
            .ToList();

        var associatedRoomsIds = associatedTimeSelections.Select(e => e.RoomId);
        var associatedRooms = _context
            .Rooms.AsNoTracking()
            .Where(e => associatedRoomsIds.Contains(e.Id))
            .ToList();

        var perfilTimeSelectionIds = associatedTimeSelections
            .Select(e => e.PerfilId)
            .Where(id => id.HasValue)
            .Select(id => id.Value)
            .ToList();

        var associatedPerfil = await _perfilWebService.GetAllById(perfilTimeSelectionIds) ?? new();

        var associatedPerfilLegacy = new List<Domain.Entities.Perfil>();

        foreach (var perfilDomain in associatedPerfil)
        {
            var perfilLegacy = new Domain.Entities.Perfil
            {
                Id = perfilDomain.Id,
                Nome = perfilDomain.Nome,
                Foto = perfilDomain.Foto,
                Token = perfilDomain.Token,
                UserName = perfilDomain.UserName,
                Linkedin = perfilDomain.Linkedin,
                GitHub = perfilDomain.GitHub,
                Bio = perfilDomain.Bio,
                Email = perfilDomain.Email,
                Descricao = perfilDomain.Descricao,
                Experiencia = (Domain.Entities.ExperienceLevel)perfilDomain.Experiencia,
            };
            associatedPerfilLegacy.Add(perfilLegacy);
        }

        foreach (var item in associatedTimeSelections)
        {
            var tags = associatedTags.Where(e => e.FreeTimeRelacao == item.Id.ToString()).ToList();
            item.Tags?.AddRange(tags);
            var perfil = associatedPerfilLegacy.Find(e => e.Id == item.PerfilId);
            item.Perfil = perfil;
            var code = associatedRooms.Find(e => e.Id == item.RoomId)?.CodigoSala;
            code ??= "";
            item.LinkSala = _meetUrl + "?name=" + code + "&usr=" + UserProfile.Nome;
        }

        var joinTimeSelectionDictionary = myJoins.ToDictionary(
            joinTime => joinTime,
            joinTime =>
                associatedTimeSelections.FirstOrDefault(timeSelection =>
                    timeSelection.Id == joinTime.TimeSelectionId
                )
        );

        foreach (var item in joinTimeSelectionDictionary)
        {
            if (item.Value == null)
            {
                continue;
            }
            if (item.Value.Status == StatusTimeSelection.Cancelado)
            {
                continue;
            }
            if (
                item.Value.EndTime < DateTime.Now
                && item.Value.Status != StatusTimeSelection.Concluído
                && item.Value.Status != StatusTimeSelection.Marcado
                && item.Value.Status != StatusTimeSelection.ConclusaoPendente
            )
            {
                OldMyEvents![item.Key] = item.Value;
                continue;
            }

            item.Value.TempoRestante = Math.Floor(
                    item.Value.StartTime.Subtract(DateTime.Now).TotalHours
                )
                .ToString();

            if (item.Value == null)
            {
                continue;
            }

            if (
                (
                    item.Key.StatusJoinTime == StatusJoinTime.Marcado
                    || item.Key.StatusJoinTime == StatusJoinTime.ConclusaoPendente
                )
                && item.Value.StartTime < DateTime.Now
            )
            {
                item.Value.ActionNeeded = true;
            }
            MyEvents![item.Key] = item.Value;
        }
    }

    public async Task<ActionResult> OnGetMentores(Guid id)
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return NotFound();
        }

        await GetMyEvents();

        var perfilResponse = await _perfilWebService.GetById(id);

        var perfil = new Domain.Entities.Perfil
        {
            Id = perfilResponse.Id,
            Nome = perfilResponse.Nome,
            Foto = perfilResponse.Foto,
            Token = perfilResponse.Token,
            UserName = perfilResponse.UserName,
            Linkedin = perfilResponse.Linkedin,
            GitHub = perfilResponse.GitHub,
            Bio = perfilResponse.Bio,
            Email = perfilResponse.Email,
            Descricao = perfilResponse.Descricao,
            Experiencia = (Domain.Entities.ExperienceLevel)perfilResponse.Experiencia,
        };

        if (perfil == null)
        {
            return BadRequest();
        }

        PerfilOwner = perfil;
        HashSet<TimeSelection> valueSet = new HashSet<TimeSelection>(MyEvents!.Values);

        var timeSelections =
            GetFreeTimeService.ObtemTimeSelectionsByPerfilIdExcludingSet(
                PerfilOwner.Id,
                valueSet,
                _context
            ) ?? new List<TimeSelection>();

        var filteredTimeSelections = GetFreeTimeService.FiltraPelosNaoConflitantes(
            timeSelections,
            _context,
            MyEvents
        );

        var timeSelectionsFinal = filteredTimeSelections
            .Select(e => new TimeSelectionForMentorFreeTimeViewModel()
            {
                TimeSelectionId = e.Id.ToString(),
                PerfilId = e.PerfilId.ToString() ?? Guid.Empty.ToString(),
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Titulo = e.TituloTemporario,
                Variacao = (int)e.Variacao,
            })
            .ToList();

        var timeSelectionGroupByPerfilId = timeSelectionsFinal.GroupBy(e => e.PerfilId);

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

    public async Task<ActionResult> OnGetFreeTimeConflictChecker(string freeTimeId)
    {
        await GetMyEvents();

        if (MyEvents == null || MyEvents.Count <= 0)
        {
            return new JsonResult(true);
        }

        bool canSetToTryFreeTime = !MyEvents.Values.Any(e => e.Id.ToString() == freeTimeId);
        return new JsonResult(canSetToTryFreeTime);
    }

    public async Task<ActionResult> OnPostTrySetMentor()
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("../Perfil");
        }

        if (JoinTime == null)
        {
            return Redirect("/Canal");
        }

        var timeSelection = _context
            .TimeSelections.Where(e => e.Id == JoinTime.TimeSelectionId)
            .FirstOrDefault();

        var perfilResponse = await _perfilWebService.GetById((Guid)(timeSelection?.PerfilId));

        var perfil = new Domain.Entities.Perfil
        {
            Id = perfilResponse.Id,
            Nome = perfilResponse.Nome,
            Foto = perfilResponse.Foto,
            Token = perfilResponse.Token,
            UserName = perfilResponse.UserName,
            Linkedin = perfilResponse.Linkedin,
            GitHub = perfilResponse.GitHub,
            Bio = perfilResponse.Bio,
            Email = perfilResponse.Email,
            Descricao = perfilResponse.Descricao,
            Experiencia = (Domain.Entities.ExperienceLevel)perfilResponse.Experiencia,
        };

        if (perfil == null)
        {
            return BadRequest();
        }

        var channelUserName = perfil.UserName;

        if (UserProfile.Id == Guid.Empty)
        {
            var urlToReturn = $"/Canal?usr={channelUserName}";

            return Redirect($"/Identity/Account/Login?returnUrl={urlToReturn}");
        }

        JoinTime.PerfilId = UserProfile.Id;

        var freeTimeBackstage = _context
            .FreeTimeBackstages.AsNoTracking()
            .FirstOrDefault(e => e.TimeSelectionId == timeSelection.Id);

        if (freeTimeBackstage?.Ilimitado ?? false)
        {
            JoinTime.StatusJoinTime = StatusJoinTime.Marcado;
        }
        else
        {
            JoinTime.StatusJoinTime = StatusJoinTime.Pendente;
        }

        _context.JoinTimes.Add(JoinTime);

        var feedback = new FeedbackJoinTime
        {
            Id = Guid.NewGuid(),
            JoinTimeId = JoinTime.Id,
            DataTentativaMarcacao = DateTime.Now,
        };
        _context.FeedbackJoinTimes?.Add(feedback);

        _context.SaveChanges();

        if (timeSelection != null && timeSelection.Tipo == EnumTipoTimeSelection.FreeTime)
        {
            var notification = new Notification
            {
                DestinoPerfilId = timeSelection!.PerfilId ?? Guid.Empty,
                GeradorPerfilId = UserProfile.Id,
                TipoNotificacao = TipoNotificacao.NovoInteressadoMentoria,
                DataCriacao = DateTime.Now,
                Conteudo =
                    $@" está interessado
                    em receber mentoria {timeSelection.TituloTemporario}
                    no dia {timeSelection.StartTime:dd/MM/yyyy}
                ",
                ActionLink = "./?event=" + JoinTime.TimeSelectionId,
            };

            await _messagePublisher.PublishAsync(typeof(NotificationsQueue).Name, notification);
        }
        else if (timeSelection != null && timeSelection.Tipo == EnumTipoTimeSelection.RequestHelp)
        {
            var notification = new Notification
            {
                DestinoPerfilId = timeSelection!.PerfilId ?? Guid.Empty,
                GeradorPerfilId = UserProfile.Id,
                TipoNotificacao = TipoNotificacao.NovaOfertaOrientacao,
                DataCriacao = DateTime.Now,
                Conteudo =
                    $@" está interessado
                    em oferecer orientação para o pedido de ajuda: {timeSelection.TituloTemporario}
                    no dia {timeSelection.StartTime:dd/MM/yyyy}
                ",
                ActionLink = "./?event=" + JoinTime.TimeSelectionId,
            };

            await _messagePublisher.PublishAsync(typeof(NotificationsQueue).Name, notification);
        }

        return Redirect("./?event=" + JoinTime.TimeSelectionId);
    }
}
