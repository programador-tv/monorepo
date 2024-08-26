using Domain.Primitives;

namespace Domain.Entities;

public sealed class Tag(
    Guid id,
    string titulo,
    string? liveRelacao,
    string? roomRelacao,
    string? freeTimeRelacao,
    string? preLiveRelacao,
    DateTime createdAt
) : Entity(id, createdAt)
{
    public string Titulo { get; private set; } = titulo;
    public string? LiveRelacao { get; private set; } = liveRelacao;
    public string? RoomRelacao { get; private set; } = roomRelacao;
    public string? FreeTimeRelacao { get; private set; } = freeTimeRelacao;
    public string? PreLiveRelacao { get; private set; } = preLiveRelacao;

    public static Tag AddForLive(string titulo, string liveRelacao)
    {
        return new Tag(Guid.NewGuid(), titulo, liveRelacao, null, null, null, DateTime.Now);
    }

    public static Tag AddForRoom(string titulo, string roomRelacao)
    {
        return new Tag(Guid.NewGuid(), titulo, null, roomRelacao, null, null, DateTime.Now);
    }

    public static Tag AddForFreeTime(string titulo, string freeTimeRelacao)
    {
        return new Tag(Guid.NewGuid(), titulo, null, null, freeTimeRelacao, null, DateTime.Now);
    }

    public static Tag AddForPreLive(string titulo, string preLiveRelacao)
    {
        return new Tag(Guid.NewGuid(), titulo, null, null, null, preLiveRelacao, DateTime.Now);
    }
}
