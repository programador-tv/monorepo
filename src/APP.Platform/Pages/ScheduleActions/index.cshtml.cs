using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using APP.Platform.Pages.Components.ModalJoinTime;
using APP.Platform.Pages.Components.TimeSelections;
using APP.Platform.Services;
using Background;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.ViewModels;
using Domain.RequestModels;
using Domain.Utils;
using Domain.WebServices;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models;
using Platform.Services;
using Queue;
using SixLabors.ImageSharp.Formats.Jpeg;
using tags;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;

namespace APP.Platform.Pages.ScheduleActions
{
    public sealed class ScheduleActionsModel : CustomPageModel
    {
        private new readonly ApplicationDbContext _context;
        public Dictionary<string, List<string>> RelatioTags { get; set; }

        [BindProperty]
        public ScheduleTimeSelectionRequestModel? ScheduleTimeSelection { get; set; }

        [BindProperty]
        public ScheduleLiveForTimeSelection? ScheduleLiveForTimeSelection { get; set; }

        [BindProperty]
        public ScheduleFreeTimeForTimeSelectionRequestModel? ScheduleFreeTimeForTimeSelection { get; set; }

        [BindProperty]
        public List<string> TagsSelected { get; set; }

        [BindProperty]
        public string? JoinId { get; set; }

        [BindProperty]
        public EnumTipoTimeSelection TimeSelectionType { get; set; }

        [BindProperty]
        public EnumWeekPattern WeekPattern { get; set; }

        [BindProperty]
        public EnumRepeatWeekParttern? RepeatWeekParttern { get; set; }

        [BindProperty]
        public bool NeedAddActualMonth { get; set; }

        [BindProperty]
        public string? TimeSelectionId { get; set; }

        public Dictionary<TimeSelection, List<JoinTimeViewModel>> TimeSelectionList { get; set; }
        public List<TimeSelection> OldTimeSelectionList { get; set; }
        public Dictionary<
            TimeSelection,
            List<Domain.Entities.Perfil>
        > TimeSelectionsCheckedUsers { get; set; }

        [BindProperty]
        public TimeSelection? TimeSelection { get; set; }

        [BindProperty]
        public string? Descricao { get; set; }

        [BindProperty]
        public string? IdToCancel { get; set; }

        [BindProperty]
        public FreeTimeBackstage TimeSelectionBackstage { get; set; }

        private readonly IMessagePublisher _messagePublisher;

        private readonly OpenAiService _openAiService;
        private readonly IEnsinarService _ensinarService;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IAliasService _aliasService;
        public Dictionary<JoinTime, TimeSelection>? MyEvents { get; set; } = new();
        public Dictionary<JoinTime, TimeSelection> OldMyEvents { get; set; } = new();
        private readonly IAprenderService _AprenderService;
        private readonly IHttpClientFactory _httpClientFactory;
        private IPerfilWebService _perfilWebService { get; set; }
        private IHelpResponseWebService _helpResponseWebService { get; set; }

        public ScheduleActionsModel(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IAprenderService aprenderService,
            IEnsinarService ensinarService,
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IMessagePublisher messagePublisher,
            OpenAiService openAiService,
            Settings settings,
            IAliasService aliasService,
            IPerfilWebService perfilWebService,
            IHelpResponseWebService helpResponseWebService
        )
            : base(context, httpClientFactory, httpContextAccessor, settings)
        {
            _httpClientFactory = httpClientFactory;
            _tempDataProvider = tempDataProvider;
            _viewEngine = viewEngine;
            _ensinarService = ensinarService;
            _AprenderService = aprenderService;
            _context = context;
            _messagePublisher = messagePublisher;
            _openAiService = openAiService;
            _aliasService = aliasService;
            _perfilWebService = perfilWebService;
            _helpResponseWebService = helpResponseWebService;
            RelatioTags = DataTags.GetTags();
            TagsSelected = new();
            TimeSelectionList = new();
            OldTimeSelectionList = new();
            TimeSelectionsCheckedUsers = new();
            TimeSelectionBackstage = new();
        }

        [BindProperty]
        public string? Skills { get; set; }

        [BindProperty]
        public PreLiveViewModel? Live { get; set; }

        [BindProperty]
        public Room? Room { get; set; }

        public SelectList? TagsFront { get; set; }

