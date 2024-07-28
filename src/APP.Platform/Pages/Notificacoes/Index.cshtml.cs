using Domain.Contracts;
using Domain.Models.ViewModels;
using Domain.WebServices;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace APP.Platform.Pages
{
    public sealed class NotificacoesModel : CustomPageModel
    {
        private new readonly ApplicationDbContext _context;
        private IPerfilWebService _perfilWebService { get; set; }
        private readonly INotificationWebService _notificationWebService;

        public NotificacoesModel(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            Settings settings,
            IPerfilWebService perfilWebService,
            INotificationWebService notificationWebService
        )
            : base(context, httpClientFactory, httpContextAccessor, settings)
        {
            _context = context;
            _perfilWebService = perfilWebService;
            _notificationWebService = notificationWebService;
        }

        public List<NotificationViewModel>? Notifications { get; set; }

        public async Task<ActionResult> OnGet()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("/Perfil");
            }

            List<NotificationItemResponse> notifications =
                await _notificationWebService.GetById(UserProfile.Id) ?? [];

            var profileIds = notifications?.Select(x => x.GeradorPerfilId).ToList() ?? [];

            var profiles = await _perfilWebService.GetAllById(profileIds) ?? [];

            var profilesLegacy = new List<Domain.Entities.Perfil>();

            foreach (var perfil in profiles)
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
                profilesLegacy.Add(perfilLegacy);
            }

            Notifications = new();
            foreach (
                var notification in notifications ?? Enumerable.Empty<NotificationItemResponse>()
            )
            {
                Notifications.Add(
                    new NotificationViewModel
                    {
                        DestinoPerfilId = notification.DestinoPerfilId,
                        GeradorPerfilId = notification.GeradorPerfilId,
                        TipoNotificacao = notification.TipoNotificacao,
                        Vizualizado = notification.Vizualizado,
                        DataCriacao = notification.DataCriacao,
                        Conteudo = notification.Conteudo,
                        ActionLink = notification.ActionLink,
                        SecundaryLink = notification.SecundaryLink,
                        PerfilGerador = profilesLegacy?.FirstOrDefault(x =>
                            x.Id == notification.GeradorPerfilId
                        )
                    }
                );
            }
            CountNotifications = 0;
            return Page();
        }
    }
}
