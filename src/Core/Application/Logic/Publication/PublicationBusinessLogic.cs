using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Logic;

public sealed class PublicationBusinessLogic(IPublicationRepository _repository)
    : IPublicationBusinessLogic
{
    public async Task<List<Publication>> GetPublicationPerfilById(Guid perfilId, int pageNumber)
    {
        var pageSize = 2;
        return await _repository.GetAllAsync(perfilId, pageSize, pageNumber);
    }

    public async Task AddPublication(CreatePublicationRequest createPublicationRequest)
    {
        var publication =
            Publication.Create(createPublicationRequest)
            ?? throw new UriFormatException("A URL fornecida é inválida!");

        await _repository.AddAsync(publication);
    }
}
