using System.ComponentModel.DataAnnotations;

namespace Domain.Primitives;

public abstract class Entity
{
    protected Entity(Guid id) => Id = id;

    [Key]
    public Guid Id { get; protected set; }

    [Timestamp]
    public byte[] Version { get; set; }
}
