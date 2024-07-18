namespace Infrastructure.WebServices;

public sealed class OpenaiClient(IHttpClientFactory factory)
    : AbstractClient(factory, "OpenaiAPI") { }
