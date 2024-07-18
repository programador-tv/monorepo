using Application.Logic;
using Domain.Contracts;
using Domain.Repositories;
using Infrastructure.FileHandling;
using Microsoft.AspNetCore.Http;
using Moq;

namespace tests;

public class HelpBackstageBusinessLogicTest
{
    private readonly Mock<IHelpBackstageRepository> _mockRepository;
    private readonly Mock<ISaveFile> _mockSaveFile;
    private readonly HelpBackstageBusinessLogic _businessLogic;

    public HelpBackstageBusinessLogicTest()
    {
        _mockRepository = new Mock<IHelpBackstageRepository>();
        _mockSaveFile = new Mock<ISaveFile>();
        _businessLogic = new HelpBackstageBusinessLogic(
            _mockRepository.Object,
            _mockSaveFile.Object
        );
    }

    [Fact]
    public async Task AddHelpBackstage_ShouldInvokeScheduleResquestedHelp()
    {
        string fakeDescription = "DESCRICAO_FALSA";
        var createHelpBackstageRequest = new CreateHelpBackstageRequest(
            Guid.NewGuid(),
            fakeDescription
        );

        _mockRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.HelpBackstage>()))
            .Returns(Task.CompletedTask);

        await _businessLogic.ScheduleResquestedHelp(createHelpBackstageRequest);

        _mockRepository.Verify(
            repo => repo.AddAsync(It.IsAny<Domain.Entities.HelpBackstage>()),
            Times.Once()
        );
    }

    [Fact]
    public async Task SaveImageFile_ShouldThrowNewException()
    {
        Guid timeSelectionId = Guid.NewGuid();
        var formFile = new FormFile(Stream.Null, 0, 0, "name", "filename");

        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _businessLogic.SaveImageFile(timeSelectionId, formFile)
        );
    }

    [Fact]
    public async Task SaveImageFile_ShouldSaveImageForBackstage()
    {
        FormFile formFile = new(Stream.Null, 0, 0, "name", "image.jpg");
        var helpNackstage = Domain.Entities.HelpBackstage.Create(
            new(Guid.NewGuid(), "FAKE_DESCRIPTION")
        );
        Guid timeSelectionId = helpNackstage.TimeSelectionId;

        _mockRepository.Setup(repo => repo.AddAsync(helpNackstage));
        _mockRepository
            .Setup(repo => repo.GetByTimeSelectionIdAsync(timeSelectionId))
            .ReturnsAsync(helpNackstage);
        _mockSaveFile
            .Setup(e => e.SaveImageFile(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        await _businessLogic.SaveImageFile(timeSelectionId, formFile);

        _mockRepository.Verify(repo => repo.UpdateAsync(helpNackstage), Times.Once);
    }

    [Fact]
    public async Task GetByIds_ShouldthrowException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _businessLogic.GetByIds([])
        );
    }

    [Fact]
    public async Task GetByIds_ShouldReturnAListOfHelpBackstahe()
    {
        List<Domain.Entities.HelpBackstage> helpBackstages = [];

        for (int i = 0; i < 3; i++)
        {
            helpBackstages.Add(
                Domain.Entities.HelpBackstage.Create(new(Guid.NewGuid(), "FAKE_DESCRIPTION"))
            );
        }
        var tsIds = helpBackstages.Select(s => s.TimeSelectionId).ToList();

        _mockRepository.Setup(repo => repo.GetByIdsAsync(tsIds)).ReturnsAsync(helpBackstages);

        var result = await _businessLogic.GetByIds(tsIds);

        Assert.Multiple(() =>
        {
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
        });
    }
}
