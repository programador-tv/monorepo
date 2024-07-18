namespace Infrastructure.WebServices;

public sealed class BocaSujaClient(IHttpClientFactory factory)
    : AbstractClient(factory, "BocaSujaAPI") { }
