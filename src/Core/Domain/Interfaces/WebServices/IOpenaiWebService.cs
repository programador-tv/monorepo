using Domain.Entities;

namespace Domain.WebServices;

public interface IOpenaiWebService
{
    Task<string> GetCompletationResponse(string prompt);
}
