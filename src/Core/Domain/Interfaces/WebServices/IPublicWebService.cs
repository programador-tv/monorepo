using Domain.Entities;

namespace Domain.WebServices;

public interface IPublicWebService
{
    Task<string> GetFotoPerfilBase64Async(string uerNickName);
}
