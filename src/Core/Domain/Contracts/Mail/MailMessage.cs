namespace Domain.Contracts;

public static class MailMessage
{
    public static string MESSAGE = "message";
    public static string NOME_INTERESSADO = "nomeInteressado";
    public static string FOTO_INTERESSADO = "fotoInteressado";

    public static string NOME_MENTOR = "nomeMentor";
    public static string FOTO_MENTOR = "fotoMentor";
    public static string LINK_SALA = "linkMentoria";

    public static string NovoInteressadoContent =
        "Você tem um novo interessado em seu tempo livre!";
    public static string NovoInteressadoSubject = "Novo interessado!";
    public static string InteressadoAceitoContent = "Você foi aceito para uma mentoria!";
    public static string InteressadoAceitoSubject = "Você foi aceito para mentoria!";
}
