using System.Threading.Tasks;

namespace APP.Platform
{
    public interface IAliasService
    {
        Task<string> AliasGeneratorAsync(string title);
        Task<bool> AliasExists(string urlAlias);
        string NormalizeTitle(string title);
    }
}
