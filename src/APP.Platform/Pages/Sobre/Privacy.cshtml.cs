﻿using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace APP.Platform.Pages
{
    public sealed class PrivacyModel : CustomPageModel
    {
        public PrivacyModel(
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
