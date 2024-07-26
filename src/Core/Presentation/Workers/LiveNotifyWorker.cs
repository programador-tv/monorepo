using Application.Logic;
using Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;
using Domain.WebServices;

namespace Background;

public sealed class LiveNotifyWorker(ILiveWebService liveWebService) : BackgroundService
{
    private const int EXECUTION_INTERVAL = 60000 * 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await liveWebService.NotifyUpcomingLives();
                await Task.Delay(EXECUTION_INTERVAL, stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
