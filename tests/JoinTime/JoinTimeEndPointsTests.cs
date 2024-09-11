using Application.Logic;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.EndPoints;

namespace tests;

public class JoinTimeEndPointsTests
{
    [Fact]
    public async Task UpdateOldJoinTimes_ReturnsOkResult()
    {
        // Mock of IJoinTimeBusinessLogic
        var mockLogic = new Mock<IJoinTimeBusinessLogic>();

        mockLogic.Setup(logic => logic.UpdateOldJoinTimes()).Returns(Task.CompletedTask);

        // Call the UpdateOldJoinTimes method and verify the result
        var result = await JoinTimeEndPoints.UpdateOldJoinTimes(mockLogic.Object);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task UpdateOldJoinTimes_ReturnsBadRequest_OnException()
    {
        // Mock of IJoinTimeBusinessLogic
        var mockLogic = new Mock<IJoinTimeBusinessLogic>();

        mockLogic
            .Setup(logic => logic.UpdateOldJoinTimes())
            .ThrowsAsync(new Exception("Test error"));

        // Call the UpdateOldJoinTimes method and verify the result
        var result = await JoinTimeEndPoints.UpdateOldJoinTimes(mockLogic.Object);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task GetJoinTimesAtivos_ReturnsOk_WithValidData()
    {
        // Arrange: 
        var mockLogic = new Mock<IJoinTimeBusinessLogic>();
        var validGuid = Guid.NewGuid();
        var joinTimes = new List<JoinTime>
        {
            JoinTime.Create(validGuid, validGuid, StatusJoinTime.Marcado, false, TipoAction.Aprender),
            JoinTime.Create(validGuid, validGuid, StatusJoinTime.Pendente, false, TipoAction.Aprender)
        };
        mockLogic.Setup(logic => logic.GetJoinTimesAtivos(validGuid)).ReturnsAsync(joinTimes);

        // Act: 
        var result = await JoinTimeEndPoints.GetJoinTimesAtivos(validGuid, mockLogic.Object);

        // Assert: 
        var okResult = Assert.IsType<Ok<List<JoinTime>>>(result);
        Assert.Equal(joinTimes, okResult.Value);
    }

    [Fact]
    public async Task GetJoinTimesAtivos_ReturnsNotFound_WithValidDataButNoResults()
    {
        // Arrange: 
        var mockLogic = new Mock<IJoinTimeBusinessLogic>();
        var validGuid = Guid.NewGuid();
        mockLogic.Setup(logic => logic.GetJoinTimesAtivos(validGuid)).ReturnsAsync(new List<JoinTime>());

        // Act: 
        var result = await JoinTimeEndPoints.GetJoinTimesAtivos(validGuid, mockLogic.Object);

        // Assert: 
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task GetJoinTimesAtivos_ReturnsBadRequest_OnException()
    {
        // Arrange: 
        var mockLogic = new Mock<IJoinTimeBusinessLogic>();
        var invalidGuid = Guid.NewGuid();
        mockLogic.Setup(logic => logic.GetJoinTimesAtivos(invalidGuid)).ThrowsAsync(new Exception("Test error"));

        // Act: 
        var result = await JoinTimeEndPoints.GetJoinTimesAtivos(invalidGuid, mockLogic.Object);

        // Assert: 
        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Test error", badRequestResult.Value);
    }

    [Fact]
    public async Task GetJoinTimesAtivos_ReturnsBadRequest_WithInvalidGuid()
    {
        // Arrange: 
        var mockLogic = new Mock<IJoinTimeBusinessLogic>();
        var invalidGuid = Guid.Empty; 
        mockLogic.Setup(logic => logic.GetJoinTimesAtivos(invalidGuid)).ThrowsAsync(new ArgumentException("Invalid GUID"));

        // Act: 
        var result = await JoinTimeEndPoints.GetJoinTimesAtivos(invalidGuid, mockLogic.Object);

        // Assert: 
        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Invalid GUID", badRequestResult.Value);
    }

    [Fact]
    public async Task GetJoinTimesAtivos_ReturnsAllJoinTimes_WithMultipleResults()
    {
        // Arrange:
        var mockLogic = new Mock<IJoinTimeBusinessLogic>();
        var validGuid = Guid.NewGuid();
        var joinTimes = new List<JoinTime>
        {
            JoinTime.Create(validGuid, validGuid, StatusJoinTime.Marcado, false, TipoAction.Aprender),
            JoinTime.Create(validGuid, validGuid, StatusJoinTime.Pendente, false, TipoAction.Aprender),
            JoinTime.Create(validGuid, validGuid, StatusJoinTime.Marcado, false, TipoAction.Ensinar)
        };
        mockLogic.Setup(logic => logic.GetJoinTimesAtivos(validGuid)).ReturnsAsync(joinTimes);

        // Act:
        var result = await JoinTimeEndPoints.GetJoinTimesAtivos(validGuid, mockLogic.Object);

        // Assert: 
        var okResult = Assert.IsType<Ok<List<JoinTime>>>(result);
        Assert.True(okResult.Value?.Count == 3);
    }

}
