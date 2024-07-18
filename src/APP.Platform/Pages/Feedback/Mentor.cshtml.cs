using Domain.Entities;
using Domain.Enums;
using Domain.Models.ViewModels;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tags;

namespace APP.Platform.Pages
{
    public sealed class MentorModel : CustomPageModel
    {
        [BindProperty]
        public FeedbackTimeSelection FeedbackTimeSelection { get; set; }
        public List<SelectListItem> SatisfacaoPossivel { get; set; }
        public List<SelectListItem> EstimativaSenioridade { get; set; }

        public MentorModel(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            Settings settings
        )
            : base(context, httpClientFactory, httpContextAccessor, settings)
        {
            FeedbackTimeSelection = new();
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
        }

        public ActionResult OnGet(Guid Id)
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }
            var TimeSelection = _context?.TimeSelections.Where(i => i.Id == Id).FirstOrDefault();

            if (TimeSelection == null)
            {
                return Redirect("Index");
            }
            if (TimeSelection.PerfilId != UserProfile.Id)
            {
                return Redirect("Index");
            }
            FeedbackTimeSelection = new FeedbackTimeSelection();
            FeedbackTimeSelection.TimeSelectionId = TimeSelection.Id;

            return Page();
        }

        public ActionResult OnPostSemAluno()
        {
            if (IsAuthenticatedWithoutProfile())
            {
                return Redirect("../Perfil");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var TimeSelection = _context
                ?.TimeSelections?.Where(i => i.Id == FeedbackTimeSelection.TimeSelectionId)
                .FirstOrDefault();
            if (TimeSelection == null)
            {
                return Redirect("Index");
            }
            if (TimeSelection.PerfilId != UserProfile.Id)
            {
                return Redirect("Index");
            }
            var feedback = _context
                ?.FeedbackTimeSelections.Where(i =>
                    i.TimeSelectionId == FeedbackTimeSelection.TimeSelectionId
                )
                .FirstOrDefault();
            if (feedback == null)
            {
                return Redirect("Index");
            }
            feedback.AvaliadoCompareceu = FeedbackTimeSelection.AvaliadoCompareceu;
            feedback.AvaliadorCompareceu = FeedbackTimeSelection.AvaliadorCompareceu;

            _context?.FeedbackTimeSelections.Update(feedback);

            TimeSelection.Status = StatusTimeSelection.Concluído;
            _context?.TimeSelections.Update(TimeSelection);
            _context?.SaveChanges();
            return Redirect("../Ensinar");
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
            var TimeSelection = _context
                ?.TimeSelections.Where(i => i.Id == FeedbackTimeSelection.TimeSelectionId)
                .FirstOrDefault();
            if (TimeSelection == null)
            {
                return Redirect("Index");
            }
            if (TimeSelection.PerfilId != UserProfile.Id)
            {
                return Redirect("Index");
            }

            var feedback = _context
                ?.FeedbackTimeSelections.Where(i =>
                    i.TimeSelectionId == FeedbackTimeSelection.TimeSelectionId
                )
                .FirstOrDefault();
            if (feedback == null)
            {
                return Redirect("Index");
            }
            feedback.AvaliadoCompareceu = true;
            feedback.ConheciaAvaliadoPreviamente =
                FeedbackTimeSelection.ConheciaAvaliadoPreviamente;
            feedback.EstimativaSalarioAvaliado = FeedbackTimeSelection.EstimativaSalarioAvaliado;
            feedback.EstimativaSenioridadeAvaliado =
                FeedbackTimeSelection.EstimativaSenioridadeAvaliado;
            feedback.SatisfacaoExperiencia = FeedbackTimeSelection.SatisfacaoExperiencia;

            _context?.FeedbackTimeSelections.Update(feedback);

            TimeSelection.Status = StatusTimeSelection.Concluído;
            _context?.TimeSelections.Update(TimeSelection);
            _context?.SaveChanges();
            return Redirect("../Ensinar");
        }
    }
}
