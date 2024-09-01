using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Background;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.Request;
using Domain.WebServices;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Queue;

namespace APP.Platform.Pages
{
    public sealed class PerfilModel(
        IWebHostEnvironment environment,
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IPerfilWebService perfilWebService,
        Settings settings
    ) : CustomPageModel(context, httpClientFactory, httpContextAccessor, settings)
    {
        [BindProperty]
        public bool HasPefil { get; set; }

        [BindProperty]
        public bool UsernameExist { get; set; }

        [BindProperty]
        public PerfilViewModel? Perfil { get; set; }

        public IWebHostEnvironment Environment { get; } = environment;

        public IActionResult OnGet()
        {
            Perfil = new();
            if (UserProfile != null)
            {
                HasPefil = true;
                Perfil.Nome = UserProfile.Nome;
                Perfil.UserName = UserProfile.UserName;
                Perfil.Linkedin = UserProfile.Linkedin;
                Perfil.GitHub = UserProfile.GitHub;
                Perfil.Bio = UserProfile.Bio;
                Perfil.Descricao = UserProfile.Descricao;
                Perfil.Experiencia = UserProfile.Experiencia;
            }
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return OnGet();
            }
            Perfil!.UserName = Perfil.UserName?.Trim();

            if (string.IsNullOrEmpty(Perfil.UserName))
            {
                return OnGet();
            }

            var createOrUpdatePerfilRequest = new CreateOrUpdatePerfilRequest(
                Nome: Perfil.Nome,
                Token: User.Claims.ToArray()[0].Value,
                UserName: Perfil.UserName,
                Linkedin: Perfil.Linkedin,
                GitHub: Perfil.GitHub,
                Bio: Perfil.Bio,
                Email: User.Claims.ToArray()[1].Value,
                Descricao: Perfil.Descricao,
                Experiencia: (Domain.Enumerables.ExperienceLevel)Perfil.Experiencia
            );

            var result = await perfilWebService.TryCreateOrUpdate(createOrUpdatePerfilRequest);

            var perfilId = Guid.Empty;

            if (
                result.status == StatusCreateOrUpdatePerfil.PerfilCreated
                || result.status == StatusCreateOrUpdatePerfil.PerfilUpdated
            )
            {
                perfilId = result.Id;
            }
            else if (result.status == StatusCreateOrUpdatePerfil.UsernameAlreadyInUse)
            {
                UsernameExist = true;
                return OnGet();
            }

            if (Perfil.Foto != null)
            {
                using var form = new MultipartFormDataContent();

                var stream = Perfil.Foto.OpenReadStream();
                using var streamContent = new StreamContent(stream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                    Perfil.Foto.ContentType
                );

                form.Add(streamContent, "file", Path.GetFileName(Perfil.Foto.FileName));

                var client = _httpClientFactory.CreateClient("CoreAPI");

                var response = await client.PutAsync("api/perfils/UpdateFoto/" + perfilId, form);
            }
            return RedirectToPage("../Index");
        }
    }
}
