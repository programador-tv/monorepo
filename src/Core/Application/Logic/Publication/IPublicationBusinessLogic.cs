using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Repositories;

//using Domain.Entities;

namespace Application.Logic;

public interface IPublicationBusinessLogic
{
    Task<List<Publication>> GetPublicationPerfilById(Guid perfilId);
    public Task AddPublication(CreatePublicationRequest createPublicationRequest);
}
