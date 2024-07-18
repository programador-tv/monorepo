using APP.Platform.Services;
using ClassLib.Schedule.Rules;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.ViewModels;
using Domain.Utils;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Platform.Services;
using tags;

namespace APP.Platform.Pages.Sala
{
    public sealed class SalaModel : CustomPageModel
    {
        public Dictionary<string, List<string>> RelatioTags { get; set; }

        [BindProperty]
        public List<string> TagsSelected { get; set; }

        [BindProperty]
        public Room? Room { get; set; }

        private readonly IEnsinarService _ensinarService;

        public SalaModel(
            IEnsinarService ensinarService,
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            Settings settings
        )
            : base(context, httpClientFactory, httpContextAccessor, settings)
        {
            _ensinarService = ensinarService;
            RelatioTags = DataTags.GetTags();
            TagsSelected = new();
        }

        public IActionResult OnGet()
        {
            return Redirect("../Index");
        }

        public IActionResult OnPostGeneralRoom()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }

            try
            {
                _ensinarService.SetRoomProfileIdFromUserProfile(Room!, UserProfile);
                _ensinarService.RemoveRoomProfileIdFromModelState(ModelState);

                if (!ModelState.IsValid)
                {
                    return OnGet();
                }

                _ensinarService.CreateAndAddRoomToContext(Room!);
                _ensinarService.AddTagsToRoom(Room!, TagsSelected);

                return Redirect(
                    _meetUrl + "?name=" + Room!.CodigoSala + "&usr=" + UserProfile.UserName
                );
            }
            catch
            {
                return OnGet();
            }
        }
    }
}
