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
    public sealed class NotificacoesModel(
        ApplicationDbContext _context,
        IHttpClientFactory _httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        Settings settings,
        IPerfilWebService _perfilWebService,
        INotificationWebService _notificationWebService
        ) : CustomPageModel(_context, _httpClientFactory, httpContextAccessor, settings)
    {
        public List<NotificationViewModel>? Notifications { get; set; }

        public async Task<ActionResult> OnGet()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("/Perfil");
            }

            var notifications = await _notificationWebService.GetById(UserProfile.Id) ?? [];


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
