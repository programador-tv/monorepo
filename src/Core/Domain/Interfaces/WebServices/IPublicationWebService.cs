using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities;

namespace Domain.WebServices;

public interface IPublicationWebService
{
    Task<List<Publication>> GetPublicationPerfilById(Guid perfilId);
}
