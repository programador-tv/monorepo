using System.Text.RegularExpressions;
using Domain.Contracts;
using Domain.Primitives;

namespace Domain.Entities;

public sealed class Publication(Guid id, Guid perfilId, string link, bool isValid) : Entity(id)
{
    public Guid PerfilId { get; private set; } = perfilId;
    public string Link { get; private set; } = link;
    public bool IsValid { get; private set; } = isValid;

    public static Publication? Create(CreatePublicationRequest createPublicationRequest)
    {
        string? url = createPublicationRequest.Link;
        if (url == null || !UrlIsValid(url))
            return null;

        return new Publication(Guid.NewGuid(), createPublicationRequest.PerfilId, url, true);
    }

    public void NotValid()
    {
        IsValid = false;
    }

    public void Valid()
    {
        IsValid = true;
    }

    public static bool UrlIsValid(string url)
    {
        string[] allowedDomains =
        {
            @"^https:\/\/(www\.)?linkedin\.com(\/.*)?$",
            @"^https:\/\/x\.com(\/.*)?$",
            @"^https:\/\/dev\.to(\/.*)?$",
            @"^https:\/\/tabnews\.com\.br(\/.*)?$",
            @"^https:\/\/medium\.com(\/.*)?$",
        };

        if (!allowedDomains.Any(domain => Regex.IsMatch(url, domain, RegexOptions.IgnoreCase)))
            return false;

        return true;
    }
}
