using Domain.Entities;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RabbitMQ.Client;

namespace APP.Platform.Pages;

public class CustomPageModel : PageModel
{
    private Domain.Entities.Perfil Profile = new();
    public Domain.Entities.Perfil UserProfile
    {
        get { return Profile; }
    }
    public bool IsAuth { get; set; }
    public int CountNotifications { get; set; }
    protected readonly ApplicationDbContext _context;
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    public Settings _settings;
    protected readonly string _meetUrl;

    public CustomPageModel(
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        Settings settings
    )
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings;
        _context = context;
        _httpContextAccessor = httpContextAccessor;

        SetProfileIfExistis().Wait();
        SetCountNotifications();
        _meetUrl = settings.MEET_URL;
    }

    public new HttpContext? HttpContext => _httpContextAccessor.HttpContext;

    private void SetCountNotifications()
    {
        if (Profile == null)
            return;
        CountNotifications = _context.Notifications.Count(x =>
            x.DestinoPerfilId == Profile.Id && !x.Vizualizado
        );
    }

    private async Task SetProfileIfExistis()
    {
        try
        {
            var auth = HttpContext!.User.Claims.FirstOrDefault();
            if (auth == null)
                return;
            IsAuth = true;
            var identityId = auth.Value;

            var client = _httpClientFactory.CreateClient("CoreAPI");
            using var responseTask = await client.GetAsync("api/perfils/ByToken/" + identityId);

            var profile = await responseTask.Content.ReadFromJsonAsync<Domain.Entities.Perfil>();

            if (profile == null)
                return;
            Profile = profile;
        }
        catch
        {
            Console.WriteLine("Erro ao buscar perfil");
        }
    }

    public bool IsAuthenticatedWithoutProfile()
    {
        return IsAuth && UserProfile.Token == null;
    }
}
