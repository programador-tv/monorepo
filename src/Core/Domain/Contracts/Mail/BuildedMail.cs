using Domain.Contracts;
using Domain.Entities;

namespace Contracts;

public sealed class BuildedEmail
{
    public string Destination { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public EmailType Type { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = [];

    public static BuildedEmail NovoInteressadoMentoria(
        Perfil destino,
        Perfil gerador,
        string? actionLink
    )
    {
        return new BuildedEmail
        {
            Destination = destino.Email,
            Name = destino.Nome ?? destino.UserName ?? string.Empty,
            Subject = MailMessage.NovoInteressadoContent,
            Message = string.Empty,
            Type = EmailType.NovoInteressadoMentoria,
            Parameters = new()
            {
                { MailMessage.MESSAGE, MailMessage.NovoInteressadoContent },
                { MailMessage.NOME_INTERESSADO, gerador.Nome ?? gerador.UserName ?? string.Empty },
                { MailMessage.FOTO_INTERESSADO, gerador.Foto ?? string.Empty },
                { MailMessage.LINK_SALA, actionLink ?? string.Empty }
            }
        };
    }

    public static BuildedEmail AlunoAceitoNaMentoria(
        Perfil destino,
        Perfil gerador,
        string? actionLink
    )
    {
        return new BuildedEmail
        {
            Destination = destino.Email,
            Name = destino.Nome ?? destino.UserName ?? string.Empty,
            Subject = MailMessage.InteressadoAceitoSubject,
            Message = string.Empty,
            Type = EmailType.AlunoAceitoNaMentoria,
            Parameters = new()
            {
                { MailMessage.MESSAGE, MailMessage.InteressadoAceitoContent },
                { MailMessage.NOME_MENTOR, gerador.Nome ?? gerador.UserName ?? string.Empty },
                { MailMessage.FOTO_MENTOR, gerador.Foto ?? string.Empty },
                { MailMessage.LINK_SALA, actionLink ?? string.Empty }
            }
        };
    }
}
