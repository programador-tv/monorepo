﻿using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Background;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.Request;
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
        IMessagePublisher messagePublisher,
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

            var client = _httpClientFactory.CreateClient("CoreAPI");
            using var byIdResponse = await client.GetAsync(
                $"api/perfils/ByToken/" + User.Claims.ToArray()[0].Value
            );

            if (!byIdResponse.IsSuccessStatusCode)
            {
                using var byUsernameResponse = await client.GetAsync(
                    $"api/perfils/ByUsername/" + Perfil.UserName
                );

                if (byUsernameResponse.IsSuccessStatusCode)
                {
                    UsernameExist = true;
                    return OnGet();
                }

                var foto = string.Empty;
                if (Perfil.Foto != null)
                {
                    // foto = SaveFoto(Perfil.Foto);
                }

                var _perfil = new Domain.Entities.Perfil()
                {
                    Nome = Perfil.Nome,
                    UserName = Perfil.UserName,
                    Foto = string.Empty,
                    Token = User.Claims.ToArray()[0].Value,
                    Email = User.Claims.ToArray()[1].Value,
                    Linkedin = Perfil.Linkedin,
                    GitHub = Perfil.GitHub,
                    Bio = Perfil.Bio,
                    Descricao = Perfil.Descricao,
                    Experiencia = Perfil.Experiencia,
                };
                UsernameExist = false;

                var json = JsonSerializer.Serialize(_perfil);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var responseTask = await client.PostAsync($"api/perfils", content);
                if (!responseTask.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }
                var notification = new Notification
                {
                    DestinoPerfilId = _perfil.Id,
                    GeradorPerfilId = _perfil.Id,
                    TipoNotificacao = TipoNotificacao.FinalizouCadastro,
                    DataCriacao = DateTime.Now,
                    Conteudo =
                        $@"
                        Obrigado por finalizar seu cadastro,
                        agora você pode criar e participar de salas de estudo e
                        compartilhar seus conhecimentos ao vivo. Saiba mais sobre o projeto clicando em ver
                    ",
                    ActionLink = "/Sobre",
                };

                await messagePublisher.PublishAsync(typeof(NotificationsQueue).Name, notification);

                if (Perfil.Foto != null)
                {
                    using var form = new MultipartFormDataContent();

                    var stream = Perfil.Foto.OpenReadStream();
                    using var streamContent = new StreamContent(stream);

                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                        Perfil.Foto.ContentType
                    );

                    form.Add(streamContent, "file", Path.GetFileName(Perfil.Foto.FileName));

                    var response = await client.PutAsync(
                        "api/perfils/UpdateFoto/" + _perfil.Id,
                        form
                    );
                }

                return RedirectToPage("../Index");
            }
            else
            {
                var _perfil =
                    await byIdResponse.Content.ReadFromJsonAsync<Domain.Entities.Perfil>();

                using var byUsernameResponse = await client.GetAsync(
                    $"api/perfils/ByUsername/" + Perfil.UserName
                );

                if (_perfil != null)
                {
                    if (byUsernameResponse.IsSuccessStatusCode)
                    {
                        var perfilExist =
                            await byUsernameResponse.Content.ReadFromJsonAsync<Domain.Entities.Perfil>();

                        if (perfilExist?.Token != _perfil.Token)
                        {
                            UsernameExist = true;
                            return OnGet();
                        }
                    }

                    _perfil.Nome = Perfil.Nome;
                    _perfil.UserName = Perfil.UserName;
                    _perfil.Linkedin = Perfil.Linkedin;
                    _perfil.GitHub = Perfil.GitHub;
                    _perfil.Bio = Perfil.Bio;
                    _perfil.Descricao = Perfil.Descricao;
                    _perfil.Experiencia = Perfil.Experiencia;

                    var json = JsonSerializer.Serialize(_perfil);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    using var responseTask = await client.PutAsync($"api/perfils", content);

                    if (responseTask.IsSuccessStatusCode)
                    {
                        if (Perfil.Foto != null)
                        {
                            using var form = new MultipartFormDataContent();

                            var stream = Perfil.Foto.OpenReadStream();
                            using var streamContent = new StreamContent(stream);

                            streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                                Perfil.Foto.ContentType
                            );

                            form.Add(streamContent, "file", Path.GetFileName(Perfil.Foto.FileName));

                            var response = await client.PutAsync(
                                "api/perfils/UpdateFoto/" + _perfil.Id,
                                form
                            );
                        }
                    }
                }
                return new JsonResult(new { });
            }
        }
    }
}
