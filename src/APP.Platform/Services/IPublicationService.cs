using Domain.Contracts;

namespace Platform.Services;

public interface IPublicationService
{

    public Task Add(CreatePublicationRequest request);
   
}
