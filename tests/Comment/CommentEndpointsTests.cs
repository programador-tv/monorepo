using Application.Logic;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.EndPoints;

namespace tests;

public class CommentEndPointsTests
{
    private readonly Mock<ICommentBusinessLogic> mockLogic;

    public CommentEndPointsTests()
    {
        mockLogic = new Mock<ICommentBusinessLogic>();
    }

    [Fact]
    public async Task GetAllByLiveIdAndPerfilId_ShouldReturnResponse_WhenOk()
    {
        var liveId = Guid.NewGuid();
        var perfilId = Guid.NewGuid();

        var comments = new List<Comment>
        {
            Comment.Create(perfilId, liveId, "comentário teste 1"),
            Comment.Create(perfilId, liveId, "comentário teste 2"),
        };

        mockLogic
            .Setup(logic => logic.GetAllByLiveIdAndPerfilId(liveId, perfilId))
            .ReturnsAsync(comments);

        var result = await CommentEndPoints.GetAllByLiveIdAndPerfilId(
            mockLogic.Object,
            liveId,
            perfilId
        );

        Assert.IsType<Ok<List<Comment>>>(result);
    }

    [Fact]
    public async Task GetAllByLiveIdAndPerfilId_ShouldReturnResponse_WhenException()
    {
        var liveId = Guid.NewGuid();
        var perfilId = Guid.NewGuid();

        mockLogic
            .Setup(logic => logic.GetAllByLiveIdAndPerfilId(liveId, perfilId))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await CommentEndPoints.GetAllByLiveIdAndPerfilId(
            mockLogic.Object,
            liveId,
            perfilId
        );

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task ValidateComment_ShouldReturnResponse_WhenOk()
    {
        var expectedId = "validId";

        var mockLogic = new Mock<ICommentBusinessLogic>();

        mockLogic.Setup(logic => logic.ValidateComment(It.IsAny<string>()))
            .Returns(Task.CompletedTask); 

        var result = await CommentEndPoints.ValidateComment(mockLogic.Object, expectedId);


        Assert.IsType<Ok>(result); 
    }

    [Fact]
    public async Task ValidateComment_ShouldReturnResponse_WhenException()
    {
        var expectedId = "validId";

        var mockLogic = new Mock<ICommentBusinessLogic>();

        mockLogic.Setup(logic => logic.ValidateComment(It.IsAny<string>()))
            .Throws(new Exception("Erro de teste"));

        var result = await CommentEndPoints.ValidateComment(mockLogic.Object, expectedId);


        Assert.IsType<BadRequest<string>>(result);
    }
}
