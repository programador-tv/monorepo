using System.Linq;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using Background;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Domain.Models.ViewModels;
using Domain.Utils;
using Domain.WebServices;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//
using Platform.Services;
using Queue;

namespace APP.Platform.Pages
{
    public sealed partial class WatchIndexModel : CustomPageModel
    {
        private new readonly ApplicationDbContext _context;
        private readonly IMessagePublisher _messagePublisher;
        private readonly RateLimit _rateLimit;
        private readonly IAliasService _aliasService;
        public Live? Live { get; set; }
        public bool isScheduledLive { get; set; }
        public bool initialScheduledState { get; set; }
        public TimeSelection? TimeSelectionScheduledLive { get; set; }
        public List<CommentViewModel>? Comments { get; set; }

        [BindProperty]
        public string? LiveId { get; set; }

        [BindProperty]
        public Guid IdCommentToDelete { get; set; }
        public Domain.Entities.Perfil? PerfilOwner { get; set; }
        public int LikesCounter { get; set; }
        public bool IsUserLiked { get; set; }
        public IHttpClientFactory _httpClientFactory { get; set; }
        private IPerfilWebService _perfilWebService { get; set; }

        public WatchIndexModel(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IMessagePublisher messagePublisher,
            Settings settings,
            RateLimit rateLimit,
            IAliasService aliasService,
            IPerfilWebService perfilWebService
        )
            : base(context, httpClientFactory, httpContextAccessor, settings)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _perfilWebService = perfilWebService;
            _messagePublisher = messagePublisher;
            _rateLimit = rateLimit;
            _aliasService = aliasService;
        }

        public string GetTempoQuePassouFormatado(DateTime DataCriacao)
        {
            return TimeHelper.GetTempoQuePassouFormatado(DataCriacao);
        }

        public async Task<IActionResult> OnGetAsync(string key)
        {
            var client = _httpClientFactory.CreateClient("CoreAPI");

            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }

            var alias = _aliasService.NormalizeTitle(key);

            using var LiveTask = await client.GetAsync($"api/lives/ByUrl/{alias}");

            if (LiveTask.StatusCode != HttpStatusCode.OK && Guid.TryParse(key, out _))
            {
                var videoEvent = _context.Lives.FirstOrDefault(e => e.Id.ToString() == key);
                if (videoEvent == null)
                {
                    return Redirect("../Index");
                }

                if (videoEvent != null && videoEvent.Titulo != null)
                {
                    videoEvent.UrlAlias = await _aliasService.AliasGeneratorAsync(
                        videoEvent.Titulo
                    );

                    var updateLive = _context.Lives.Update(videoEvent);
                    await _context.SaveChangesAsync();

                    Live = videoEvent;

                    return Redirect($"../Watch/{videoEvent?.UrlAlias}");
                }
            }

            Live = await LiveTask.Content.ReadFromJsonAsync<Live>() ?? new();

            if (Live == null)
            {
                return Redirect("../Index");
            }

            var backstage = _context.LiveBackstages.FirstOrDefault(e => e.LiveId == Live.Id);

            if (backstage != null)
            {
                var ts = _context.TimeSelections.FirstOrDefault(e =>
                    e.Id == backstage.TimeSelectionId
                );
                if (ts != null && ts.StartTime > DateTime.Now)
                {
                    TimeSelectionScheduledLive = ts;
                    isScheduledLive = true;
                }
            }

            LiveId = Live?.Id.ToString() ?? string.Empty;

            AccountLiveView(LiveId);

            var likes = _context
                .Likes.Where(e => e.EntityId.ToString() == LiveId && e.IsLiked)
                .ToList();
            LikesCounter = likes.Count;
            IsUserLiked = likes.Exists(e => e.RelatedUserId == UserProfile.Id);

            var userNotify = await _context.NotifyUserLiveEarlies.FirstOrDefaultAsync(x =>
                x.LiveId == Guid.Parse(LiveId) && x.PerfilId == UserProfile.Id
            );

            if (userNotify != null)
            {
                initialScheduledState = userNotify.Active;
            }
            Comments = new List<CommentViewModel> { };

            DateTime agora = DateTime.Now;

            using var responseTask = await client.GetAsync(
                $"api/comments/getAllByLiveIdAndPerfilId/{LiveId}/{UserProfile.Id}"
            );

            responseTask.EnsureSuccessStatusCode();

            var comments = await responseTask.Content.ReadFromJsonAsync<List<Comment>>();

            var perfilCommentsId = comments.Select(e => e.PerfilId).ToList();

