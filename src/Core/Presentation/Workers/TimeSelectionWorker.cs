using Application.Logic;
using Domain.WebServices;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Background;

public sealed class TimeSelectionWorker(IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    private const int EXECUTION_INTERVAL = 60000 * 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();

                var timeSelectionWebService =
                    scope.ServiceProvider.GetRequiredService<ITimeSelectionWebService>();

                await timeSelectionWebService.UpdateOldTimeSelections();
                await timeSelectionWebService.NotifyUpcomingTimeSelectionAndJoinTime();

                await Task.Delay(EXECUTION_INTERVAL, stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
