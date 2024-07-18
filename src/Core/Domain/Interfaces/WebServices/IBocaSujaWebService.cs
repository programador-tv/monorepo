using Domain.Contracts;

namespace Domain.WebServices;

public interface IBocaSujaWebService
{
    Task<string> Validate(string text, Guid id);
}
