using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;

namespace tests;

public class LiveEntity
{
    [Fact]
    public void Create_GenereteTypeOfLive()
    {
        var request = new CreateLiveRequest(
            PerfilId: Guid.NewGuid(),
            Titulo: "Test",
            Descricao: "Teste de descrição",
            Thumbnail: "https://i.ytimg.com/vi/9XzDuhgJhKs/maxresdefault.jpg",
            IsUsingObs: false,
            StreamId: Guid.NewGuid().ToString(),
            UrlAlias: "Test-efb58h"
        );

        var live = Live.Create(request);

        Assert.IsType<Live>(live);
        Assert.Equal(request.PerfilId, live.PerfilId);
        Assert.Equal(request.Titulo, live.Titulo);
        Assert.Equal(request.Descricao, live.Descricao);
        Assert.Equal(request.Thumbnail, live.Thumbnail);
        Assert.Equal(request.IsUsingObs, live.IsUsingObs);
        Assert.Equal(request.UrlAlias, live.UrlAlias);
    }
}
