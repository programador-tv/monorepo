using Background;
using Domain.WebServices;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using static Azure.Core.HttpHeader;

namespace tests;

public class LiveNotifyWorkerTest
{
    [Fact]
    public async Task StartAsync_ShouldCall_NotifyUpcomingLives()
    {
        var mockLiveWebService = new Mock<ILiveWebService>();
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockServiceScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ILiveWebService)))
            .Returns(mockLiveWebService.Object);

        var liveNotifyWorkerSimulation = new LiveNotifyWorker(mockServiceScopeFactory.Object);
        const int EXECUTION_INTERVAL = 100;
        var cancellationTokenSource = new CancellationTokenSource(EXECUTION_INTERVAL);

        await liveNotifyWorkerSimulation.StartAsync(cancellationTokenSource.Token);
        mockLiveWebService.Verify(x => x.NotifyUpcomingLives(), Times.AtLeastOnce());
    }

    [Fact]
    public async Task StartAsync_ShouldCall_NotifyUpcomingLives_Exceptions()
    {
        var mockLiveWebService = new Mock<ILiveWebService>();
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockLiveWebService
            .Setup(x => x.NotifyUpcomingLives())
            .ThrowsAsync(new Exception("teste exception"));
        mockServiceScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ILiveWebService)))
            .Returns(mockLiveWebService.Object);

        var liveNotifyWorkerSimulation = new LiveNotifyWorker(mockServiceScopeFactory.Object);
        const int EXECUTION_INTERVAL = 100;
        var cancellationTokenSource = new CancellationTokenSource(EXECUTION_INTERVAL);
        var exception = await Record.ExceptionAsync(
            () => liveNotifyWorkerSimulation.StartAsync(cancellationTokenSource.Token)
        );

        Assert.Null(exception);
    }
}
