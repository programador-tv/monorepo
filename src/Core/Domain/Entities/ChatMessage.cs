using Domain.Primitives;

namespace Domain.Entities
{
    public sealed class ChatMessage(
        Guid id,
        Guid perfilId,
        Guid liveId,
        string? content,
        DateTime dataCriacao,
        bool valid
    ) : Entity(id)
    {
        public Guid PerfilId { get; private set; } = perfilId;
        public Guid LiveId { get; private set; } = liveId;
        public string? Content { get; private set; } = content;
        public DateTime DataCriacao { get; private set; } = dataCriacao;
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
