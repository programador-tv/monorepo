using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Domain.WebServices;

public interface ILikeWebService
{
    Task CreateLike(CreateLikeRequest createLikeRequest);
}
