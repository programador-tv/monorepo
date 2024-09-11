using System.Drawing;
using Domain.Entities;
using Domain.Enums;
using Domain.Models.ViewModels;
using Domain.WebServices;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Jpeg;
using tags;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;

namespace APP.Platform.Pages;

public sealed class EditorIndexModel : CustomPageModel
{
    private new readonly ApplicationDbContext _context;
    private new readonly IHttpClientFactory _httpClientFactory;
    private new readonly IPerfilWebService _perfilWebService;
    public SelectList? TagsFront { get; set; }

    [BindProperty]
    public EditorLiveViewModel? Live { get; set; }
    public LiveViewModel? LivePreview { get; set; }

    [BindProperty]
    public List<string>? TagsSelected { get; set; }

    [BindProperty]
    public List<Tag>? LiveTags { get; set; }

    [BindProperty]
    public bool IsUsrCanal { get; set; }

    public Domain.Entities.Perfil? Perfil { get; set; }

    public Dictionary<string, List<string>> RelatioTags { get; set; }

    public EditorIndexModel(
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IPerfilWebService perfilWebService,
        Settings settings
    )
        : base(context, httpClientFactory, httpContextAccessor, settings)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
        _perfilWebService = perfilWebService;

        RelatioTags = DataTags.GetTags();
    }

    public async Task<IActionResult> OnGetAsync(string key)
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("../Perfil");
        }
        try
        {
            var client = _httpClientFactory.CreateClient("CoreAPI");

            using var responseTask = await client.GetAsync($"api/lives/{key}");

            responseTask.EnsureSuccessStatusCode();

            var live = await responseTask.Content.ReadFromJsonAsync<Live>();

            if (live == null)
            {
                return Redirect("../");
            }

            var owner = await _perfilWebService.GetById(live.PerfilId);
           

            if (owner != null && owner.Id != live.PerfilId)
            {
                return Redirect("./?usr=" + UserProfile.UserName);
            }
            IsUsrCanal = true;
            var visualizacoes = _context.Visualizations.Count(i => i.LiveId == live.Id);
            var isTimeSelection = _context
                .LiveBackstages.AsNoTracking()
                .Any(e => e.LiveId == live.Id);
            LivePreview = new LiveViewModel
            {
                Visibility = live.Visibility,
                CodigoLive = live.Id.ToString(),
                Titulo = live.Titulo,
                Descricao = live.Descricao,
                FotoCriador = owner?.Foto,
                NomeCriador = owner?.Nome,
                UserNameCriador = owner?.UserName ?? string.Empty,
                Thumbnail = live.Thumbnail,
                QuantidadeDeVisualizacoes = visualizacoes,
                IsTimeSelection = isTimeSelection,
            };
            Live = new EditorLiveViewModel
            {
                Visibility = live.Visibility,
                Titulo = live.Titulo,
                Descricao = live.Descricao,
            };

            var tags = new List<Tag>();

            if (isTimeSelection)
            {
                var timeSelectionId = _context
                    .LiveBackstages.AsNoTracking()
                    .FirstOrDefault(e => e.LiveId == live.Id)
                    ?.TimeSelectionId.ToString();
                tags = _context.Tags?.Where(e => e.FreeTimeRelacao == timeSelectionId).ToList();
            }
            else
            {
                tags = _context.Tags?.Where(e => e.LiveRelacao == live.Id.ToString()).ToList();
            }

            if (tags == null)
            {
                tags = new List<Tag> { };
            }

            LiveTags = tags;

            TagsSelected = new();
            if (LiveTags != null)
            {
                foreach (var tag in LiveTags)
                {
                    TagsSelected.Add(tag.Titulo ?? string.Empty);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            if (UserProfile.UserName == null)
            {
                return Redirect("../../Identity/Account/Login");
            }
            else
            {
                return Redirect("./?usr=" + UserProfile.UserName);
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string key)
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("../Perfil");
        }

        var client = _httpClientFactory.CreateClient("CoreAPI");

        using var responseTask = await client.GetAsync($"api/lives/{key}");

        responseTask.EnsureSuccessStatusCode();

        var live = await responseTask.Content.ReadFromJsonAsync<Live>();

        if (live == null)
        {
            return Redirect("./?usr=" + UserProfile.UserName);
        }
        if (ModelState.IsValid)
        {
            var timeSelectionId =
                _context.LiveBackstages.FirstOrDefault(e => e.LiveId == live.Id)?.TimeSelectionId
                ?? Guid.Empty;
            var oldTags = _context
                .Tags.Where(e =>
                    e.LiveRelacao == live.Id.ToString()
                    || e.FreeTimeRelacao == timeSelectionId.ToString()
                )
                .ToList();
            if (oldTags != null)
            {
                _context.Tags.RemoveRange(oldTags);
            }

            var Thumbnail =
                Live!.Thumbnail != null ? ResizeImageToBase64(Live.Thumbnail) : live.Thumbnail;

            live.Titulo = Live.Titulo;
            live.Descricao = Live.Descricao;
            live.Visibility = Live.Visibility;
            live.Thumbnail = Thumbnail;
            foreach (var t in TagsSelected!)
            {
                var tag = new Tag
                {
                    Titulo = t,
                    LiveRelacao = live.Id.ToString(),
                    FreeTimeRelacao = timeSelectionId.ToString(),
                };
                _context.Tags.Add(tag);
            }
            _context.Lives.Update(live);

            var liveBackstage = _context.LiveBackstages.FirstOrDefault(e =>
                e.LiveId.ToString() == key
            );

            if (liveBackstage != null && liveBackstage.Id != Guid.Empty)
            {
                var timeSelection = _context.TimeSelections.First(e =>
                    e.Id == liveBackstage.TimeSelectionId
                );

                timeSelection.TituloTemporario = Live.Titulo;
                liveBackstage.Descricao = Live.Descricao;

                _context.TimeSelections.Update(timeSelection);
                _context.LiveBackstages.Update(liveBackstage);
            }

            _context.SaveChanges();
        }
        else
        {
            return await OnGetAsync(live.Id.ToString() ?? string.Empty);
        }
        return new JsonResult(live);
    }

    public static string ResizeImageToBase64(IFormFile imageFile)
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
}
