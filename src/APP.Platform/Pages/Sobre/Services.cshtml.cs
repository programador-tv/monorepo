using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace APP.Platform.Pages
{
    public sealed class ServicesModel : CustomPageModel
    {
        public ServicesModel(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            Settings settings
        )
            : base(context, httpClientFactory, httpContextAccessor, settings) { }

        public ActionResult OnGet()
        {
            return Page();
        }
    }
}
