using System.ComponentModel.DataAnnotations;
using APP.Platform.Pages;
using Domain.Entities;

namespace APP.Platform.Pages;

public sealed class _NewPostModalModel
{
    [Required(ErrorMessage = "O link do post é obrigatório.")]
    [RegularExpression(
        @"^(https?:\/\/)?(www\.)?(linkedin\.com|x\.com|dev\.to|tabnews\.com\.br|medium\.com)\/.*$",
        ErrorMessage = "O link deve ser de um dos seguintes sites: linkedin.com, x.com, dev.to, tabnews.com.br, medium.com."
    )]
    public string PostLink { get; set; }
}
