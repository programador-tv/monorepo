using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Presentation.EndPoints;

namespace tests;

public sealed class NotificationEndpointTests
{
    private readonly Mock<INotificationBusinessLogic> mockLogic;

    public NotificationEndpointTests()
    {
        mockLogic = new Mock<INotificationBusinessLogic>();
    }

    [Fact]
    public async Task SaveAndSendNotification_ShouldReturnOk_WithNotifications()
    {
        var destinoPerfilId = Guid.NewGuid();
        var geradorPerfilId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();

        var createNotificationRequest = new CreateNotificationRequest(
            DestinoPerfilId: destinoPerfilId,
            GeradorPerfilId: geradorPerfilId,
            TipoNotificacao: TipoNotificacao.AlunoAceitoNaMentoria,
            Conteudo: "Conteúdo",
            ActionLink: "ActionLink",
            SecundaryLink: "SecundaryLink"
        );

        var expectedNotification = new Notification(
            id: notificationId,
            destinoPerfilId: destinoPerfilId,
            geradorPerfilId: geradorPerfilId,
            tipoNotificacao: TipoNotificacao.AlunoAceitoNaMentoria,
            vizualizado: false,
            dataCriacao: DateTime.Now,
            conteudo: "Conteúdo",
            actionLink: "ActionLink",
            secundaryLink: "SecundaryLink"
        );

        mockLogic
            .Setup(logic => logic.SaveNotification(createNotificationRequest))
            .ReturnsAsync(expectedNotification);

        mockLogic
            .Setup(logic => logic.NotifyWithServices(expectedNotification))
            .Returns(Task.CompletedTask);

        var result = await NotificationsEndPoints.SaveAndSendNotification(
            mockLogic.Object,
            createNotificationRequest
        );

        mockLogic.Verify(logic => logic.SaveNotification(createNotificationRequest), Times.Once);

        mockLogic.Verify(logic => logic.NotifyWithServices(expectedNotification), Times.Once);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task SaveAndSendNotification_ShouldReturnBadRequest_WithMessage_OnException()
    {
        var destinoPerfilId = Guid.NewGuid();
        var geradorPerfilId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();

        var createNotificationRequest = new CreateNotificationRequest(
            DestinoPerfilId: destinoPerfilId,
            GeradorPerfilId: geradorPerfilId,
            TipoNotificacao: TipoNotificacao.AlunoAceitoNaMentoria,
            Conteudo: "Conteúdo",
            ActionLink: "ActionLink",
            SecundaryLink: "SecundaryLink"
        );

        var expectedNotification = new Notification(
            id: notificationId,
            destinoPerfilId: destinoPerfilId,
            geradorPerfilId: geradorPerfilId,
            tipoNotificacao: TipoNotificacao.AlunoAceitoNaMentoria,
            vizualizado: false,
            dataCriacao: DateTime.Now,
            conteudo: "Conteúdo",
            actionLink: "ActionLink",
            secundaryLink: "SecundaryLink"
        );

        mockLogic
            .Setup(logic => logic.SaveNotification(createNotificationRequest))
            .ThrowsAsync(new Exception("Erro de teste"));

        mockLogic
            .Setup(logic => logic.NotifyWithServices(expectedNotification))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await NotificationsEndPoints.SaveAndSendNotification(
            mockLogic.Object,
            createNotificationRequest
        );

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task GetNotificationsAndMarkAsViewed_ShouldReturnOk_WithNotifications()
    {
        // Arrange
        var destinationId = Guid.NewGuid();
        var expectedNotifications = new List<NotificationItemResponse>
        {
            new(
                destinationId,
                destinationId,
                TipoNotificacao.FinalizouCadastro,
                true,
                DateTime.Now,
                "Mensagem de teste numero 2",
                "https://programador.tv/Sobre",
                "_blank"
            ),
        };

        mockLogic
            .Setup(logic => logic.GetNotificationsAndMarkAsViewed(destinationId))
            .ReturnsAsync(expectedNotifications);

        // Act
        var result = await NotificationsEndPoints.GetNotificationsAndMarkAsViewed(
            mockLogic.Object,
            destinationId
        );

        // Assert
        Assert.IsType<Ok<List<NotificationItemResponse>>>(result);
        var okResult = result as Ok<List<NotificationItemResponse>>;
        Assert.Equal(expectedNotifications, okResult?.Value);
    }

    [Fact]
    public async Task GetNotificationsAndMarkAsViewed_ShouldReturnBadRequest_WithMessage_OnException()
    {
        // Arrange
        var destinationId = Guid.NewGuid();
        mockLogic
            .Setup(logic => logic.GetNotificationsAndMarkAsViewed(destinationId))
            .ThrowsAsync(new Exception("Erro de teste"));

        // Act
        var result = await NotificationsEndPoints.GetNotificationsAndMarkAsViewed(
            mockLogic.Object,
            destinationId
        );

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Erro de teste", badRequestResult.Value);
    }
}
