using Domain.Primitives;

namespace Domain.Entities;

public sealed class Comment(
    Guid id,
    Guid perfilId,
    Guid liveId,
    string? content,
    DateTime createdAt,
    bool isValid
) : Entity(id, createdAt)
{
    public Guid PerfilId { get; private set; } = perfilId;
    public Guid LiveId { get; private set; } = liveId;
    public string? Content { get; private set; } = content;
    public bool IsValid { get; private set; } = isValid;

    public static Comment Create(Guid perfilId, Guid liveId, string? content)
    {
        return new Comment(Guid.NewGuid(), perfilId, liveId, content, DateTime.Now, false);
    }

    public void Validate()
    {
        IsValid = true;
    }

    public void Invalidate()
    {
        IsValid = false;
    }
}
