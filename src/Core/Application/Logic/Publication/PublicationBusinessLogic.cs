using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Logic;

public sealed class PublicationBusinessLogic(IPublicationRepository _repository)
    : IPublicationBusinessLogic
{
    public async Task<List<Publication>> GetPublicationPerfilById(Guid perfilId) =>
        await _repository.GetPublicationPerfilById(perfilId);
}
