using Domain.Contracts;

//using Domain.Entities;

namespace Application.Logic;

public interface IPublicationBusinessLogic
{
    public Task AddPublication(CreatePublicationRequest createPublicationRequest);
}
