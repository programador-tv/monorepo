using Moq;
using Xunit;
using System;
using System.Threading.Tasks;
using Domain.Contracts;
using MassTransit;
using Background;
using Infrastructure.FileHandling;
using System.Collections.Concurrent;

public class LiveConversionQueueTests
{

    [Fact]
    public async Task Consume_ProcessesMessageAndCallsProcessMP4Correctly()
    {
        // Arrange
        var mockVideoHandling = new Mock<IVideoHandling>();
        var consumer = new LiveConversionQueue(mockVideoHandling.Object);

        var messageId = Guid.NewGuid();
        var message = new LiveConversionMessage(messageId);
        var context = new Mock<ConsumeContext<LiveConversionMessage>>();
        context.Setup(c => c.Message).Returns(message);

        // Act
        await consumer.Consume(context.Object);

        // Assert
        mockVideoHandling.Verify(
            handler => handler.ProcessMP4Async(messageId, 0),
            Times.Once
        );

    }


    [Fact]
    public async Task Consume_WithInvalidMessage_DoesNotCallProcessMP4()
    {
        // Arrange
        var mockVideoHandling = new Mock<IVideoHandling>();
        var consumer = new LiveConversionQueue(mockVideoHandling.Object);

        var context = new Mock<ConsumeContext<LiveConversionMessage>>();
        context.Setup(c => c.Message).Returns((LiveConversionMessage)null);

        // Act
        await consumer.Consume(context.Object);

        // Assert
        mockVideoHandling.Verify(
            handler => handler.ProcessMP4Async(It.IsAny<Guid>(), It.IsAny<int>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Consume_WhenProcessMP4Throws_HandlesException()
    {
        // Arrange
        var mockVideoHandling = new Mock<IVideoHandling>();
        mockVideoHandling
            .Setup(h => h.ProcessMP4Async(It.IsAny<Guid>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Processing failed"));

        var consumer = new LiveConversionQueue(mockVideoHandling.Object);

        var messageId = Guid.NewGuid();
        var message = new LiveConversionMessage(messageId);
        var context = new Mock<ConsumeContext<LiveConversionMessage>>();
        context.Setup(c => c.Message).Returns(message);

        // Act
        await consumer.Consume(context.Object);

        // Assert
        mockVideoHandling.Verify(
            handler => handler.ProcessMP4Async(messageId, It.IsAny<int>()),
            Times.Once
        );

    }
}
