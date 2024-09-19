using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Repositories;
using Domain.WebServices;
using Application.Logic;
using Domain.Contracts;
using Queue;

public class QueueTests
{
    [Fact]
    public async Task Close_PublishesMessagesToBothQueues()
    {
        // Arrange
        var mockRepository = new Mock<ILiveRepository>();
        var mockMessagePublisher = new Mock<IMessagePublisher>();
        var mockOpenaiWebService = new Mock<IOpenaiWebService>();
        var mockPerfilRepository = new Mock<IPerfilRepository>();
        var mockTagRepository = new Mock<ITagRepository>();

        var logic = new LiveBusinessLogic(
            mockRepository.Object,
            mockMessagePublisher.Object,
            mockOpenaiWebService.Object,
            mockPerfilRepository.Object,
            mockTagRepository.Object
        );

        var idsToClose = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        mockRepository
            .Setup(r => r.CloseNonUpdatedLiveRangeAsync())
            .ReturnsAsync(idsToClose);

        // Act
        await logic.Close();

        // Assert
        foreach (var id in idsToClose)
        {
            // Verifica se a mensagem de fechamento foi publicada na fila "LiveCloseQueue"
            mockMessagePublisher
                .Verify(m => m.PublishAsync("LiveCloseQueue", It.Is<StopLiveProcessMessage>(msg => msg.Id == id)), Times.Once);

            // Verifica se a mensagem de conversão foi publicada na fila "LiveConversionQueue"
            mockMessagePublisher
                .Verify(m => m.PublishAsync("LiveConversionQueue", It.Is<LiveConversionMessage>(msg => msg.Id == id)), Times.Once);
        }
    }
}
