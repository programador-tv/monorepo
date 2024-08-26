using System.ComponentModel.DataAnnotations;

namespace Domain.Primitives;

public abstract class Entity(Guid id, DateTime createdAt)
{
    [Key]
    public Guid Id { get; protected set; } = id;

    [Timestamp]
    public byte[] Version { get; set; } = new byte[8];
    public DateTime CreatedAt { get; protected set; } = createdAt;
}
