using Domain.Contracts;
using Domain.WebServices;

namespace Platform.Services;

public class PublicationService(IPublicationWebService _publicationWebService) : IPublicationService
{
    public async Task Add(CreatePublicationRequest request)
    {
        try
        {
            await _publicationWebService.Add(request);
        }
        catch
        {
            throw;
        }
    }
}
