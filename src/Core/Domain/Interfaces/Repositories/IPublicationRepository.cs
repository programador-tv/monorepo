using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPublicationRepository
    {
        public Task AddAsync(Publication publication);
    }
}
