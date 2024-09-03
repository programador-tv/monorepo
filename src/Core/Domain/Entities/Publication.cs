using Domain.Primitives;
using Domain.Contracts;

namespace Domain.Entities;

public sealed class Publication(Guid id, Guid perfilId, string link, bool isValid) : Entity(id)
{
    public Guid PerfilId { get; private set; } = perfilId;
    public string Link { get; private set; } = link;
    public bool IsValid { get; private set; } = isValid;

    public static Publication? Create(CreatePublicationRequest createPublicationRequest)
    {
        if (!UrlIsValid(createPublicationRequest.Link))
            return null;

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

    public static bool UrlIsValid(string url) {
        string[] allowedDomains = new[]
        {
            "linkedin.com",
            "x.com",
            "dev.to",
            "tabnews.com.br",
            "medium.com"
        };

        if (string.IsNullOrWhiteSpace(url))
            return false;
        
        Uri.TryCreate(url, UriKind.Absolute, out var uriResult);
        
        if (
            uriResult == null
            || uriResult.Scheme != Uri.UriSchemeHttps
            || !allowedDomains.Any(domain => uriResult.Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
            )
            return false;
            
        return true;
    }
}
