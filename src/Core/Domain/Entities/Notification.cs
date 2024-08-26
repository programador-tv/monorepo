using Contracts;
using Domain.Enumerables;
using Domain.Primitives;

namespace Domain.Entities;

public class Notification(
    Guid id,
    Guid destinoPerfilId,
    Guid geradorPerfilId,
    TipoNotificacao tipoNotificacao,
    bool vizualizado,
    DateTime createdAt,
    string? conteudo,
    string? actionLink,
    string? secundaryLink
) : Entity(id, createdAt)
{
    public Guid DestinoPerfilId { get; private set; } = destinoPerfilId;
    public Guid GeradorPerfilId { get; private set; } = geradorPerfilId;
    public TipoNotificacao TipoNotificacao { get; private set; } = tipoNotificacao;
    public bool Vizualizado { get; private set; } = vizualizado;
    public string? Conteudo { get; private set; } = conteudo;
    public string? ActionLink { get; private set; } = actionLink;
    public string? SecundaryLink { get; private set; } = secundaryLink;

    public static Notification Create(
        Guid destinoPerfilId,
        Guid geradorPerfilId,
        TipoNotificacao tipoNotificacao,
        string? conteudo,
        string? actionLink,
        string? secundaryLink
    )
    {
        return new Notification(
            Guid.NewGuid(),
            destinoPerfilId,
            geradorPerfilId,
            tipoNotificacao,
            false,
            DateTime.Now,
            conteudo,
            actionLink,
            secundaryLink
        );
    }

    public void Visualizar()
    {
        Vizualizado = true;
    }

    public BuildedEmail? BuildEmail(Perfil destino, Perfil gerador)
    {
        BuildedEmail? mailTo = null;
        switch (TipoNotificacao)
        {
            case TipoNotificacao.NovoInteressadoMentoria:
                mailTo = BuildedEmail.NovoInteressadoMentoria(destino, gerador, ActionLink);
                break;
            case TipoNotificacao.AlunoAceitoNaMentoria:
                mailTo = BuildedEmail.AlunoAceitoNaMentoria(destino, gerador, ActionLink);
                break;
        }

        return mailTo;
    }
}
