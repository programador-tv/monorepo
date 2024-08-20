using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Domain.WebServices;
using MassTransit.Saga;
using Moq;
using Queue;

namespace tests;

public class CommentBusinessLogicTests
{
    private readonly Mock<ICommentRepository> mockCommentRepo;
    private readonly Mock<ILiveRepository> mockLiveRepo;
    private readonly Mock<IPerfilRepository> mockPerfilRepo;
    private readonly Mock<IMessagePublisher> mockMessagePublisher;
    private readonly Mock<IBocaSujaWebService> mockBocaSujaWebService;
    private readonly CommentBusinessLogic businessLogic;

    public CommentBusinessLogicTests()
    {
        mockCommentRepo = new Mock<ICommentRepository>();
        mockLiveRepo = new Mock<ILiveRepository>();
        mockPerfilRepo = new Mock<IPerfilRepository>();
        mockMessagePublisher = new Mock<IMessagePublisher>();
        mockBocaSujaWebService = new Mock<IBocaSujaWebService>();

        businessLogic = new CommentBusinessLogic(
            mockCommentRepo.Object,
            mockLiveRepo.Object,
            mockPerfilRepo.Object,
            mockMessagePublisher.Object,
            mockBocaSujaWebService.Object
        );
    }

    [Fact]
    public async Task ValidateComment_SuccessfulValidation()
    {
        // Arrange
        var perfil = Perfil.Create(
            new CreatePerfilRequest(
                "nome teste",
                "token teste",
                "userName teste",
                "email teste",
                "",
                "",
                "",
                "",
                ExperienceLevel.MenosDe1Ano
            )
        );

        var live = Live.Create(
            new CreateLiveRequest(
                perfil.Id,
                "titulo teste",
                "descricao teste",
                "thumbnail teste",
                true,
                "streamId teste",
                ""
            )
        );

        var comment = Comment.Create(perfil.Id, live.Id, "comentario teste");

        var commentId = comment.Id.ToString();

        mockCommentRepo.Setup(repo => repo.GetById(commentId)).ReturnsAsync(comment);
        mockLiveRepo.Setup(repo => repo.GetByIdAsync(live.Id)).ReturnsAsync(live);
        mockPerfilRepo.Setup(repo => repo.GetByIdAsync(perfil.Id)).ReturnsAsync(perfil);
        mockBocaSujaWebService
            .Setup(service => service.Validate(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync("true");
        // Act
        await businessLogic.ValidateComment(commentId);

        // Assert
        mockCommentRepo.Verify(repo => repo.Update(comment), Times.Once);

        mockMessagePublisher.Verify(
            publisher => publisher.PublishAsync("NotificationsQueue", It.IsAny<Notification>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ValidateComment_503ServiceTemporarilyUnavailable()
    {
        // Arrange
        var perfil = Perfil.Create(
            new CreatePerfilRequest(
                "nome teste",
                "token teste",
                "userName teste",
                "email teste",
                "",
                "",
                "",
                "",
                ExperienceLevel.MenosDe1Ano
            )
        );

        var live = Live.Create(
            new CreateLiveRequest(
                perfil.Id,
                "titulo teste",
                "descricao teste",
                "thumbnail teste",
                true,
                "streamId teste",
                ""
            )
        );

        var comment = Comment.Create(perfil.Id, live.Id, "comentario teste");

        var commentId = comment.Id.ToString();

        mockCommentRepo.Setup(repo => repo.GetById(commentId)).ReturnsAsync(comment);
        mockLiveRepo.Setup(repo => repo.GetByIdAsync(live.Id)).ReturnsAsync(live);
        mockPerfilRepo.Setup(repo => repo.GetByIdAsync(perfil.Id)).ReturnsAsync(perfil);
        mockBocaSujaWebService
            .Setup(service => service.Validate(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync("503 Service Temporarily Unavailable");

        // Act
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => businessLogic.ValidateComment(commentId)
        );

        // Assert
        Assert.Equal("503 Serviço temporariamente indisponível.", exception.Message);
        mockCommentRepo.Verify(repo => repo.Update(comment), Times.Never);
        mockMessagePublisher.Verify(
            publisher => publisher.PublishAsync("NotificationsQueue", It.IsAny<Notification>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ValidateComment_FailedValidation()
    {
        // Arrange
        var perfil = Perfil.Create(
            new CreatePerfilRequest(
                "nome teste",
                "token teste",
                "userName teste",
                "email teste",
                "",
                "",
                "",
                "",
                ExperienceLevel.MenosDe1Ano
            )
        );

        var live = Live.Create(
            new CreateLiveRequest(
                perfil.Id,
                "titulo teste",
                "descricao teste",
                "thumbnail teste",
                true,
                "streamId teste",
                ""
            )
        );

        var comment = Comment.Create(perfil.Id, live.Id, "comentario teste");

        var commentId = comment.Id.ToString();

        mockCommentRepo.Setup(repo => repo.GetById(commentId)).ReturnsAsync(comment);
        mockLiveRepo.Setup(repo => repo.GetByIdAsync(live.Id)).ReturnsAsync(live);
        mockPerfilRepo.Setup(repo => repo.GetByIdAsync(perfil.Id)).ReturnsAsync(perfil);
        mockBocaSujaWebService
            .Setup(service => service.Validate(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync("false");

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(
            () => businessLogic.ValidateComment(commentId)
        );

        // Assert
        mockCommentRepo.Verify(repo => repo.Update(comment), Times.Never);
        mockMessagePublisher.Verify(
            publisher => publisher.PublishAsync("NotificationsQueue", It.IsAny<Notification>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetAllByLiveIdAndPerfilId_ShouldReturnListOfComments()
    {
        var liveId = Guid.NewGuid();
        var perfilId = Guid.NewGuid();

        var comments = new List<Comment>
        {
            Comment.Create(perfilId, liveId, "comentário teste 1"),
            Comment.Create(perfilId, liveId, "comentário teste 2"),
        };

        mockCommentRepo
            .Setup(repo => repo.GetAllByLiveIdAndPerfilId(liveId, perfilId))
            .ReturnsAsync(comments);

        var result = await businessLogic.GetAllByLiveIdAndPerfilId(liveId, perfilId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(
            result,
            comment =>
            {
                Assert.Equal(liveId, comment.LiveId);
                Assert.Equal(perfilId, comment.PerfilId);
            }
        );
    }

    [Fact]
    public async Task GetAllByLiveIdAndPerfilId_ShouldReturnAnEmptyListOfComments()
    {
        var liveId = Guid.NewGuid();
        var perfilId = Guid.NewGuid();

        mockCommentRepo
            .Setup(repo => repo.GetAllByLiveIdAndPerfilId(liveId, perfilId))
            .ReturnsAsync(new List<Comment>());

        var result = await businessLogic.GetAllByLiveIdAndPerfilId(liveId, perfilId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
