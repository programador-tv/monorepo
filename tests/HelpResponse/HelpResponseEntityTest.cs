using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;

namespace tests;

public class HelpResponseEntityTest
{
    [Fact]
    public void Create_GenereteTypeOfHelpResponse()
    {
        var request = new CreateHelpResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Conteudo da mensagem de teste"
        );
        var helpResponse = HelpResponse.Create(
            request.timeSelectionId,
            request.perfilId,
            request.Conteudo
        );

        Assert.IsType<HelpResponse>(helpResponse);
        Assert.Equal(request.timeSelectionId, helpResponse.TimeSelectionId);
        Assert.Equal(request.perfilId, helpResponse.PerfilId);
        Assert.Equal(request.Conteudo, helpResponse.Conteudo);
        Assert.Equal(
            helpResponse.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
            DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
        );
        Assert.Equal(nameof(ResponseStatus.Posted), helpResponse.ResponseStatus.ToString());
        Assert.IsType<ResponseStatus>(helpResponse.ResponseStatus);
    }

    [Fact]
    public void Update_ShouldUpdateTheCommentToDeletedStatus()
    {
        var request = new CreateHelpResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Conteudo da mensagem de teste"
        );
        var helpResponse = HelpResponse.Create(
            request.timeSelectionId,
            request.perfilId,
            request.Conteudo
        );
        helpResponse.DeleteResponse();

        Assert.IsType<ResponseStatus>(helpResponse.ResponseStatus);
        Assert.Equal(nameof(ResponseStatus.Deleted), helpResponse.ResponseStatus.ToString());
    }
}
