using Application.Logic;
using Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;

namespace Background;

public sealed class LiveCloseWorker(IHttpClientFactory factory) : BackgroundService
{
    private const int EXECUTION_INTERVAL = 60000 * 3;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var client = factory.CreateClient("CoreAPI");
            var CloseNotUpdatedAnymore = client.GetAsync("api/lives/close", stoppingToken);
            try
            {
                await CloseNotUpdatedAnymore;
                await Task.Delay(EXECUTION_INTERVAL, stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
