using Domain.Primitives;

namespace Domain.Entities;

public sealed class Presentes(
    Guid id,
    Guid roomId,
    Guid perfilId,
    bool estaPresente,
    string? nome,
    string? foto,
    DateTime dataEntrou,
    DateTime ultimaAtualizacao,
    DateTime createdAt
) : Entity(id, createdAt)
{
    public Guid RoomId { get; private set; } = roomId;
    public Guid PerfilId { get; private set; } = perfilId;
    public bool EstaPresente { get; private set; } = estaPresente;
    public string? Nome { get; private set; } = nome;
    public string? Foto { get; private set; } = foto;
    public DateTime DataEntrou { get; private set; } = dataEntrou;
    public DateTime UltimaAtualizacao { get; private set; } = ultimaAtualizacao;

    public static Presentes Create(Guid roomId, Guid perfilId, string? nome, string? foto)
    {
        return new Presentes(
            Guid.NewGuid(),
            roomId,
            perfilId,
            true,
            nome,
            foto,
            DateTime.Now,
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