            var commentsAssociatedPerfils =
                await _perfilWebService.GetAllById(perfilCommentsId) ?? new();

            foreach (var item in comments)
            {
                var associatedPerfil = commentsAssociatedPerfils.Find(e => e.Id == item.PerfilId);
                var time = GetTempoQuePassouFormatado(item.DataCriacao);

                var model = new CommentViewModel
                {
                    TempoQuePassou = time,
                    Perfil = associatedPerfil,
                    DataCriacao = item.DataCriacao,
                    Content = item.Content ?? string.Empty,
                    Id = item.Id
                };

                Comments.Add(model);
            }
            var perfilOwnerId = Live?.PerfilId ?? Guid.Empty;

            using var byIdResponse = await client.GetAsync($"api/perfils/" + perfilOwnerId);
            var perfilOwner =
                await byIdResponse.Content.ReadFromJsonAsync<Domain.Entities.Perfil>();

            if (perfilOwner == null)
            {
                return Redirect("../Index");
            }

            PerfilOwner = perfilOwner;
            return Page();
        }

        public async Task<ActionResult> OnGetLivePlusView(Guid Id)
        {
            var client = _httpClientFactory.CreateClient("CoreAPI");

            var lives = _context.Lives.Where(e => e.PerfilId == Id && e.Visibility).ToList();

            lives = lives
                .OrderByDescending(l => _context.Visualizations.Count(v => v.LiveId == l.Id))
                .ToList();

            var LivesEndVideo = new List<LiveViewModel>();

            var takedLives = lives.Take(Math.Min(lives.Count, 3)).ToList();

            var takedPerfilIds = takedLives.Select(e => e.PerfilId).ToList();

            var takedPerfils = await _perfilWebService.GetAllById(takedPerfilIds) ?? new();

            foreach (var live in takedLives)
            {
                var vizualizations = _context.Visualizations.Count(i => i.LiveId == live.Id);
                var owner = takedPerfils.Find(e => e.Id == live.PerfilId);

                LivesEndVideo.Add(
                    new LiveViewModel
                    {
                        CodigoLive = live.Id.ToString(),
                        Titulo = live.Titulo,
                        Descricao = live.Descricao,
                        Thumbnail = live.Thumbnail,
                        Visibility = live.Visibility,
                        NomeCriador = owner?.Nome ?? "Anônimo",
                        UserNameCriador = owner?.UserName ?? string.Empty,
                        FotoCriador = owner?.Foto,
                        QuantidadeDeVisualizacoes = vizualizations,
                        FormatedDuration = live.FormatedDuration,
                        StatusLive = live.StatusLive,
                        UrlAlias = live.UrlAlias,
                    }
                );
            }

            return new JsonResult(LivesEndVideo);
        }

        private void AccountLiveView(string key)
        {
            try
            {
                var remoteAdress =
                    Response.HttpContext.Connection.RemoteIpAddress ?? IPAddress.None;
                string IPV4 = remoteAdress.ToString();

                var live =
                    _context.Lives.Where((e) => e.Id.ToString() == key).FirstOrDefault()
                    ?? throw new Exception("Live não encontrada");

                if (UserProfile != null)
                {
                    var visualization = new Visualization
                    {
                        LiveId = live.Id,
                        PerfilId = UserProfile.Id,
                        IPV4 = IPV4,
                    };
                    _context.Visualizations?.Add(visualization);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task<ActionResult> OnGetComment(string comment, string liveId)
        {
            if (string.IsNullOrEmpty(comment))
            {
                return BadRequest();
            }
            var commentModel = new Comment
            {
                PerfilId = UserProfile.Id,
                LiveId = Guid.Parse(liveId),
                Content = WebUtility.HtmlEncode(comment),
                DataCriacao = DateTime.Now
            };

            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();

            await _messagePublisher.PublishAsync(
                typeof(CommentsQueue).Name,
                commentModel.Id.ToString()
            );

            var fotoUsuario = UserProfile.Foto;
            var nomeUsuario = UserProfile.Nome;
            var dataUsuario = GetTempoQuePassouFormatado(commentModel.DataCriacao);
            return new JsonResult(
                new
                {
                    id = commentModel.Id,
                    perfilId = commentModel.PerfilId,
                    comentario = commentModel.Content,
                    data = dataUsuario,
                    foto = fotoUsuario,
                    nome = nomeUsuario
                }
            );
        }

        public ActionResult OnPostDeleteMessage(string messageId, string liveId)
        {
            if (string.IsNullOrEmpty(messageId) || string.IsNullOrEmpty(liveId))
            {
                return BadRequest();
            }

            var message = _context.ChatMessages.FirstOrDefault(x => x.Id == Guid.Parse(messageId));

            if (message == null)
            {
                return BadRequest();
            }

            if (message.PerfilId != UserProfile.Id)
            {
                return BadRequest();
            }

            message.Valid = false;
            _context.ChatMessages.Update(message);
            _context.SaveChanges();

            // _messagingAdapter.Publish(
            //     "removeChatMessage",
            //     Encoding.UTF8.GetBytes(
            //         JsonSerializer.Serialize(new { LiveId = liveId, MessageId = messageId })
            //     )
            // );
            return new JsonResult(new { LiveId = liveId, MessageId = messageId });
        }

        public ActionResult OnPostMessage(string comment, string liveId)
        {
            if (string.IsNullOrEmpty(comment) || string.IsNullOrEmpty(liveId))
            {
                return BadRequest();
            }

            if (_rateLimit.IsRateLimited(UserProfile.Id.ToString(), liveId))
            {
                return BadRequest("Você está enviando mensagens muito rápido.");
            }

            var formatedDate = GetTempoQuePassouFormatado(DateTime.Now);
            var messageToProcess = new ChatMessageRequestModel
            {
                Id = Guid.NewGuid(),
                PerfilId = UserProfile.Id,
                LiveId = Guid.Parse(liveId),
                Content = WebUtility.HtmlEncode(comment),
                DataCriacao = DateTime.Now,
                Foto = UserProfile.Foto,
                Nome = UserProfile.Nome,
                Data = formatedDate
            };

            var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(messageToProcess));
            // _messagingAdapter.Publish("chatMessage", message);
            return new JsonResult(messageToProcess);
        }

        public async Task<IActionResult> OnPostDeleteMyComment()
        {
            var commentEntitie =
                _context.Comments.FirstOrDefault(x => x.Id == IdCommentToDelete)
                ?? throw new Exception("comentEntitie null");

            _context.Remove(commentEntitie);
            await _context.SaveChangesAsync();

            var live = _context?.Lives?.FirstOrDefault(x => x.Id == commentEntitie.LiveId);
            return new JsonResult(new { });
        }

        public async Task<IActionResult> OnPostActiveNotification()
        {
            var userNotify = await _context.NotifyUserLiveEarlies.FirstOrDefaultAsync(x =>
                x.LiveId == Guid.Parse(LiveId!) && x.PerfilId == UserProfile.Id
            );

            if (userNotify == null)
            {
                userNotify = new NotifyUserLiveEarly
                {
                    LiveId = Guid.Parse(LiveId!),
                    PerfilId = UserProfile.Id,
                    Active = true,
                    hasNotificated = false
                };
                _context.NotifyUserLiveEarlies.Add(userNotify);
            }
            else
            {
                userNotify.Active = !userNotify.Active;
                _context.NotifyUserLiveEarlies.Update(userNotify);
            }
            await _context.SaveChangesAsync();
            return new JsonResult(new { });
        }

        public async Task<IActionResult> OnGetLikeInteration(string entityKey)
        {
            var entityId = Guid.Parse(entityKey);
            if (!IsAuth)
            {
                return new JsonResult(new { });
            }
            Guid userId = UserProfile.Id;

            bool userAlredyLiked = false;

            var client = _httpClientFactory.CreateClient("CoreAPI");

            using var responseTask = await client.GetAsync($"api/like/getLikesByLiveId/{entityId}");

            responseTask.EnsureSuccessStatusCode();

            var likes = await responseTask.Content.ReadFromJsonAsync<List<Like>>() ?? [];

            var relation = likes.Find(e => e.RelatedUserId == userId);

            if (relation != null)
            {
                relation.IsLiked = !relation.IsLiked;
                userAlredyLiked = relation.IsLiked;

                _context.Likes.Update(relation);
            }
            else
            {
                var newLike = new Like
                {
                    EntityId = entityId,
                    RelatedUserId = userId,
                    IsLiked = true,
                };
                userAlredyLiked = true;

                if (_context == null)
                {
                    return BadRequest();
                }

                _context.Likes.Add(newLike);
            }

            await _context.SaveChangesAsync();

            var newCount = _context
                ?.Likes.AsNoTracking()
                .Where(e => e.EntityId == entityId && e.IsLiked)
                .ToList()
                .Count;

            return new JsonResult(new { NewCount = newCount, UserAlredyLiked = userAlredyLiked });
        }

        [GeneratedRegex(@"[^\u0000-\u007F]")]
        private static partial Regex MyRegex();
    }
}
