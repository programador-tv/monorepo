using System.Text;
using System.Text.Json;
using ClassLib.Follow.Models.ViewModels;
using Domain.Contracts;
using Domain.Entities;
using Domain.Models.ViewModels;
using Domain.WebServices;
using Infrastructure.Data.Contexts;
using Infrastructure.WebServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Platform.Services;

namespace APP.Platform.Pages;

public sealed class BuscaIndexModel(
    ApplicationDbContext context,
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    Settings settings,
    IFollowService followService,
    IPerfilWebService perfilWebService
) : CustomPageModel(context, httpClientFactory, httpContextAccessor, settings)
{
    private readonly IFollowService followService = followService;

    public List<LiveViewModel> Lives { get; set; } = new();

    [BindProperty]
    public List<PerfilBuscarViewModel> Perfil { get; set; } = new();

    public bool IsFollowing { get; set; }

    public async Task<ActionResult> OnGetAsync(string key)
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("../Perfil");
        }
        if (string.IsNullOrEmpty(key))
        {
            return Redirect("index");
        }

        var search = new Search(_context, _httpClientFactory, _perfilWebService, key);

        var client = _httpClientFactory.CreateClient("CoreAPI");

        using var responseSearchLives = await client.GetAsync($"api/lives/searchLives/{key}");

        var lives = await responseSearchLives.Content.ReadFromJsonAsync<List<Live>>();

        var perfils = search.Perfils.Distinct().ToList();

        var givenPerfilIds = perfils.Select(e => e.Id);
        var remainProfileIds = lives
            .Where(e => !givenPerfilIds.Contains(e.PerfilId))
            .Select(e => e.PerfilId)
            .ToList();

        var perfilsResponse = await perfilWebService.GetAllById(remainProfileIds) ?? new();

        perfils.AddRange(perfilsResponse);

        var previewModel = perfils.Select(e => e.Id).ToList();

        var followersRequest = new FollowersRequest(previewModel);
        var followersRequestJson = JsonSerializer.Serialize(followersRequest);
        var followersContent = new StringContent(
            followersRequestJson,
            Encoding.UTF8,
            "application/json"
        );

        var responseTaskFollow = await client.PostAsync(
            $"api/follow/GetFollowersCount",
            followersContent
        );
        responseTaskFollow.EnsureSuccessStatusCode();

        var followers = await responseTaskFollow.Content.ReadFromJsonAsync<
            List<FollowersCountViewModel>
        >();

        var perfilIds = perfils.Select(e => e.Id);

        var userLoggedFollows = await _context
            .Follows.Where(f =>
                f.FollowerId == UserProfile.Id && perfilIds.Contains(f.FollowingId) && f.Active
            )
            .ToListAsync();

        foreach (var perfil in perfils)
        {
            var isFollowing = userLoggedFollows.Any(e => e.FollowingId == perfil.Id);

            if (perfil == null)
            {
                continue;
            }

            var follower = followers?.FirstOrDefault(e => e.UserId == perfil.Id);
            var followersCount = follower?.Followers ?? 0;

            Perfil.Add(
                new PerfilBuscarViewModel()
                {
                    Id = perfil.Id,
                    Nome = perfil.Nome,
                    UserName = perfil.UserName,
                    Foto = perfil.Foto,
                    Bio = perfil.Bio,
                    Followers = followersCount,
                    isFollowing = isFollowing
                }
            );
        }

        var visualizations = _context
            .Visualizations.GroupBy(v => v.LiveId)
            .Select(g => new { LiveId = g.Key, Count = g.Count() })
            .ToList();

        var visualizationCounts = visualizations.ToDictionary(x => x.LiveId, x => x.Count);

        foreach (var live in lives)
        {
            var perfilCriador = perfils.FirstOrDefault(i => i.Id == live.PerfilId);

            if (perfilCriador != null)
            {
                Lives.Add(
                    new LiveViewModel()
                    {
                        CodigoLive = live.CodigoLive,
                        DataCriacao = live.DataCriacao,
                        Titulo = live.Titulo,
                        Descricao = live.Descricao,
                        Thumbnail = live.Thumbnail,
                        UserNameCriador = perfilCriador.UserName ?? "",
                        NomeCriador = perfilCriador.Nome ?? "Anônimo",
                        FotoCriador = perfilCriador.Foto,
                        QuantidadeDeVisualizacoes = visualizationCounts.ContainsKey(live.Id)
                            ? visualizationCounts[live.Id]
                            : 0,
                        Visibility = live.Visibility,
                        FormatedDuration = live.FormatedDuration,
                    }
                );
            }
        }
        return Page();
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
}
