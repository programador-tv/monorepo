using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Logic;

public sealed class PublicationBusinessLogic(IPublicationRepository _repository) : IPublicationBusinessLogic
{
    public async Task AddPublication(CreatePublicationRequest createPublicationRequest)
    {
        var publication = 
            Publication.Create(createPublicationRequest)
            ?? throw new UriFormatException("A URL fornecida é inválida!");
        
        await _repository.AddAsync(publication);
    }
}