using Application.Logic;
using Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;

namespace Background;

public sealed class LiveNotifyWorker(IHttpClientFactory factory) : BackgroundService
{
    private const int EXECUTION_INTERVAL = 60000 * 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var client = factory.CreateClient("CoreAPI");
            var NotifyUpcomingLives = client.GetAsync(
                "api/lives/NotifyUpcomingLives",
                stoppingToken
            );

            try
            {
                await NotifyUpcomingLives;
                await Task.Delay(EXECUTION_INTERVAL, stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
