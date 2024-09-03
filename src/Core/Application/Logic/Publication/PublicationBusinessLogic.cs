using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Logic;

public sealed class PublicationBusinessLogic(IPublicationRepository _repository) : IPublicationBusinessLogic
{
    public async Task AddPublication(CreatePublicationRequest createPublicationRequest)
    {
        var publication = Publication.Create(createPublicationRequest);

        await _repository.AddAsync(publication);
    }
}