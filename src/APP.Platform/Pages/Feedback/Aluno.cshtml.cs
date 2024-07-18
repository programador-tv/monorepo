using Domain.Entities;
using Domain.Enums;
using Domain.Models.ViewModels;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tags;

namespace APP.Platform.Pages
{
    public sealed class AlunoModel : CustomPageModel
    {
        private new readonly ApplicationDbContext _context;

        [BindProperty]
        public FeedbackJoinTime FeedbackJoinTime { get; set; }
        public List<SelectListItem> SatisfacaoPossivel { get; set; }

        public List<SelectListItem> EstimativaSenioridade { get; set; }

        public AlunoModel(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            Settings settings
        )
            : base(context, httpClientFactory, httpContextAccessor, settings)
        {
            FeedbackJoinTime = new();
            var satisfacaoExperiencia = Enum.GetValues(typeof(SatisfacaoExperiencia))
                .Cast<SatisfacaoExperiencia>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = char.ConvertFromUtf32((int)e),
                });
            var estimativaSenioridade = Enum.GetValues(typeof(EstimativaSenioridade))
                .Cast<EstimativaSenioridade>()
                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString(), });

            SatisfacaoPossivel = satisfacaoExperiencia.ToList();
            EstimativaSenioridade = estimativaSenioridade.ToList();
            _context = context;
        }

        public ActionResult OnGet(Guid Id)
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }
            var joinTime = _context?.JoinTimes.Where(i => i.Id == Id).FirstOrDefault();

            if (joinTime == null)
            {
                return Redirect("Index");
            }
            if (joinTime.PerfilId != UserProfile.Id)
            {
                return Redirect("Index");
            }
            FeedbackJoinTime = new FeedbackJoinTime();
            FeedbackJoinTime.JoinTimeId = joinTime.Id;

            return Page();
        }

        public ActionResult OnPostSemMentor()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var joinTime = _context
                ?.JoinTimes.Where(i => i.Id == FeedbackJoinTime.JoinTimeId)
                .FirstOrDefault();
            if (joinTime == null)
            {
                return Redirect("Index");
            }
            if (joinTime.PerfilId != UserProfile.Id)
            {
                return Redirect("Index");
            }
            var feedback = _context
                ?.FeedbackJoinTimes.Where(i => i.JoinTimeId == FeedbackJoinTime.JoinTimeId)
                .FirstOrDefault();
            if (feedback == null)
            {
                return Redirect("Index");
            }
            feedback.AvaliadoCompareceu = FeedbackJoinTime.AvaliadoCompareceu;
            feedback.AvaliadorCompareceu = FeedbackJoinTime.AvaliadorCompareceu;

            _context?.FeedbackJoinTimes.Update(feedback);

            joinTime.StatusJoinTime = StatusJoinTime.Concluído;
            _context?.JoinTimes.Update(joinTime);
            _context?.SaveChanges();
            return Redirect("../Aprender");
        }

        public ActionResult OnPost()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var joinTime = _context
                ?.JoinTimes.Where(i => i.Id == FeedbackJoinTime.JoinTimeId)
                .FirstOrDefault();
            if (joinTime == null)
            {
                return Redirect("Index");
            }
            if (joinTime.PerfilId != UserProfile.Id)
            {
                return Redirect("Index");
            }
            var feedback = _context
                ?.FeedbackJoinTimes.Where(i => i.JoinTimeId == FeedbackJoinTime.JoinTimeId)
                .FirstOrDefault();
            if (feedback == null)
            {
                return Redirect("Index");
            }
            feedback.AvaliadoCompareceu = true;
            feedback.ConheciaAvaliadoPreviamente = FeedbackJoinTime.ConheciaAvaliadoPreviamente;
            feedback.EstimativaSalarioAvaliado = FeedbackJoinTime.EstimativaSalarioAvaliado;
            feedback.EstimativaSenioridadeAvaliado = FeedbackJoinTime.EstimativaSenioridadeAvaliado;
            feedback.SatisfacaoExperiencia = FeedbackJoinTime.SatisfacaoExperiencia;

            _context?.FeedbackJoinTimes.Update(feedback);

            joinTime.StatusJoinTime = StatusJoinTime.Concluído;
            _context?.JoinTimes.Update(joinTime);
            _context?.SaveChanges();
            return Redirect("../Aprender");
        }
    }
}
