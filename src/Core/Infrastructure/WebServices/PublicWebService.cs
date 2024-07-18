using System.Globalization;
using System.Net.Http.Headers;
using Domain.WebServices;

namespace Infrastructure.WebServices;

public sealed class PublicWebService(WebAppClient client) : IPublicWebService
{
    public async Task<string> GetFotoPerfilBase64Async(string uerNickName)
    {
        var path = $"shared/profile/{uerNickName}.jpeg";
        var response = new byte[] { };

        try
        {
            response = await client.GetBytesAsync(path);
        }
        catch { }
        try
        {
            path = "no-user.jpg";
            response = await client.GetBytesAsync(path);
        }
        catch { }

        if (response.Length == 0)
        {
            return string.Empty;
        }

        return Convert.ToBase64String(response);
    }
}
