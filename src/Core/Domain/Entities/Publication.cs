using Domain.Primitives;
using Domain.Contracts;

namespace Domain.Entities;

public sealed class Publication(Guid id, Guid perfilId, string link, bool isValid) : Entity(id)
{
    public Guid PerfilId { get; private set; } = perfilId;
    public string Link { get; private set; } = link;
    public bool IsValid { get; private set; } = isValid;

    public static Publication Create(CreatePublicationRequest createPublicationRequest)
    {
        return new Publication(
            Guid.NewGuid(),
            createPublicationRequest.PerfilId,
            createPublicationRequest.Link,
            true
        );
    }

    public void NotValid()
    {
        IsValid = false;
    }

    public void Valid()
    {
        IsValid = true;
    }
}
