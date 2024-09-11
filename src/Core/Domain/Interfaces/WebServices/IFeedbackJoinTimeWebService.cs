using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Domain.WebServices;

public interface IFeedbackJoinTimeWebService
{
    Task Add(Guid joinTimeId);
}
