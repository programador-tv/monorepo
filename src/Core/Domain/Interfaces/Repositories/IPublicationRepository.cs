using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPublicationRepository
    {
        public Task<List<Publication>> GetAllAsync(Guid perfilId);
    }
}
