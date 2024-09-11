using Domain.Primitives;

namespace Domain.Entities;

public sealed class Project(Guid id, Guid perfilId, string link, bool isValid) : Entity(id)
{
    public Guid PerfilId { get; private set; } = perfilId;
    public string Link { get; private set; } = link;
    public bool IsValid { get; private set; } = isValid;
    public string Title { get; set; }
    public string Image { get; set; }

    public static Project Create(Guid perfilId, string link)
    {
        return new Project(Guid.NewGuid(), perfilId, link, true);
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
