using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Domain.WebServices;
using Moq;
using Queue;

namespace tests;

public class LiveBusinessLogicTests
{
    private readonly LiveBusinessLogic _businessLogic;
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly Mock<IMessagePublisher> _mockMessagePublisher;
    private readonly Mock<IOpenaiWebService> _mockOpenaiWebService;
    private readonly Mock<ILiveRepository> _mockRepository;
    private readonly Mock<IPerfilRepository> _mockPerfilRepository;

    public LiveBusinessLogicTests()
    {
        _mockRepository = new Mock<ILiveRepository>();
        _mockMessagePublisher = new Mock<IMessagePublisher>();
        _mockOpenaiWebService = new Mock<IOpenaiWebService>();
        _mockPerfilRepository = new Mock<IPerfilRepository>();
        _mockTagRepository = new Mock<ITagRepository>();

        _businessLogic = new LiveBusinessLogic(
            _mockRepository.Object,
            _mockMessagePublisher.Object,
            _mockOpenaiWebService.Object,
            _mockPerfilRepository.Object,
            _mockTagRepository.Object
        );
    }

    private Live LiveMock()
    {
        return Live.Create(
            new CreateLiveRequest(
                PerfilId: Guid.NewGuid(),
                Titulo: "Test",
                Descricao: "Teste de descrição",
                Thumbnail: "https://i.ytimg.com/vi/9XzDuhgJhKs/maxresdefault.jpg",
                IsUsingObs: false,
                StreamId: Guid.NewGuid().ToString(),
                UrlAlias: "Test-efb58h"
            )
        );
    }

    [Fact]
    public async Task GetById_ShouldReturnALive()
    {
        var live = LiveMock();
        _mockRepository.Setup(repo => repo.GetByIdAsync(live.Id)).ReturnsAsync(live);

        var result = await _businessLogic.GetLiveById(live.Id);

        Assert.NotNull(result);
        Assert.Equal(live.Id, result.Id);
    }

