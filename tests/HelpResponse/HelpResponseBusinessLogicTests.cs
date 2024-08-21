using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Repositories;
using Moq;

namespace tests;

public class HelpResponseBusinessLogicTests
{
    private readonly HelpResponseBusinessLogic _businessLogic;
    private readonly Mock<IHelpResponseRepository> _mockRepository;

    public HelpResponseBusinessLogicTests()
    {
        _mockRepository = new Mock<IHelpResponseRepository>();
        _businessLogic = new HelpResponseBusinessLogic(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllHelpResponses()
    {
        var timeSelectionId = Guid.NewGuid();
        var request = new CreateHelpResponse(
            timeSelectionId,
            Guid.NewGuid(),
            "Conteudo da mensagem de teste"
        );
        var newRequest = new CreateHelpResponse(
            timeSelectionId,
            Guid.NewGuid(),
            "Conteudo de outra mensagem"
        );

        var helpResponses = new List<HelpResponse>()
        {
            HelpResponse.Create(request.timeSelectionId, request.perfilId, request.Conteudo),
            HelpResponse.Create(
                newRequest.timeSelectionId,
                newRequest.perfilId,
                newRequest.Conteudo
            )
        };

        _mockRepository
            .Setup(repo => repo.GetAllAsync(timeSelectionId))
            .ReturnsAsync(helpResponses);

        var result = await _businessLogic.GetAll(timeSelectionId);

        Assert.NotNull(result);
        Assert.Equal(helpResponses.Count, result.Count);
        Assert.True(helpResponses.All(hr => hr.TimeSelectionId == timeSelectionId));
    }

    [Fact]
    public async Task AddHelpResponse_ShouldInvokeAddAsync()
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

        _mockRepository
            .Setup(repo => repo.AddAsync(It.IsAny<HelpResponse>()))
            .ReturnsAsync(helpResponse);

        await _businessLogic.Add(request);

        _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<HelpResponse>()), Times.Once);
    }

    [Fact]
    public async Task UpdateHelpResponse_ShouldInvokeUpdateAsync()
    {
        var timeSelectionId = Guid.NewGuid();
        var request = new CreateHelpResponse(
            timeSelectionId,
            Guid.NewGuid(),
            "Conteudo da mensagem de teste"
        );
        var helpResponse = HelpResponse.Create(
            request.timeSelectionId,
            request.perfilId,
            request.Conteudo
        );

        _mockRepository.Setup(repo => repo.GetById(It.IsAny<Guid>())).ReturnsAsync(helpResponse);
        _mockRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<HelpResponse>()))
            .Returns(Task.CompletedTask);

        helpResponse.DeleteResponse();
        await _businessLogic.Delete(helpResponse.Id);

        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<HelpResponse>()), Times.Once);
    }
}
