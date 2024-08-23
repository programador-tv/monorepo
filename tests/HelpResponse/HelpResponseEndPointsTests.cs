using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Presentation.EndPoints;

namespace tests;

public class HelpResponseEndPointsTests
{
    private readonly Mock<IHelpResponseBusinessLogic> mockLogic;

    public HelpResponseEndPointsTests()
    {
        mockLogic = new Mock<IHelpResponseBusinessLogic>();
    }

    [Fact]
    public async Task GetAll_ShuoldReturnOk_WithHelpResponse()
    {
        var timeSelectionId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var request = new CreateHelpResponse(
            timeSelectionId,
            profileId,
            "Conteudo da mensagem de teste"
        );
        var newRequest = new CreateHelpResponse(
            timeSelectionId,
            profileId,
            "Conteudo de outra mensagem"
        );

        var helpResponses = new List<HelpResponse>()
        {
            HelpResponse.Create(request.timeSelectionId, request.perfilId, request.Conteudo),
            HelpResponse.Create(
                newRequest.timeSelectionId,
                newRequest.perfilId,
                newRequest.Conteudo
            ),
        };

        mockLogic.Setup(logic => logic.GetAll(timeSelectionId)).ReturnsAsync(helpResponses);

        var result = await HelpResponseEndPoints.GetAll(mockLogic.Object, timeSelectionId);

        Assert.IsType<Ok<List<HelpResponse>>>(result);
        var resultOk = result as Ok<List<HelpResponse>>;
        Assert.Equal(helpResponses, resultOk?.Value);
    }

    [Fact]
    public async Task GetAll_ReturnBadRequestOnException()
    {
        var timeSelectionId = Guid.NewGuid();

        mockLogic
            .Setup(logic => logic.GetAll(timeSelectionId))
            .ThrowsAsync(new Exception("Exception de teste"));

        var result = await HelpResponseEndPoints.GetAll(mockLogic.Object, timeSelectionId);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task Add_ShouldReturnOk_WhenHelpResponseIsAdded()
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

        mockLogic.Setup(logic => logic.Add(request)).ReturnsAsync(helpResponse);

        var result = await HelpResponseEndPoints.Save(mockLogic.Object, request);

        Assert.IsType<Ok<HelpResponse>>(result);
    }

    [Fact]
    public async Task Add_ShouldReturnBadRequest_OnException()
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

        mockLogic.Setup(logic => logic.Add(request)).ThrowsAsync(new Exception("Erro de teste"));

        var result = await HelpResponseEndPoints.Save(mockLogic.Object, request);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenHelpResponseIsAddedWithDeletedStatus()
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

        mockLogic.Setup(logic => logic.Delete(helpResponse.Id)).Returns(Task.CompletedTask);

        var result = await HelpResponseEndPoints.Update(mockLogic.Object, helpResponse.Id);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_OnException()
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

        mockLogic
            .Setup(logic => logic.Delete(helpResponse.Id))
            .ThrowsAsync(new Exception("Exception de teste"));

        var result = await HelpResponseEndPoints.Update(mockLogic.Object, helpResponse.Id);

        Assert.IsType<BadRequest<string>>(result);
    }
}
