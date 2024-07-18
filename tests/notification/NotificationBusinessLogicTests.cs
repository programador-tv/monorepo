using System.Net.Mail;
using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using MassTransit.RabbitMqTransport;
using Moq;
using Queue;

namespace tests;

public class NotificationBusinessLogicTests
{
    private readonly NotificationBusinessLogic _businessLogic;
    private readonly Mock<INotificationRepository> _mockRepository;
    private readonly Mock<IPerfilRepository> _mockPerfilRepository;
    private readonly Mock<IMessagePublisher> _mockPublisherRepository;

    public NotificationBusinessLogicTests()
    {
        _mockRepository = new Mock<INotificationRepository>();
        _mockPerfilRepository = new Mock<IPerfilRepository>();
        _mockPublisherRepository = new Mock<IMessagePublisher>();
        _businessLogic = new NotificationBusinessLogic(
            _mockRepository.Object,
            _mockPerfilRepository.Object,
            _mockPublisherRepository.Object
        );
    }

    [Fact]
    public async Task GetNotificationsByPerfilId_shouldReturnAllUserNotifications()
    {
        var fakeDestinationId = Guid.NewGuid();
        var fakeGeneratorId = Guid.NewGuid();
        var notifications = new List<Notification>
        {
            Notification.Create(
                fakeDestinationId,
                fakeGeneratorId,
                TipoNotificacao.FinalizouCadastro,
                "Obrigado por finalizar seu cadastro, agora você pode criar e participar de salas de estudo e compartilhar seus conhecimentos ao vivo. Saiba mais sobre o projeto clicando em ver",
                "https://programador.tv/Sobre",
                "_blank"
            ),
            Notification.Create(
                fakeDestinationId,
                fakeGeneratorId,
                TipoNotificacao.FinalizouCadastro,
                "Mensagem de teste numero 2",
                "https://programador.tv/Sobre",
                "_blank"
            )
        };

        _mockRepository
            .Setup(repo => repo.GetNotificationsByPerfilId(fakeDestinationId))
            .ReturnsAsync(notifications);

        var result = await _businessLogic.GetNotificationsByPerfilId(fakeDestinationId);

        Assert.Multiple(() =>
        {
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        });
    }

    [Fact]
    public async Task GetNotificationsAndMarkAsViewed_shouldReturnNotificationsAndMarkAsViewed()
    {
        var fakeDestinationId = Guid.NewGuid();
        var fakeGeneratorId = Guid.NewGuid();
        var notifications = new List<Notification>
        {
            Notification.Create(
                fakeDestinationId,
                fakeGeneratorId,
                TipoNotificacao.FinalizouCadastro,
                "Obrigado por finalizar seu cadastro, agora você pode criar e participar de salas de estudo e compartilhar seus conhecimentos ao vivo. Saiba mais sobre o projeto clicando em ver",
                "https://programador.tv/Sobre",
                "_blank"
            ),
            Notification.Create(
                fakeDestinationId,
                fakeGeneratorId,
                TipoNotificacao.FinalizouCadastro,
                "Mensagem de teste numero 2",
                "https://programador.tv/Sobre",
                "_blank"
            )
        };

        _mockRepository
            .Setup(repo => repo.GetNotificationsByPerfilId(fakeDestinationId))
            .ReturnsAsync(notifications);

        var result = await _businessLogic.GetNotificationsAndMarkAsViewed(fakeDestinationId);

        Assert.Multiple(() =>
        {
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        });

        _mockRepository.Verify(
            repo => repo.UpdateRangeAsync(It.IsAny<List<Notification>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task SaveNotification_shouldSaveNotification()
    {
        var fakeDestinationId = Guid.NewGuid();
        var fakeGeneratorId = Guid.NewGuid();
        var request = new CreateNotificationRequest(
            DestinoPerfilId: fakeDestinationId,
            GeradorPerfilId: fakeGeneratorId,
            TipoNotificacao: TipoNotificacao.FinalizouCadastro,
            Conteudo: "teste",
            ActionLink: "https://programador.tv/Sobre",
            SecundaryLink: "_blank"
        );

        var notification = Notification.Create(
            fakeDestinationId,
            fakeGeneratorId,
            TipoNotificacao.FinalizouCadastro,
            "teste",
            "https://programador.tv/Sobre",
            "_blank"
        );

        _mockRepository
            .Setup(repo => repo.SaveNotification(It.IsAny<Notification>()))
            .ReturnsAsync(notification);

        var result = await _businessLogic.SaveNotification(request);

        Assert.NotNull(result);
    }
}
