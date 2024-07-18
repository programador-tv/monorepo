using Domain.Contracts;
using Domain.Entities;

namespace tests;

public class HelpBackstageEntityTest
{
    [Fact]
    public void Create_GenereteTypeOfHelpBackstage_withNoImage()
    {
        var request = new CreateHelpBackstageRequest(Guid.NewGuid(), "FAKE_DESCRICAO");

        var helpBackstage = HelpBackstage.Create(request);

        Assert.Multiple(() =>
        {
            Assert.NotNull(helpBackstage);
            Assert.IsType<HelpBackstage>(helpBackstage);
            Assert.Equal(request.TimeSelectionId, helpBackstage.TimeSelectionId);
            Assert.Equal(request.Description, helpBackstage.Descricao);
            Assert.Null(helpBackstage.ImagePath);
        });
    }

    [Fact]
    public void AddImagePath()
    {
        var fakeImgPath = "CAMINHO_DA_IMG";
        var request = new CreateHelpBackstageRequest(Guid.NewGuid(), "FAKE_DESCRICAO");

        var helpBackstage = HelpBackstage.Create(request);

        helpBackstage.AddImagePath(fakeImgPath);

        Assert.Equal(fakeImgPath, helpBackstage.ImagePath);
    }
}
