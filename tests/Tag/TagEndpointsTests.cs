using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Presentation.EndPoints;

namespace tests;

public class TagEndPointsTests
{
    private readonly Mock<ITagBusinessLogic> mockLogic;

    public TagEndPointsTests()
    {
        mockLogic = new Mock<ITagBusinessLogic>();
    }

    [Fact]
    public async Task CreateTagsForLiveAndFreeTime_ShouldReturnResponse_WhenOk()
    {
        var list = new List<CreateTagForLiveAndFreeTimeRequest>();
        var request = new CreateTagForLiveAndFreeTimeRequest(
            Titulo: "Teste",
            LiveId: Guid.NewGuid().ToString(),
            FreeTimeId: Guid.NewGuid().ToString()
        );
        list.Add(request);

        mockLogic
            .Setup(logic => logic.CreateTagsForLiveAndFreeTime(list))
            .Returns(Task.CompletedTask);

        var result = await TagEndPoints.CreateTagsForLiveAndFreeTime(mockLogic.Object, list);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task CreateTagsForLiveAndFreeTime_ShouldReturnResponse_WhenException()
    {
        var list = new List<CreateTagForLiveAndFreeTimeRequest>();
        var request = new CreateTagForLiveAndFreeTimeRequest(
            Titulo: "Teste",
            LiveId: Guid.NewGuid().ToString(),
            FreeTimeId: Guid.NewGuid().ToString()
        );
        list.Add(request);

        mockLogic
            .Setup(logic => logic.CreateTagsForLiveAndFreeTime(list))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await TagEndPoints.CreateTagsForLiveAndFreeTime(mockLogic.Object, list);

        Assert.IsType<BadRequest<string>>(result);
    }
}
