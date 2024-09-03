using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Logic;

public interface IPublicationBusinessLogic
{
    Task<List<Publication>> GetPublicationPerfilById(Guid perfilId);
}
