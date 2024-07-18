using System.Text;
using System.Text.Json;
using Domain.Contracts;
using Domain.Models.ViewModels;
using Domain.WebServices;
using Infrastructure.Data.Contexts;
using Infrastructure.WebServices;
using Microsoft.AspNetCore.Mvc;

namespace APP.Platform.Pages
{
    public sealed class NotificacoesModel : CustomPageModel
    {
        private new readonly ApplicationDbContext _context;
        private new readonly IHttpClientFactory _httpClientFactory;
        private IPerfilWebService _perfilWebService { get; set; }

        public NotificacoesModel(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            Settings settings,
            IPerfilWebService perfilWebService
        )
            : base(context, httpClientFactory, httpContextAccessor, settings)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
            _perfilWebService = perfilWebService;
        }

        public List<NotificationViewModel>? Notifications { get; set; }

        public async Task<ActionResult> OnGet()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("/Perfil");
            }

            var client = _httpClientFactory.CreateClient("CoreAPI");
            using var notificationResponse = await client.GetAsync(
                $"api/notifications/{UserProfile.Id}"
            );

            var notifications = await notificationResponse.Content.ReadFromJsonAsync<
                List<NotificationItemResponse>
            >();
            // var notifications = JsonSerializer.Deserialize<List<NotificationItemResponse>>(
            //     responseTaskNotification
            // );

            if (notifications == null)
            {
                notifications = new List<NotificationItemResponse> { };
            }

            var profileIds = notifications?.Select(x => x.GeradorPerfilId).ToList() ?? new();

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
