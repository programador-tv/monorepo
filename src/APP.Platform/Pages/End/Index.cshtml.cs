using Domain.Entities;
using Domain.Enums;
using Domain.Models.ViewModels;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace APP.Platform.Pages;

public sealed class EndIndexModel : CustomPageModel
{
    private new readonly ApplicationDbContext _context;
    public IList<RoomViewModel>? Room { get; set; }

    [BindProperty]
    public bool IsUsrCanal { get; set; }
    public Domain.Entities.Perfil? Perfil { get; set; }

    public EndIndexModel(
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        Settings settings
    )
        : base(context, httpClientFactory, httpContextAccessor, settings)
    {
        _context = context;
    }

    public IActionResult OnGetAsync(string key)
    {
        if (IsAuthenticatedWithoutProfile())
        {
            return Redirect("../Perfil");
        }

        var live = _context
            ?.Lives?.Where(e => e.Id.ToString() == key && e.PerfilId == UserProfile.Id)
            .FirstOrDefault();
        if (live != null)
        {
            live.LiveEstaAberta = false;
            live.StatusLive = StatusLive.Encerrada;
            _context?.Lives?.Update(live);
            _context?.SaveChanges();

            return Redirect("/canal/editor?key=" + key);
        }
        else
        {
            var room = _context
                ?.Rooms?.Where(e => e.CodigoSala == key && e.PerfilId == UserProfile.Id)
                .FirstOrDefault();
            if (live != null && room != null)
            {
                room.EstaAberto = false;
                _context?.Rooms?.Update(room);
                _context?.SaveChanges();
            }
        }
        return Redirect("../");
    }
}
