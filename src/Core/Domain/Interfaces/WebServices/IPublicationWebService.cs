using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Domain.WebServices;

public interface IPublicationWebService
{
    public Task Add(CreatePublicationRequest request);
    
}
