namespace Domain.Contracts;

public static class MailMessage
{
    public static readonly string MESSAGE = "message";
    public static readonly string NOME_INTERESSADO = "nomeInteressado";
    public static readonly string FOTO_INTERESSADO = "fotoInteressado";

    public static readonly string NOME_MENTOR = "nomeMentor";
    public static readonly string FOTO_MENTOR = "fotoMentor";
    public static readonly string LINK_SALA = "linkMentoria";

    public static readonly string NovoInteressadoContent =
        "Você tem um novo interessado em seu tempo livre!";
    public static readonly string NovoInteressadoSubject = "Novo interessado!";
    public static readonly string InteressadoAceitoContent = "Você foi aceito para uma mentoria!";
    public static readonly string InteressadoAceitoSubject = "Você foi aceito para mentoria!";
}
