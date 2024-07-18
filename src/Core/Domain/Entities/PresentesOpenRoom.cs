using Domain.Primitives;

namespace Domain.Entities;

public sealed class PresentesOpenRoom(
    Guid id,
    Guid perfilId,
    bool estaPresente,
    string? nome,
    string? foto,
    DateTime dataEntrou,
    DateTime ultimaAtualizacao
) : Entity(id)
{
    public Guid PerfilId { get; private set; } = perfilId;
    public bool EstaPresente { get; private set; } = estaPresente;
    public string? Nome { get; private set; } = nome;
    public string? Foto { get; private set; } = foto;
    public DateTime DataEntrou { get; private set; } = dataEntrou;
    public DateTime UltimaAtualizacao { get; private set; } = ultimaAtualizacao;

    public static PresentesOpenRoom Create(Guid perfilId, string? nome, string? foto)
    {
        return new PresentesOpenRoom(
            Guid.NewGuid(),
            perfilId,
            true,
            nome,
            foto,
            DateTime.Now,
            DateTime.Now
        );
    }

    public void EstaNaSala()
    {
        EstaPresente = true;
        UltimaAtualizacao = DateTime.Now;
    }

    public void SaiuDaSala()
    {
        EstaPresente = false;
    }
}
