using Application.Logic;
using Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;
using Domain.WebServices;

namespace Background;

public sealed class LiveCloseWorker(ILiveWebService liveWebService) : BackgroundService
{
    private const int EXECUTION_INTERVAL = 60000 * 3;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await liveWebService.Close();
                await Task.Delay(EXECUTION_INTERVAL, stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
