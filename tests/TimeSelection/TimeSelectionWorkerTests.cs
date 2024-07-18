using System.Net;
using System.Reflection;
using Background;
using Moq;

namespace tests;

public class TimeSelectionWorkerTests
{
    [Fact]
    public async Task ExecuteAsync_CallsHttpEndpoints_AndRespectsInterval()
    {
        // Simula uma resposta HTTP
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK);
        var fakeHandler = new FakeHttpMessageHandler(fakeResponse);
        var fakeHttpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://example.com/")
        };

        // Mock do IHttpClientFactory para retornar o HttpClient simulado
        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        // Inicializa TimeSelectionWorker com o mockFactory
        var worker = new TimeSelectionWorker(mockFactory.Object);

        // CancellationToken com timeout para encerrar o teste
        using var cts = new CancellationTokenSource(1000);

        var methodInfo = typeof(TimeSelectionWorker).GetMethod(
            "ExecuteAsync",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        var method = methodInfo?.Invoke(worker, new object[] { cts.Token });

        await (Task)method!;
        cts.Cancel();

        // Verifica se as URLs específicas foram chamadas
        var calledUrls = fakeHandler
            .Requests.Select(request => request.RequestUri?.ToString())
            .ToList();

        Assert.Contains(
            "http://example.com/api/timeSelections/UpdateOldTimeSelections",
            calledUrls
        );
        Assert.Contains(
            "http://example.com/api/timeSelections/NotifyUpcomingTimeSelectionAndJoinTime",
            calledUrls
        );
    }
}
