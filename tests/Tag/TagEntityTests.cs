using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;

namespace tests;

public class TagEntityTest
{
    [Fact]
    public void AddForLiveAndFreeTime()
    {
        var titulo = "Teste";
        var liveRelacao = Guid.NewGuid().ToString();
        var freeTimeRelacao = Guid.NewGuid().ToString();

        var tag = Tag.AddForLiveAndFreeTime(titulo, liveRelacao, freeTimeRelacao);

        Assert.IsType<Tag>(tag);
        Assert.Equal(titulo, tag.Titulo);
        Assert.Equal(liveRelacao, tag.LiveRelacao);
        Assert.Equal(freeTimeRelacao, tag.FreeTimeRelacao);
    }
}
