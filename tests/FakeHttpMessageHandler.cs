namespace tests;

public interface IHttpClient
{
    Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken);
}

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;
    public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
        _response = response;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        Requests.Add(request);
        return await Task.FromResult(_response);
    }
}
