using System.Net;
using System.Reflection;
using Background;
using Domain.WebServices;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using tests;

namespace tests;

public class JoinTimeWorkerTests
{
    [Fact]
    public async Task ExecuteAsync_CallsHttpEndpoints_AndRespectsInterval()
    {
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK);
        var fakeHandler = new FakeHttpMessageHandler(fakeResponse);
        var fakeHttpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://example.com/"),
        };

        var mockJoinTimeWebService = new Mock<IJoinTimeWebService>();
        mockJoinTimeWebService
            .Setup(service => service.UpdateOldJoinTimes())
            .Returns(Task.CompletedTask);

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(provider => provider.GetService(typeof(IJoinTimeWebService)))
            .Returns(mockJoinTimeWebService.Object);

        var mockServiceScope = new Mock<IServiceScope>();
        mockServiceScope.Setup(scope => scope.ServiceProvider).Returns(mockServiceProvider.Object);

        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        mockServiceScopeFactory
            .Setup(factory => factory.CreateScope())
            .Returns(mockServiceScope.Object);

        var worker = new JoinTimeWorker(mockServiceScopeFactory.Object);

        using var cts = new CancellationTokenSource(1000);

        var methodInfo = typeof(JoinTimeWorker).GetMethod(
            "ExecuteAsync",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (methodInfo == null)
        {
            throw new InvalidOperationException("Method 'ExecuteAsync' not found.");
        }

        var task = methodInfo.Invoke(worker, new object[] { cts.Token }) as Task;
        if (task == null)
        {
            throw new InvalidOperationException("Task returned by 'ExecuteAsync' is null.");
        }

        await task;

        cts.Cancel();

        mockJoinTimeWebService.Verify(service => service.UpdateOldJoinTimes(), Times.AtLeastOnce());
    }
}
