using Domain.Primitives;

namespace Domain.Entities
{
    public sealed class ChatMessage(
        Guid id,
        Guid perfilId,
        Guid liveId,
        string? content,
        DateTime createdAt,
        bool valid
    ) : Entity(id, createdAt)
    {
        public Guid PerfilId { get; private set; } = perfilId;
        public Guid LiveId { get; private set; } = liveId;
        public string? Content { get; private set; } = content;
        public bool Valid { get; private set; } = valid;

        public static ChatMessage Create(Guid perfilId, Guid liveId, string? content)
        {
            return new ChatMessage(Guid.NewGuid(), perfilId, liveId, content, DateTime.Now, false);
        }

        public void Validate()
        {
            Valid = true;
        }

        public void Invalidate()
        {
            Valid = false;
        }
    }
}
