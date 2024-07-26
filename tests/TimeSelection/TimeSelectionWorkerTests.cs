using System.Net;
using System.Reflection;
using Background;
using Domain.WebServices;
using Moq;

namespace tests;

public class TimeSelectionWorkerTests
{
    [Fact]
    public async Task ExecuteAsync_CallsHttpEndpoints_AndRespectsInterval()
    {
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK);
        var fakeHandler = new FakeHttpMessageHandler(fakeResponse);
        var fakeHttpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://example.com/")
        };

        var mockTimeSelectionWebService = new Mock<ITimeSelectionWebService>();
        mockTimeSelectionWebService
            .Setup(service => service.UpdateOldTimeSelections())
            .Returns(Task.CompletedTask);

        var worker = new TimeSelectionWorker(mockTimeSelectionWebService.Object);

        using var cts = new CancellationTokenSource(1000);

        var methodInfo = typeof(TimeSelectionWorker).GetMethod(
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

        mockTimeSelectionWebService.Verify(service => service.UpdateOldTimeSelections(), Times.AtLeastOnce());
    }
}
