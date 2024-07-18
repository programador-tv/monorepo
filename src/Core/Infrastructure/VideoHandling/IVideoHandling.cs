using Microsoft.AspNetCore.Http;

namespace Infrastructure.FileHandling;

public interface IVideoHandling
{
    Task ProcessChunkAsync(Guid id, byte[] chunk, int tentative);
    Task StopAsync(Guid id);
}
