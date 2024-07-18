using Domain.Enumerables;
using Domain.Primitives;

namespace Domain.Entities;

public sealed class Room(
    Guid id,
    Guid perfilId,
    string? codigoSala,
    DateTime dataCriacao,
    DateTime? ultimaAtualizacao,
    bool estaAberto,
    TipoSalas tipoSala,
    string? titulo,
    bool privado
) : Entity(id)
{
    public Guid PerfilId { get; private set; } = perfilId;
    public string? CodigoSala { get; private set; } = codigoSala;
    public DateTime DataCriacao { get; private set; } = dataCriacao;
    public DateTime? UltimaAtualizacao { get; private set; } = ultimaAtualizacao;
    public bool EstaAberto { get; private set; } = estaAberto;
    public TipoSalas TipoSala { get; private set; } = tipoSala;
    public string? Titulo { get; private set; } = titulo;
    public bool Privado { get; private set; } = privado;

    public static Room Create(
        Guid perfilId,
        string? codigoSala,
        TipoSalas tipoSala,
        string? titulo,
        bool privado
    )
    {
        return new Room(
            Guid.NewGuid(),
            perfilId,
            codigoSala,
            DateTime.Now,
            DateTime.Now,
            true,
            tipoSala,
            titulo,
            privado
        );
    }

    public void FecharSala()
    {
        EstaAberto = false;
    }

    public void AbrirSala()
    {
        EstaAberto = true;
    }

    public void MantemAberta()
    {
        UltimaAtualizacao = DateTime.Now;
    }
}