        public IActionResult OnGet()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }
            return Page();
        }

        public async Task<IActionResult> OnGetRenderCalendarAsync()
        {
            if (!IsAuth)
            {
                return NotFound();
            }

            var myJoinTimes = _context
                .JoinTimes.Where(j =>
                    j.PerfilId == UserProfile.Id && j.StatusJoinTime != StatusJoinTime.Cancelado
                )
                .ToList();

            var tsFromJtIds = myJoinTimes.Select(e => e.TimeSelectionId).ToList();

            var timeSelectionsQuery = _context
                .TimeSelections.Where(e =>
                    (e.PerfilId == UserProfile.Id && e.Status != StatusTimeSelection.Cancelado)
                    || (tsFromJtIds.Contains(e.Id))
                )
                .OrderBy(e => e.StartTime)
                .ToList();

            OldTimeSelectionList = timeSelectionsQuery
                .Where(ts => _ensinarService.ShouldAddToOldList(ts))
                .ToList();

            var timeSelections = timeSelectionsQuery
                .Where(ts => !_ensinarService.ShouldAddToOldList(ts))
                .ToList();

            var roomIds = timeSelections.Where(ts => ts.RoomId != null).Select(ts => ts.RoomId);

            var rooms = _context.Rooms.Where(r => roomIds.Contains(r.Id)).ToList();

            var tsIds = timeSelections.Select(ts => ts.Id).ToList();
            var stringTsIds = tsIds.Select(tsId => tsId.ToString()).ToList();
            var tags = _context
                .Tags.Where(t =>
                    t.FreeTimeRelacao != null && stringTsIds.Contains(t.FreeTimeRelacao)
                )
                .ToList();

            foreach (var ts in timeSelections)
            {
                var room = rooms.Find(r => r.Id == ts.RoomId);
                ts.Tags = tags.Where(t => t.FreeTimeRelacao == ts.Id.ToString()).ToList();
                ts.TempoRestante = Math.Floor(ts.StartTime.Subtract(DateTime.Now).TotalHours)
                    .ToString();
                ts.LinkSala =
                    room != null
                        ? _meetUrl + "?name=" + room.CodigoSala + "&usr=" + UserProfile.UserName
                        : string.Empty;
                ts.ActionNeeded = _ensinarService.SetActionNeeded(ts);
            }

            var joins = _context
                .JoinTimes.Where(j =>
                    tsIds.Contains(j.TimeSelectionId ?? Guid.Empty)
                    && j.StatusJoinTime != StatusJoinTime.Cancelado
                )
                .ToList();

            var perfilsIds = joins
                .Select(j => j.PerfilId)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            perfilsIds.AddRange(
                timeSelections
                    .Select(ts => ts.PerfilId)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
            );

            var perfils = await _perfilWebService.GetAllById(perfilsIds) ?? new();

            var perfilsLegacy = new List<Domain.Entities.Perfil>();

            foreach (var perfil in perfils)
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
                    Experiencia = (Domain.Entities.ExperienceLevel)perfil.Experiencia,
                };

                perfilsLegacy.Add(perfilLegacy);
            }

            var joinViewModels = joins
                .Select(j => new JoinTimeViewModel
                {
                    TimeSelectionId = j.TimeSelectionId,
                    JoinTimeId = j.Id,
                    StatusJoinTime = j.StatusJoinTime,
                    Perfil = perfilsLegacy.Find(p => p.Id == j.PerfilId),
                })
                .Where(e => e.Perfil != null)
                .ToList();

            TimeSelectionList = timeSelections.ToDictionary(
                ts => ts,
                ts =>
                    joinViewModels
                        .Where(j =>
                            j.TimeSelectionId == ts.Id
                            && j.StatusJoinTime != StatusJoinTime.Cancelado
                        )
                        .ToList()
            );

            var times = _ensinarService.CastTimeSelectionIntoCalendarSectionViewModel(
                TimeSelectionList.Keys.Where(e => e.PerfilId == UserProfile.Id).ToList()
            );
            var attachTimes = _ensinarService.CastTimeSelectionIntoCalendarSectionViewModel(
                TimeSelectionList.Keys.Where(e => e.PerfilId != UserProfile.Id).ToList()
            );

            var old = _ensinarService.CastTimeSelectionIntoBadFinishedCalendarSectionViewModel(
                OldTimeSelectionList
            );

            var freeTimeList = TimeSelectionList
                .Where(e => e.Key.PerfilId == UserProfile.Id)
                .Where(kvp => kvp.Key.Tipo == EnumTipoTimeSelection.FreeTime)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var requestHelpList = TimeSelectionList
                .Where(e => e.Key.PerfilId == UserProfile.Id)
                .Where(kvp => kvp.Key.Tipo == EnumTipoTimeSelection.RequestHelp)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var livesList = TimeSelectionList
                .Keys.Where(ts => ts.Tipo == EnumTipoTimeSelection.Live)
                .ToList();

            var liveTimeSelectionIds = livesList.Select(ts => ts.Id).ToList();

            var backstage = _context
                .LiveBackstages.Where(x => liveTimeSelectionIds.Contains(x.TimeSelectionId))
                .ToList();

            foreach (var backstageItem in backstage)
            {
                var ts = livesList.Find(x => x.Id == backstageItem.TimeSelectionId);
                if (ts != null)
                {
                    ts.LinkSala = backstageItem.LiveId.ToString();
                }
            }

            foreach (var time in freeTimeList)
            {
                var joinTimes = _context
                    .JoinTimes.Where(j =>
                        j.TimeSelectionId == time.Key.Id
                        && (
                            j.StatusJoinTime == StatusJoinTime.Marcado
                            || j.StatusJoinTime == StatusJoinTime.Pendente
                        )
                    )
                    .ToList();

                var pendentJoinTimes = joinTimes
                    .Where(j => j.StatusJoinTime == StatusJoinTime.Pendente)
                    .ToArray();
                var markedJoinTimes = joinTimes.Where(j =>
                    j.StatusJoinTime == StatusJoinTime.Marcado
                );

                var freeTimeBackstage = _context
                    .FreeTimeBackstages.AsNoTracking()
                    .FirstOrDefault(e => e.TimeSelectionId == time.Key.Id);

                var max = freeTimeBackstage?.MaxParticipants ?? 1;

                var availableSlots = max - markedJoinTimes.Count();

                time.Key.ShowSelectRandomStudents =
                    availableSlots > 0 && pendentJoinTimes.Length > 0;

                time.Key.MaxParticipants = max;
                time.Key.Ilimitado = freeTimeBackstage?.Ilimitado ?? false;
            }

            var RhListIds = requestHelpList.Keys.Select(kvp => kvp.Id).ToList();

            var client = _httpClientFactory.CreateClient("CoreAPI");

            var json = JsonSerializer.Serialize(RhListIds);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var responseHelpTask = await client.PostAsync(
                $"api/helpbackstage/AllByIds",
                content
            );

            var helpBackstages =
                await responseHelpTask.Content.ReadFromJsonAsync<List<HelpBackstage>>() ?? [];

            var RHKeys = requestHelpList.Select(ts => ts.Key).ToList();

            RHKeys.ForEach(key =>
            {
                var rhBackstage = helpBackstages.Find(x => x.TimeSelectionId == key.Id);

                if (rhBackstage != null)
                {
                    key.Descricao = rhBackstage.Descricao;
                    key.RequestedHelpImagePath = rhBackstage.ImagePath;
                }

                if (requestHelpList.ContainsKey(key))
                {
                    requestHelpList[key] = requestHelpList
                        .FirstOrDefault(x => x.Key.Id == key.Id)
                        .Value;
                }
            });

            var userFreeTimesHtml = await RenderViewAsync(
                "Components/TimeSelections/_ModalFreeTimePanel",
                new ModalFreeTimePanel { TimeSelectionAndJoinTimes = freeTimeList }
            );

            foreach (var kvp in requestHelpList)
            {
                freeTimeList.Add(kvp.Key, kvp.Value);
            }

            var userTimeSelectionHtml = await RenderViewAsync(
                "Components/TimeSelections/_ModalFreeTime",
                new _ModalFreeTimeModel { TimeSelectionAndJoinTimes = freeTimeList }
            );

            userTimeSelectionHtml += await RenderViewAsync(
                "Components/TimeSelections/_ModalLive",
                new ModalLive { LiveTimeSelection = livesList }
            );

            var TsList = timeSelections
                .Where(e => e.PerfilId != UserProfile.Id)
                .ToDictionary(ts => joins.Last(j => j.TimeSelectionId == ts.Id), ts => ts);

            var attatchFreeTimeList = TsList
                .Where(kvp => kvp.Value.Tipo == EnumTipoTimeSelection.FreeTime)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var attachHelpList = TsList
                .Where(kvp => kvp.Value.Tipo == EnumTipoTimeSelection.RequestHelp)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            foreach (var item in attatchFreeTimeList)
            {
                item.Value.Perfil = perfilsLegacy.Find(e => e.Id == item.Value.PerfilId);
            }

            foreach (var item in attachHelpList)
            {
                item.Value.Perfil = perfilsLegacy.Find(e => e.Id == item.Value.PerfilId);
            }

            var userJoinTimesHtml = await RenderViewAsync(
                "Components/ModalJoinTime/_ModalJoinTimePanel",
                new ModalJoinTimePanel { JoinEvent = attatchFreeTimeList }
            );

            RhListIds = attachHelpList.Select(kvp => kvp.Value.Id).ToList();

            json = JsonSerializer.Serialize(RhListIds);
            content = new StringContent(json, Encoding.UTF8, "application/json");
            using var responseJoinTimeHelpTask = await client.PostAsync(
                $"api/helpbackstage/AllByIds",
                content
            );
            helpBackstages =
                await responseJoinTimeHelpTask.Content.ReadFromJsonAsync<List<HelpBackstage>>()
                ?? [];

            foreach (var kvp in attachHelpList)
            {
                var rhBackstage = helpBackstages.Find(x => x.TimeSelectionId == kvp.Value.Id);

                if (rhBackstage != null)
                {
                    kvp.Value.Descricao = rhBackstage.Descricao;
                    kvp.Value.RequestedHelpImagePath = rhBackstage.ImagePath;
                }

                attatchFreeTimeList.Add(kvp.Key, kvp.Value);
            }

            var userLivesPanelHtml = await RenderViewAsync(
                "Components/TimeSelections/_ModalLivesPanel",
                new ModalLivesPanel { TimeSelections = livesList }
            );

            userTimeSelectionHtml += await RenderViewAsync(
                "Components/ModalJoinTime/_ModalJoinTimeEvent",
                new _ModalJoinTimeEvent { JoinEvent = attatchFreeTimeList }
            );

            var userRequestHelpListHtml = await RenderViewAsync(
                "Components/TimeSelections/_ModalFreeTimePanel",
                new ModalFreeTimePanel { TimeSelectionAndJoinTimes = requestHelpList }
            );

            var userSolvedHelpListHtml = await RenderViewAsync(
                "Components/ModalJoinTime/_ModalJoinTimePanel",
                new ModalJoinTimePanel { JoinEvent = attachHelpList }
            );

            return new JsonResult(
                new CalendarRenderObjectsViewModel
                {
                    MyTimeSelection = times,
                    AttachedTimeSelection = attachTimes,
                    BadFinished = old,
                    Modals = userTimeSelectionHtml,
                    TimeSelectionPanelModals = userFreeTimesHtml,
                    JoinTimeModalsPanel = userJoinTimesHtml,
                    RequestHelpModalsPanel = userRequestHelpListHtml,
                    SolvedHelpModalsPanel = userSolvedHelpListHtml,
                    LivesModalsPanel = userLivesPanelHtml,
                }
            );
        }

        public async Task<ActionResult> OnPostCancelMyInvitationAsync()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return new JsonResult(new { error = "Usuário não encontrado." });
            }
            if (IdToCancel == null)
            {
                return BadRequest("Evento não encontrado");
            }
            var join = _context
                .JoinTimes.Where(e => e.Id == Guid.Parse(IdToCancel))
                .FirstOrDefault();
            if (join == null)
            {
                return BadRequest("Evento não encontrado");
            }
            join.StatusJoinTime = StatusJoinTime.Cancelado;
            _context.JoinTimes.Update(join);

            var time = _context
                .TimeSelections.Where(e => e.Id == join.TimeSelectionId)
                .FirstOrDefault();

            if (time != null)
            {
                time.Status = StatusTimeSelection.Pendente;
                _context.TimeSelections.Update(time);
            }

            var feedback = _context
                .FeedbackJoinTimes.Where(e => e.JoinTimeId == join.Id)
                .FirstOrDefault();

            if (feedback != null)
            {
                feedback.DataCancelamento = DateTime.Now;
                _context.FeedbackJoinTimes.Update(feedback);
            }
            Notification notification = null;
            if (time != null && time.Tipo == EnumTipoTimeSelection.FreeTime)
            {
                notification = new Notification
                {
                    DestinoPerfilId = time.PerfilId ?? Guid.Empty,
                    GeradorPerfilId = UserProfile.Id,
                    TipoNotificacao = TipoNotificacao.AlunoCancelaMentoria,
                    DataCriacao = DateTime.Now,
                    Conteudo =
                        $@" cancelou a mentoria {time.TituloTemporario} no dia {time.StartTime:dd/MM/yyyy}",
                    ActionLink = "./",
                };
            }
            else if (time != null && time.Tipo == EnumTipoTimeSelection.RequestHelp)
            {
                notification = new Notification
                {
                    DestinoPerfilId = time.PerfilId ?? Guid.Empty,
                    GeradorPerfilId = UserProfile.Id,
                    TipoNotificacao = TipoNotificacao.OrientadorCancelaAjuda,
                    DataCriacao = DateTime.Now,
                    Conteudo =
                        $@" cancelou orientação para o pedido de ajuda {time.TituloTemporario} no dia {time.StartTime:dd/MM/yyyy}",
                    ActionLink = "./",
                };
            }
            if (notification != null)
            {
                await _messagePublisher.PublishAsync(typeof(NotificationsQueue).Name, notification);
            }
            await _context.SaveChangesAsync();
            return new JsonResult(new { });
        }

        public async Task<string> RenderViewAsync<TModel>(string viewName, TModel model)
        {
            using (var writer = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(PageContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException(
                        $"{viewName} does not match any available view"
                    );
                }

                var viewDictionary = new ViewDataDictionary<TModel>(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary()
                )
                {
                    Model = model,
                };

                var viewContext = new ViewContext(
                    PageContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(PageContext.HttpContext, _tempDataProvider),
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }

        public IActionResult OnGetPartialFreeTimeModal(string timeSelectionId)
        {
            _ensinarService.GetTimeSelectionItem(
                Guid.Parse(timeSelectionId),
                UserProfile,
                _meetUrl,
                TimeSelectionsCheckedUsers,
                OldTimeSelectionList,
                TimeSelectionList
            );

            if (TimeSelectionList.Count != 1)
            {
                return BadRequest();
            }
            var timeSelectionAndJoinTimes = TimeSelectionList.First();
            _ensinarService.CheckActionNeedAndUpdateTime(timeSelectionAndJoinTimes);

            var timeDelta = timeSelectionAndJoinTimes.Key!.StartTime - DateTime.Now;
            string tempoRestante = TimeHelper.ReturnRemainingTimeString(timeDelta);

            return Partial(
                "Components/TimeSelections/_PendentesCard",
                new PendentesCardPageModel
                {
                    Id = timeSelectionAndJoinTimes.Key.Id,
                    Titulo = timeSelectionAndJoinTimes.Key.TituloTemporario ?? string.Empty,
                    TempoRestante = tempoRestante,
                }
            );
        }

        public IActionResult OnGetPartialFreeTimePanel(string timeSelectionId)
        {
            _ensinarService.GetTimeSelectionItem(
                Guid.Parse(timeSelectionId),
                UserProfile,
                _meetUrl,
                TimeSelectionsCheckedUsers,
                OldTimeSelectionList,
                TimeSelectionList
            );

            if (TimeSelectionList.Count != 1)
            {
                return BadRequest();
            }
            var timeSelectionAndJoinTimes = TimeSelectionList.First();

            var freeTimeBackstage = _context
                .FreeTimeBackstages.AsNoTracking()
                .FirstOrDefault(e => e.TimeSelectionId == timeSelectionAndJoinTimes.Key.Id);

            timeSelectionAndJoinTimes.Key.MaxParticipants = freeTimeBackstage?.MaxParticipants ?? 1;
            timeSelectionAndJoinTimes.Key.Ilimitado = freeTimeBackstage?.Ilimitado ?? false;

            _ensinarService.CheckActionNeedAndUpdateTime(timeSelectionAndJoinTimes);

            return Partial(
                "Components/TimeSelections/_CallbackSaveFreeTime",
                new _CallbackSaveFreeTimeModel
                {
                    TimeSelectionAndJoinTimes = timeSelectionAndJoinTimes,
                    TimeSelectionsCheckedUsers = TimeSelectionsCheckedUsers.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Where(p => p != null).ToList()
                    ),
                }
            );
        }

        public IActionResult OnGetPartialLiveModal(string timeSelectionId)
        {
            _ensinarService.GetTimeSelectionItem(
                Guid.Parse(timeSelectionId),
                UserProfile,
                _meetUrl,
                TimeSelectionsCheckedUsers,
                OldTimeSelectionList,
                TimeSelectionList
            );

            var ts = TimeSelectionList.Keys.FirstOrDefault();

            if (ts == null)
            {
                return BadRequest();
            }

            var backstage = _context
                .LiveBackstages.Where(x => x.TimeSelectionId.ToString() == timeSelectionId)
                .FirstOrDefault();

            if (backstage == null)
            {
                return BadRequest();
            }

            ts.LinkSala = backstage.LiveId.ToString();
            ts.Descricao = backstage.Descricao;

            var start = ts.StartTime;
            ts.TempoRestante = GetFreeTimeService.GetTempoRestante(start);

            return Partial(
                "Components/TimeSelections/_ModalLive",
                new ModalLive { LiveTimeSelection = new List<TimeSelection> { ts } }
            );
        }

        // private PartialViewResult BuildSideCardsByTimeSelection(
        //     KeyValuePair<TimeSelection, List<JoinTimeViewModel>> timeSelectionAndJoinTimes,
        //     Dictionary<TimeSelection, List<Domain.Entities.Perfil>> TimeSelectionsCheckedUsers
        // )
        // {
        //     var firstCheckedUser = TimeSelectionsCheckedUsers.FirstOrDefault();
        //     BaseCardPageModel? vm;
        //     try
        //     {
        //         if (timeSelectionAndJoinTimes.Key.Status == StatusTimeSelection.ConclusaoPendente)
        //         {
        //             vm = _ensinarService.HandlePendingConclusionStatus(
        //                 timeSelectionAndJoinTimes,
        //                 firstCheckedUser
        //             );
        //             return Partial("Components/TimeSelections/_ConsideracoesPendentesCard", vm);
        //         }

        //         if (timeSelectionAndJoinTimes.Key.Status == StatusTimeSelection.Marcado)
        //         {
        //             vm = _ensinarService.HandleMarkedStatus(
        //                 timeSelectionAndJoinTimes,
        //                 firstCheckedUser
        //             );
        //             string partialViewName =
        //                 vm is EventosProximosCardPageModel
        //                     ? "Components/TimeSelections/_EventosProximosCard"
        //                     : "Components/TimeSelections/_EventosMarcadosCard";

        //             return Partial(partialViewName, vm);
        //         }
        //         if (timeSelectionAndJoinTimes.Key.Status == StatusTimeSelection.Pendente)
        //         {
        //             vm = _ensinarService.HandlePendingStatus(timeSelectionAndJoinTimes);
        //             return Partial("Components/TimeSelections/_PendentesCard", vm);
        //         }
        //         if (timeSelectionAndJoinTimes.Key.Status == StatusTimeSelection.Concluído)
        //         {
        //             vm = _ensinarService.HandleConcludedStatus(
        //                 timeSelectionAndJoinTimes,
        //                 firstCheckedUser
        //             );
        //             return Partial("Components/TimeSelections/_ConcluidosCard", vm);
        //         }
        //     }
        //     catch
        //     {
        //         Console.WriteLine("Nenhum componente pode ser renderizado");
        //     }
        //     return Partial("_Empty");
        // }

        // public IActionResult OnGetPartialSideCards(string timeSelectionId)
        // {
        //     var timeSelectionAndJoinTimes = GetTimeSelectionAndJoinTimes(timeSelectionId);
        //     var cardPartial = BuildSideCardsByTimeSelection(
        //         timeSelectionAndJoinTimes,
        //         TimeSelectionsCheckedUsers
        //     );
        //     if (timeSelectionAndJoinTimes.Key == null)
        //     {
        //         return BadRequest();
        //     }
        //     return cardPartial;
        // }

        private static StringContent Serializer<T>(T live)
        {
            var json = JsonSerializer.Serialize(live);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return content;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }

            ModelState.Remove("Live.PerfilId");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            bool isUsingObs = false;
            if (!string.IsNullOrEmpty(Live?.IsUsingObs))
            {
                _ = bool.TryParse(Live?.IsUsingObs, out isUsingObs);
            }
            var Thumbnail = ResizeImageToBase64(Live?.Thumbnail!) ?? "";

            var newLive = new Live
            {
                PerfilId = UserProfile.Id,
                Titulo = Live?.Titulo,
                Descricao = Live?.Descricao,
                Thumbnail = Thumbnail,
                LiveEstaAberta = false,
                DataCriacao = DateTime.Now,
                IsUsingObs = isUsingObs,
                StreamId = Guid.NewGuid().ToString(),
                UrlAlias = _aliasService.AliasGeneratorAsync(Live?.Titulo!).Result,
            };

            var client = _httpClientFactory.CreateClient("CoreAPI");
            var content = Serializer<Live>(newLive);
            using var responseTask = await client.PostAsync($"api/lives", content);

            var live = await responseTask.Content.ReadFromJsonAsync<Live>();

            if (responseTask.StatusCode != System.Net.HttpStatusCode.OK || live == null)
            {
                return BadRequest();
            }

            foreach (var selected in TagsSelected)
            {
                var tag = new Tag { Titulo = selected, LiveRelacao = live.Id.ToString() };
                _context.Tags.Add(tag);
            }

            _context.SaveChanges();

            string url;
            if (newLive.IsUsingObs)
            {
                url = "../Studio/OBS?mainkey=" + live.Id;
            }
            else
            {
                url = "../Studio?mainkey=" + live.Id;
            }
            return Redirect(url);
        }

        public async Task<IActionResult> OnGetAcceptance(string id)
        {
#warning se vem o id(token) do front, provavelmente os dados buscados aqui ja estão disponiveis la

            var perfilResponse = await _perfilWebService.GetByToken(id);

            var perfilLegacy = new Domain.Entities.Perfil
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

            return new JsonResult(perfilLegacy);
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
            var ts = _context
                .TimeSelections.Where(e => e.Id == join.TimeSelectionId)
                .FirstOrDefault();
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
                    ActionLink = "./?event=" + join.TimeSelectionId,
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
                        $@" marcou uma orientação você no pedido de ajuda {ts.TituloTemporario}
                    no dia {ts.StartTime:dd/MM/yyyy}
                ",
                    ActionLink = "./?event=" + join.TimeSelectionId,
                };
            }
            if (notification != null)
            {
                await _messagePublisher.PublishAsync(typeof(NotificationsQueue).Name, notification);
            }
            return Redirect("./?event=" + join.TimeSelectionId);
        }

        public IActionResult OnPostSetAlunoAleatorio(Guid timeSelectionId)
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }

            var joinTimes = _context
                .JoinTimes.Where(j =>
                    j.TimeSelectionId == timeSelectionId
                    && (
                        j.StatusJoinTime == StatusJoinTime.Marcado
                        || j.StatusJoinTime == StatusJoinTime.Pendente
                    )
                )
                .ToList();

            var pendentJoinTimes = joinTimes
                .Where(j => j.StatusJoinTime == StatusJoinTime.Pendente)
                .ToArray();
            var markedJoinTimes = joinTimes.Where(j => j.StatusJoinTime == StatusJoinTime.Marcado);

            var freeTimeBackstage =
                _context
                    .FreeTimeBackstages.AsNoTracking()
                    .FirstOrDefault(e => e.TimeSelectionId == timeSelectionId)
                ?? throw new Exception("Backstage não foi acessado");

            var availableSlots = freeTimeBackstage.MaxParticipants - markedJoinTimes.Count();

            if (availableSlots > 0 && pendentJoinTimes.Any())
            {
                var x = Math.Min(availableSlots, pendentJoinTimes.Count());
                for (var i = 0; i < x; i++)
                {
                    var randomIndex = new Random().Next(pendentJoinTimes.Length);
                    var jt = pendentJoinTimes.Skip(randomIndex - 1).Take(1).FirstOrDefault();

                    if (jt != null)
                    {
                        OnPostSetAluno(jt.Id);
                    }
                }
            }

            return Redirect("./?event=" + timeSelectionId);
        }

        public IActionResult OnPostTimeSelectionStatusMarcado(
            Guid id,
            Guid PerfilId = default(Guid)
        )
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
#warning Notifica {PerfilId} foi rejeitado
                }

                _context.JoinTimes.UpdateRange(rejecteds);
            }
            ts.Status = StatusTimeSelection.Marcado;
            ts.RoomId = CreatePrivateRoom(ts, ts.PerfilId ?? Guid.Empty);
            _context?.TimeSelections.Update(ts);
            _context?.SaveChanges();
            return Redirect("./?event=" + ts.Id);
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
                Privado = true,
            };
            _context?.Rooms?.Add(room);

            foreach (var t in TagsSelected)
            {
                var tag = new Tag { Titulo = t, RoomRelacao = room.CodigoSala };
                _context?.Tags?.Add(tag);
            }
            return room.Id;
        }

        public async Task<IActionResult> OnPostCancelTimeSelection()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }
            if (IdToCancel == null)
            {
                return new JsonResult(new { consegueCancelar = false });
            }
            var time = _context
                ?.TimeSelections.Where(e => e.Id == Guid.Parse(IdToCancel))
                .FirstOrDefault();
            if (time == null)
            {
                return new JsonResult(new { consegueCancelar = false });
            }
            var feedback = _context
                ?.FeedbackTimeSelections.Where(e => e.TimeSelectionId == time.Id)
                .FirstOrDefault();
            if (feedback != null)
            {
                feedback.DataCancelamento = DateTime.Now;
                _context?.FeedbackTimeSelections.Update(feedback);
            }

            time.Status = StatusTimeSelection.Cancelado;
            _context?.TimeSelections.Update(time);
            _context?.SaveChanges();

            var joins = _context?.JoinTimes.Where(e => e.TimeSelectionId == time.Id).ToList();

            if (joins == null)
            {
                joins = new List<JoinTime> { };
            }
            Notification notification = null;
            foreach (var join in joins)
            {
                if (time != null && time.Tipo == EnumTipoTimeSelection.FreeTime)
                {
                    notification = new Notification
                    {
                        DestinoPerfilId = join.PerfilId ?? Guid.Empty,
                        GeradorPerfilId = UserProfile.Id,
                        TipoNotificacao = TipoNotificacao.MentorCancelaMentoria,
                        DataCriacao = DateTime.Now,
                        Conteudo =
                            $@" cancelou a mentoria {time.TituloTemporario} no dia {time.StartTime:dd/MM/yyyy}",
                        ActionLink = "./",
                    };
                }
                else if (time != null && time.Tipo == EnumTipoTimeSelection.RequestHelp)
                {
                    notification = new Notification
                    {
                        DestinoPerfilId = join.PerfilId ?? Guid.Empty,
                        GeradorPerfilId = UserProfile.Id,
                        TipoNotificacao = TipoNotificacao.PedidoAjudaCancelado,
                        DataCriacao = DateTime.Now,
                        Conteudo =
                            $@" cancelou o pedido de ajuda {time.TituloTemporario} no dia {time.StartTime:dd/MM/yyyy}",
                        ActionLink = "./",
                    };
                }
            }
            if (notification != null)
            {
                await _messagePublisher.PublishAsync(typeof(NotificationsQueue).Name, notification);
            }

            return new JsonResult(new { consegueCancelar = true });
        }

        public async Task<ActionResult> OnPostSaveTime()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }
            if (
                ScheduleTimeSelection == null
                || ScheduleFreeTimeForTimeSelection?.TimeSelectionBackstage == null
            )
            {
                return Redirect("./");
            }

            TimeSelection = new TimeSelection
            {
                PerfilId = UserProfile.Id,
                StartTime = ScheduleTimeSelection.StartTime,
                EndTime = ScheduleTimeSelection.EndTime,
                Variacao = ScheduleTimeSelection.Variacao,
                TituloTemporario = ScheduleTimeSelection.Titulo,
            };

            if (TimeSelection.StartTime < DateTime.Now)
            {
                ModelState.AddModelError("TimeSelection.StartTime", "Data inválida");
                return BadRequest("Não é possível agendar um evento no passado");
            }

            if (TimeSelection.StartTime > DateTime.Now.AddMonths(1))
            {
                ModelState.AddModelError("TimeSelection.EndTime", "Data inválida");
                return BadRequest(
                    "Não é possível agendar um evento com mais de 1 mês de antecedência"
                );
            }

            var isLive = ScheduleTimeSelection.Tipo == EnumTipoTimeSelection.Live;

            if (ScheduleTimeSelection.TagsSelected.Count == 0)
            {
                return new JsonResult(new { });
            }
            if (
                isLive
                && (
                    ScheduleLiveForTimeSelection?.Thumbnail == null
                    || string.IsNullOrEmpty(ScheduleTimeSelection.Titulo)
                )
            )
            {
                return new JsonResult(new { });
            }
            var client = _httpClientFactory.CreateClient("CoreAPI");

            ProcessTimeSelection.ApplyBrazilianTimezone(TimeSelection);

            var timeSelectionList = _context
                .TimeSelections.Where(e =>
                    e.PerfilId == UserProfile.Id && e.Status != StatusTimeSelection.Cancelado
                )
                .ToList();

            timeSelectionList ??= new List<TimeSelection> { };

            bool isValid = TimeSelectionHelper.ValidaSeDataNaoSobrepoem(
                timeSelectionList,
                TimeSelection
            );
            if (!isValid)
            {
                ModelState.AddModelError("TimeSelection.StartTime", "Data já selecionada");
                return new JsonResult(new { consegueSalvar = false });
            }

            TimeSelection.Id = Guid.NewGuid();
            TimeSelection.TituloTemporario = ScheduleTimeSelection.Titulo;
            TimeSelection.Tipo = ScheduleTimeSelection.Tipo;
            TimeSelection.Status = ScheduleTimeSelection.Status;

            var tags = ProcessTimeSelection.ProcessingTags(
                TimeSelection.Id.ToString(),
                ScheduleTimeSelection.TagsSelected
            );

            var feedback = new FeedbackTimeSelection
            {
                Id = Guid.NewGuid(),
                TimeSelectionId = TimeSelection.Id,
                DataDeclaracao = DateTime.Now,
            };

            if (
                ScheduleTimeSelection.Variacao == Variacao.OneToOne
                || ScheduleTimeSelection.Variacao == Variacao.PedirAjuda
            )
            {
                TimeSelectionBackstage.MaxParticipants = 1;
            }
            else if (ScheduleTimeSelection.Variacao == Variacao.CursoOuEvento)
            {
                TimeSelectionBackstage = ScheduleFreeTimeForTimeSelection.TimeSelectionBackstage;
            }

            TimeSelectionBackstage.TimeSelectionId = TimeSelection.Id;

            ApplyCustomFieldsByType(TimeSelection.Tipo);

            if (TimeSelection.Tipo == EnumTipoTimeSelection.RequestHelp)
            {
                await client.PostAsync($"api/helpbackstage/{TimeSelection.Id}/{Descricao}", null);
            }

            if (ScheduleTimeSelection.ImageFile != null)
            {
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(
                    ScheduleTimeSelection.ImageFile.OpenReadStream()
                );
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/*");
                content.Add(fileContent, "file", ScheduleTimeSelection.ImageFile.FileName);
                await client.PostAsync(
                    $"api/helpbackstage/SaveImageFile/{TimeSelection.Id}",
                    content
                );
            }

            _context.FreeTimeBackstages.Add(TimeSelectionBackstage);
            _context.Tags.AddRange(tags);
            _context.TimeSelections.Add(TimeSelection);
            _context.FeedbackTimeSelections.Add(feedback);
            await _context.SaveChangesAsync();

            if (TimeSelection.Tipo == EnumTipoTimeSelection.FreeTime)
            {
                var message = new UpdateTimeSelectionPreviewMessage(TimeSelection.Id);
                await _messagePublisher.PublishAsync(
                    typeof(GenerateOpenGraphImageQueue).Name,
                    message
                );
            }

            var returnData = ProcessTimeSelection.ConverterToTimeSelectionViewModel(
                TimeSelection.StartTime,
                UserProfile.Id,
                TimeSelection,
                Descricao,
                ScheduleTimeSelection.TagsSelected
            );

            return new JsonResult(new { Content = returnData });
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

        public void ApplyCustomFieldsByType(EnumTipoTimeSelection tipo)
        {
            if (TimeSelection == null)
            {
                throw new ArgumentNullException(nameof(tipo), "TimeSelection not found.");
            }
            if (ScheduleTimeSelection == null)
            {
                throw new ArgumentNullException(
                    nameof(tipo),
                    "ScheduleTimeSelection is null or empty."
                );
            }
            if (tipo == EnumTipoTimeSelection.Live)
            {
                Descricao = ScheduleLiveForTimeSelection?.Descricao;
                CreateLiveForTimeSelection();
            }
            else if (tipo == EnumTipoTimeSelection.RequestHelp)
            {
                TimeSelection.TituloTemporario ??=
                    "Tira Dúvida: "
                    + ProcessTimeSelection.GetCategoriaTagsCom3Pontinhos(
                        ScheduleTimeSelection.TagsSelected
                    );
                Descricao = ScheduleTimeSelection.Descricao;
            }
            else
            {
                if (!string.IsNullOrEmpty(ScheduleTimeSelection.Titulo))
                {
                    TimeSelection.TituloTemporario = ScheduleTimeSelection.Titulo;
                }
                else
                {
                    TimeSelection.TituloTemporario =
                        "Mentoria: "
                        + ProcessTimeSelection.GetCategoriaTagsCom3Pontinhos(
                            ScheduleTimeSelection.TagsSelected
                        );
                }
            }
        }

        private void CreateLiveForTimeSelection()
        {
            if (TimeSelection != null)
            {
                var thumb = ResizeImageToBase64(ScheduleLiveForTimeSelection!.Thumbnail!) ?? "";

                var live = new Live
                {
                    PerfilId = UserProfile.Id,
                    StreamId = Guid.NewGuid().ToString(),
                    Titulo = TimeSelection.TituloTemporario,
                    Descricao = ScheduleLiveForTimeSelection.Descricao,
                    LiveEstaAberta = false,
                    DataCriacao = TimeSelection.StartTime,
                    Thumbnail = thumb,
                    UrlAlias = _aliasService
                        .AliasGeneratorAsync(TimeSelection.TituloTemporario)
                        .Result,
                };

                _context?.Lives?.Add(live);

                var backstage = new LiveBackstage()
                {
                    TimeSelectionId = TimeSelection.Id,
                    LiveId = live.Id,
                    Descricao = live.Descricao,
                };
                _context?.LiveBackstages.Add(backstage);

                foreach (var selected in TagsSelected)
                {
                    var tag = new Tag { Titulo = selected, LiveRelacao = live.Id.ToString() };
                    _context?.Tags?.Add(tag);
                }
            }
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

            if (totalVagasOcupada == vagasTotal?.MaxParticipants)
            {
                OnPostTimeSelectionStatusMarcado(tsId, joinPerfilId ?? Guid.Empty);
            }
        }

        public static string? ResizeImageToBase64(IFormFile imageFile)
        {
            try
            {
                using var image = Image.Load(imageFile.OpenReadStream());
                int targetWidth = 1280;
                int targetHeight = 720;

                image.Mutate(x =>
                    x.Resize(
                        new ResizeOptions
                        {
                            Size = new Size(targetWidth, targetHeight),
                            Mode = ResizeMode.Pad,
                        }
                    )
                );

                using var memoryStream = new MemoryStream();
                image.Save(memoryStream, new JpegEncoder());

                byte[] imageBytes = memoryStream.ToArray();
                return "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                // Trate qualquer exceção que possa ocorrer durante o processamento da imagem
                Console.WriteLine("Erro ao redimensionar a imagem: " + ex.Message);
                return null;
            }
        }

        public async Task<IActionResult> OnGetAfterLoadMentores()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }

            var myEvents = GetMyEvents(EnumTipoTimeSelection.FreeTime);
            var filteredTimeSelections = GetFilteredTimeSelections(myEvents);

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

            var timeSelectionGroupByPerfilId = timeSelectionsFinal.GroupBy(e =>
                e.PerfilId.ToString()
            );

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

        public async Task<IActionResult> OnGetAfterLoadRequestedHelp()
        {
            var myEvents = GetMyEvents(EnumTipoTimeSelection.RequestHelp);
            var filteredTimeSelections = GetFilteredTimeSelections(myEvents);
            var timeSelectionsFinal = GetFreeTimeService.CreateRequestedHelpViewModelList(
                filteredTimeSelections
            );
            var timeSelectionGroupByPerfilId = timeSelectionsFinal.GroupBy(e =>
                e.PerfilId.ToString()
            );
            var RequestedHelp =
                await GetFreeTimeService.PrepareViewModelForRenderRequestedHelpBoard(
                    timeSelectionGroupByPerfilId,
                    _context,
                    _httpClientFactory,
                    _perfilWebService,
                    _helpResponseWebService
                );
            return new JsonResult(new { pedidos = RequestedHelp, isLogged = IsAuth });
        }

        public async Task<IActionResult> OnGetAfterLoadRequestedHelpForChannel(Guid id)
        {
            var filteredTimeSelections = GetMyEvents(EnumTipoTimeSelection.RequestHelp)
                .FindAll(e => e.PerfilId == id);
            var timeSelectionsFinal = GetFreeTimeService.CreateRequestedHelpViewModelList(
                filteredTimeSelections
            );
            var timeSelectionGroupByPerfilId = timeSelectionsFinal
                .Where(e => e.StartTime > DateTime.Now)
                .GroupBy(e => e.PerfilId.ToString());
            var RequestedHelp =
                await GetFreeTimeService.PrepareViewModelForRenderRequestedHelpBoard(
                    timeSelectionGroupByPerfilId,
                    _context,
                    _httpClientFactory,
                    _perfilWebService,
                    _helpResponseWebService
                );
            return new JsonResult(new { pedidos = RequestedHelp, isLogged = IsAuth });
        }

        private List<TimeSelection> GetMyEvents(EnumTipoTimeSelection requiredType)
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
            HashSet<TimeSelection> valueSet = new HashSet<TimeSelection>(MyEvents!.Values);

            return GetFreeTimeService.ObtemTimeSelectionsSet(_context, valueSet, requiredType)
                ?? [];
        }

        private List<TimeSelection> GetFilteredTimeSelections(List<TimeSelection> timeSelections)
        {
            var filteredTimeSelections = GetFreeTimeService.FiltraPelosNaoConflitantes(
                timeSelections,
                _context,
                MyEvents ?? []
            );

            return filteredTimeSelections;
        }
    }
}