    [Fact]
    public async Task AddLive_ShouldAddALive()
    {
        var createLiveRequest = new CreateLiveRequest(
            PerfilId: Guid.NewGuid(),
            Titulo: "Test",
            Descricao: "Teste de descrição",
            Thumbnail: "https://i.ytimg.com/vi/9XzDuhgJhKs/maxresdefault.jpg",
            IsUsingObs: false,
            StreamId: Guid.NewGuid().ToString(),
            UrlAlias: "Test-efb58h"
        );

        _mockRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Live>()))
            .ReturnsAsync(Live.Create(createLiveRequest));

        var result = await _businessLogic.AddLive(createLiveRequest);

        _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<Live>()), Times.Once);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetLiveByUrl_ShouldReturnALive()
    {
        var liveUrl = LiveMock();
        _mockRepository
            .Setup(repo => repo.GetByUrlAsync(liveUrl.UrlAlias ?? string.Empty))
            .ReturnsAsync(liveUrl);

        var result = await _businessLogic.GetLiveByUrl(liveUrl.UrlAlias ?? string.Empty);

        Assert.NotNull(result);
        Assert.Equal(liveUrl.UrlAlias, result.UrlAlias);
    }

    [Fact]
    public async Task NotifyUpcomingLives_ShouldSendNotificationsAndMarkUsers()
    {
        // Arrange
        var upcomingLives = new Dictionary<Live, List<NotifyUserLiveEarly>>();
        var live = LiveMock();

        var usersToNotify = new List<NotifyUserLiveEarly>
        {
            NotifyUserLiveEarly.Create(live.Id, Guid.NewGuid(), true, false),
        };

        upcomingLives.Add(live, usersToNotify);

        _mockRepository.Setup(repo => repo.GetUpcomingLives()).ReturnsAsync(upcomingLives);

        foreach (var user in usersToNotify)
        {
            _mockMessagePublisher
                .Setup(publisher =>
                    publisher.PublishAsync("NotificationsQueue", It.IsAny<Notification>())
                )
                .Verifiable();
        }

        // Act
        await _businessLogic.NotifyUpcomingLives();

        // Assert
        _mockRepository.Verify(repo => repo.GetUpcomingLives(), Times.Once);
        foreach (var user in usersToNotify)
        {
            Assert.True(user.HasNotificated);
            _mockMessagePublisher.Verify(
                publisher => publisher.PublishAsync("NotificationsQueue", It.IsAny<Notification>()),
                Times.Once
            );
        }

        _mockRepository.Verify(
            repo => repo.UpdateRangeNotifyUserLiveEarly(It.IsAny<List<NotifyUserLiveEarly>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateThumbnail_WithValidRequest()
    {
        var liveId = Guid.NewGuid();
        var updateRequest = new UpdateLiveThumbnailRequest(liveId, "Updated_Image");
        var existingLive = LiveMock();

        _mockRepository.Setup(repo => repo.GetByIdAsync(liveId)).ReturnsAsync(existingLive);

        await _businessLogic.UpdateThumbnail(updateRequest);

        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Live>()), Times.Once);
        Assert.Equal(updateRequest.ImageBase64, existingLive.Thumbnail);
    }

    [Fact]
    public async Task UpdateThumbnail_WithNonExistentLive_ShouldThrowKeyNotFoundException()
    {
        var nonExistentLiveId = Guid.NewGuid();
        var updateRequest = new UpdateLiveThumbnailRequest(nonExistentLiveId, "Updated_Image");

        _mockRepository
            .Setup(repo => repo.GetByIdAsync(nonExistentLiveId))
            .ReturnsAsync((Live)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _businessLogic.UpdateThumbnail(updateRequest)
        );
    }

    [Fact]
    public async Task KeepOn_WithValidId_ShouldUpdateLiveStatus()
    {
        var liveId = Guid.NewGuid();
        var existingLive = LiveMock();

        _mockRepository.Setup(repo => repo.GetByIdAsync(liveId)).ReturnsAsync(existingLive);

        await _businessLogic.KeepOn(liveId);

        _mockRepository.Verify(repo => repo.UpdateAsync(existingLive), Times.Once);

        Assert.True(existingLive.LiveEstaAberta);
    }

    [Fact]
    public async Task KeepOn_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        var nonExistentLiveId = Guid.NewGuid();

        _mockRepository
            .Setup(repo => repo.GetByIdAsync(nonExistentLiveId))
            .ReturnsAsync((Live)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _businessLogic.KeepOn(nonExistentLiveId)
        );
    }

    [Fact]
    public async Task FinishWithDuration_WithValidId_ShouldUpdateLiveStatusAndDuration()
    {
        var liveId = Guid.NewGuid();
        var duration = "02:30:00"; // Exemplo de duração

        var existingLive = LiveMock();

        _mockRepository.Setup(repo => repo.GetByIdAsync(liveId)).ReturnsAsync(existingLive);

        await _businessLogic.FinishWithDuration(liveId, duration);

        _mockRepository.Verify(repo => repo.UpdateAsync(existingLive), Times.Once);

        Assert.True(existingLive.StatusLive == StatusLive.Finalizada);
        Assert.Equal(duration, existingLive.FormatedDuration);
    }

    [Fact]
    public async Task FinishWithDuration_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        var nonExistentLiveId = Guid.NewGuid();
        var duration = "02:30:00"; // Exemplo de duração

        _mockRepository
            .Setup(repo => repo.GetByIdAsync(nonExistentLiveId))
            .ReturnsAsync((Live)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _businessLogic.FinishWithDuration(nonExistentLiveId, duration)
        );
    }

    [Fact]
    public async Task GetKeyByStreamId_ShouldReturnId()
    {
        var streamId = "test-id";
        var expectedId = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.GetKeyByStreamId(streamId)).ReturnsAsync(expectedId);

        var result = await _businessLogic.GetKeyByStreamId(streamId);

        Assert.Equal(expectedId, result);
        _mockRepository.Verify(repo => repo.GetKeyByStreamId(streamId), Times.Once);
    }

    [Fact]
    public async Task SearchLives_ShouldReturnListWithLives()
    {
        var keyword = "Business";

        var live1 = new Live(
            id: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "Business",
            descricao: "",
            thumbnail: "Thumbnail",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        var live2 = new Live(
            id: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "Business Logic",
            descricao: "",
            thumbnail: "Thumbnail",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        var live3 = new Live(
            id: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "",
            descricao: "Business",
            thumbnail: "Thumbnail",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        var live4 = new Live(
            id: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "",
            descricao: "",
            thumbnail: "Thumbnail",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        var perfil = Perfil.Create(
            new CreatePerfilRequest(
                Nome: "Test",
                Token: "12345qwerty",
                UserName: "test",
                Linkedin: "linkedin.com/test",
                GitHub: "github.com/test",
                Bio: "Teste de bio",
                Email: "test@test.com",
                Descricao: "Teste de descrição",
                Experiencia: ExperienceLevel.Entre1E3Anos
            )
        );

        var live5 = new Live(
            id: Guid.NewGuid(),
            perfilId: perfil.Id,
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "",
            descricao: "",
            thumbnail: "Thumbnail",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        var perfis = new List<Perfil> { perfil };
        var tag = Tag.AddForLive("Business", live4.Id.ToString());
        var tags = new List<Tag> { tag };
        var lives = new List<Live> { live1, live2, live3, live4, live5 };

        _mockRepository
            .Setup(repo => repo.SearchBySpecificTitle(It.IsAny<string>()))
            .ReturnsAsync(new List<Live> { live1 });
        _mockRepository
            .Setup(repo => repo.SearchByTitleContaining(It.IsAny<string>()))
            .ReturnsAsync(new List<Live> { live2 });
        _mockRepository
            .Setup(repo => repo.SearchByDescriptionContaining(It.IsAny<string>()))
            .ReturnsAsync(new List<Live> { live3 });
        _mockRepository
            .Setup(repo => repo.SearchByTagContaining(It.IsAny<List<Tag>>()))
            .ReturnsAsync(new List<Live> { live4 });
        _mockRepository
            .Setup(repo => repo.SearchByListPerfilId(It.IsAny<List<Perfil>>()))
            .ReturnsAsync(new List<Live> { live5 });

        _mockPerfilRepository
            .Setup(repo => repo.GetWhenContainsAsync(It.IsAny<string>()))
            .ReturnsAsync(perfis);
        _mockTagRepository
            .Setup(repo => repo.GetTagRelationByKeyword(It.IsAny<string>()))
            .ReturnsAsync(tags);

        var result = await _businessLogic.SearchLives(keyword);

        Assert.NotNull(result);
        Assert.Equal(5, result.Count);

        foreach (var live in lives)
        {
            Assert.Contains(live, result);
        }

        _mockRepository.Verify(repo => repo.SearchBySpecificTitle(It.IsAny<string>()), Times.Once);
        _mockRepository.Verify(
            repo => repo.SearchByTitleContaining(It.IsAny<string>()),
            Times.Once
        );
        _mockRepository.Verify(
            repo => repo.SearchByDescriptionContaining(It.IsAny<string>()),
            Times.Once
        );
        _mockRepository.Verify(
            repo => repo.SearchByListPerfilId(It.IsAny<List<Perfil>>()),
            Times.Once
        );
        _mockRepository.Verify(
            repo => repo.SearchByTagContaining(It.IsAny<List<Tag>>()),
            Times.Once
        );
        _mockPerfilRepository.Verify(
            repo => repo.GetWhenContainsAsync(It.IsAny<string>()),
            Times.Once
        );
        _mockTagRepository.Verify(
            repo => repo.GetTagRelationByKeyword(It.IsAny<string>()),
            Times.Once
        );
    }

    [Fact]
    public async Task SearchLives_ShouldReturnEmptyList()
    {
        var keyword = "Business";

        var live = new Live(
            id: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "",
            descricao: "",
            thumbnail: "Thumbnail",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        var perfil = Perfil.Create(
            new CreatePerfilRequest(
                Nome: "Test",
                Token: "12345qwerty",
                UserName: "test",
                Linkedin: "linkedin.com/test",
                GitHub: "github.com/test",
                Bio: "Teste de bio",
                Email: "test@test.com",
                Descricao: "Teste de descrição",
                Experiencia: ExperienceLevel.Entre1E3Anos
            )
        );

        var perfis = new List<Perfil> { perfil };
        var tag = Tag.AddForLive("Teste", Guid.NewGuid().ToString());
        var tags = new List<Tag> { tag };
        var lives = new List<Live> { live };

        _mockRepository
            .Setup(repo => repo.SearchBySpecificTitle(It.IsAny<string>()))
            .ReturnsAsync(new List<Live>());
        _mockRepository
            .Setup(repo => repo.SearchByTitleContaining(It.IsAny<string>()))
            .ReturnsAsync(new List<Live>());
        _mockRepository
            .Setup(repo => repo.SearchByDescriptionContaining(It.IsAny<string>()))
            .ReturnsAsync(new List<Live>());
        _mockRepository
            .Setup(repo => repo.SearchByTagContaining(It.IsAny<List<Tag>>()))
            .ReturnsAsync(new List<Live>());
        _mockRepository
            .Setup(repo => repo.SearchByListPerfilId(It.IsAny<List<Perfil>>()))
            .ReturnsAsync(new List<Live>());

        _mockPerfilRepository
            .Setup(repo => repo.GetWhenContainsAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<Perfil>());
        _mockTagRepository
            .Setup(repo => repo.GetTagRelationByKeyword(It.IsAny<string>()))
            .ReturnsAsync(new List<Tag>());

        var result = await _businessLogic.SearchLives(keyword);

        Assert.NotNull(result);
        Assert.Empty(result);

        _mockRepository.Verify(repo => repo.SearchBySpecificTitle(It.IsAny<string>()), Times.Once);
        _mockRepository.Verify(
            repo => repo.SearchByTitleContaining(It.IsAny<string>()),
            Times.Once
        );
        _mockRepository.Verify(
            repo => repo.SearchByDescriptionContaining(It.IsAny<string>()),
            Times.Once
        );
        _mockRepository.Verify(
            repo => repo.SearchByListPerfilId(It.IsAny<List<Perfil>>()),
            Times.Once
        );
        _mockRepository.Verify(
            repo => repo.SearchByTagContaining(It.IsAny<List<Tag>>()),
            Times.Once
        );
        _mockPerfilRepository.Verify(
            repo => repo.GetWhenContainsAsync(It.IsAny<string>()),
            Times.Once
        );
        _mockTagRepository.Verify(
            repo => repo.GetTagRelationByKeyword(It.IsAny<string>()),
            Times.Once
        );
    }
}
