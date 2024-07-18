namespace Infrastructure.WebServices;

public sealed class WebAppClient(IHttpClientFactory factory)
    : AbstractClient(factory, "PublicWebApp") { }
