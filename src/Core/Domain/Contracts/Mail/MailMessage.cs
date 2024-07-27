namespace Domain.Contracts;

public static class MailMessage
{
    public static string MESSAGE { get; set; } =  "message";
    public static string NOME_INTERESSADO { get; set; } = "nomeInteressado";
    public static string FOTO_INTERESSADO { get; set; } = "fotoInteressado";

    public static string NOME_MENTOR { get; set; } = "nomeMentor";
    public static string FOTO_MENTOR { get; set; } = "fotoMentor";
    public static string LINK_SALA { get; set; } = "linkMentoria";

    public static string NovoInteressadoContent { get; set; } = "Você tem um novo interessado em seu tempo livre!";
    public static string NovoInteressadoSubject { get; set; } = "Novo interessado!";
    public static string InteressadoAceitoContent { get; set; } = "Você foi aceito para uma mentoria!";
    public static string InteressadoAceitoSubject { get; set; } = "Você foi aceito para mentoria!";
}
