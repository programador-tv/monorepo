using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using Queue;

namespace tests;

public class PerfilBusinessLogicTests
{
    private readonly PerfilBusinessLogic _businessLogic;
    private readonly Mock<IPerfilRepository> _mockRepository;
    private readonly Mock<IMessagePublisher> _mockMessagePublisher;

    public PerfilBusinessLogicTests()
    {
        _mockRepository = new Mock<IPerfilRepository>();
        _mockMessagePublisher = new Mock<IMessagePublisher>();
        _businessLogic = new PerfilBusinessLogic(
            _mockRepository.Object,
            _mockMessagePublisher.Object
        );
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllPerfils()
    {
        var perfis = new List<Perfil>
        {
            Perfil.Create(
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
            ),
            Perfil.Create(
                new CreatePerfilRequest(
                    Nome: "Test2",
                    Token: "12345qwerty",
                    UserName: "test2",
                    Linkedin: "linkedin.com/test2",
                    GitHub: "github.com/test2",
                    Bio: "Teste de bio",
                    Email: "test2@test.com",
                    Descricao: "Teste de descrição",
                    Experiencia: ExperienceLevel.Entre1E3Anos
                )
            ),
        };
        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(perfis);

        // Act
        var result = await _businessLogic.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(perfis.Count, result.Count);
    }

    [Fact]
    public async Task GetById_ShouldReturnPerfil()
    {
        // Arrange
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
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(perfil);

        // Act
        var result = await _businessLogic.GetById(Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(perfil, result);
    }

    [Fact]
    public async Task GetById_ShouldThrowAnNotFoundException()
    {
        _mockRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .Throws(new KeyNotFoundException("Perfil não encontrado"));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _businessLogic.GetById(Guid.NewGuid())
        );

        // Assertions and verifications
        Assert.Equal("Perfil não encontrado", exception.Message);
        _mockRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetWhenContainsAsync_ShouldReturnFilteredPerfils()
    {
        // Arrange
        var perfis = new List<Perfil>
        {
            Perfil.Create(
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
            ),
            Perfil.Create(
                new CreatePerfilRequest(
                    Nome: "Test2",
                    Token: "12345qwerty",
                    UserName: "test2",
                    Linkedin: "linkedin.com/test2",
                    GitHub: "github.com/test2",
                    Bio: "Teste de bio",
                    Email: "test2@test.com",
                    Descricao: "Teste de descrição",
                    Experiencia: ExperienceLevel.Entre1E3Anos
                )
            ),
        };
        _mockRepository
            .Setup(repo => repo.GetWhenContainsAsync(It.IsAny<string>()))
            .ReturnsAsync(perfis);

        // Act
        var result = await _businessLogic.GetWhenContainsAsync("keyword");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(perfis.Count, result.Count);
    }

    [Fact]
    public async Task GetWhenContainsAsync_ThrowsNotFoundException()
    {
        _mockRepository
            .Setup(repo => repo.GetWhenContainsAsync(It.IsAny<string>()))
            .Throws(new KeyNotFoundException("Perfil não encontrado"));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _businessLogic.GetWhenContainsAsync("donotcontain")
        );

        Assert.Equal("Perfil não encontrado", exception.Message);
        _mockRepository.Verify(repo => repo.GetWhenContainsAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetByToken_ShouldReturnPerfil()
    {
        // Arrange
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

        _mockRepository
            .Setup(repo => repo.GetByTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(perfil);

        // Act
        var result = await _businessLogic.GetByToken("token");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(perfil, result);
    }

    [Fact]
    public async Task GetByToken_ShouldThrowNotFoundException()
    {
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

        _mockRepository
            .Setup(repo => repo.GetByTokenAsync(It.IsAny<string>()))
            .Throws(new KeyNotFoundException("Perfil não encontrado"));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _businessLogic.GetByToken("random")
        );

        Assert.Equal("Perfil não encontrado", exception.Message);
        _mockRepository.Verify(repo => repo.GetByTokenAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task AddPerfil_ShouldInvokeAddAsync()
    {
        // Arrange
        var createPerfilRequest = new CreatePerfilRequest(
            Nome: "Test",
            Token: "12345qwerty",
            UserName: "test",
            Linkedin: "linkedin.com/test",
            GitHub: "github.com/test",
            Bio: "Teste de bio",
            Email: "test@test.com",
            Descricao: "Teste de descrição",
            Experiencia: ExperienceLevel.Entre1E3Anos
        );

        _mockRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Perfil>()))
            .Returns(Task.CompletedTask);

        // Act
        await _businessLogic.AddPerfil(createPerfilRequest);

        // Assert
        _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<Perfil>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePerfil_ShouldInvokeUpdateAsync()
    {
        // Arrange
        var updatePerfilRequest = new UpdatePerfilRequest(
            Guid.NewGuid(),
            "Novo Nome",
            "test",
            "linkedin.com/test",
            "github.com/test",
            "Teste de bio",
            "Teste de descrição",
            ExperienceLevel.Entre1E3Anos
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
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(perfil);
        _mockRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Perfil>()))
            .Returns(Task.CompletedTask);

        // Act
        await _businessLogic.UpdatePerfil(updatePerfilRequest);

        // Assert
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Perfil>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePerfil_ShouldThrowNotFoundException()
    {
        var updatePerfilRequest = new UpdatePerfilRequest(
            Guid.NewGuid(),
            "Novo Nome",
            "test",
            "linkedin.com/test",
            "github.com/test",
            "Teste de bio",
            "Teste de descrição",
            ExperienceLevel.Entre1E3Anos
        );

        _mockRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new KeyNotFoundException("Perfil não encontrado"));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _businessLogic.UpdatePerfil(updatePerfilRequest)
        );

        Assert.Equal("Perfil não encontrado", exception.Message);
        _mockRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetByIds_ShoudInvokeGetByIdsAsync()
    {
        var profiles = new List<Perfil>
        {
            Perfil.Create(
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
            ),
            Perfil.Create(
                new CreatePerfilRequest(
                    Nome: "Test2",
                    Token: "12345qwerty",
                    UserName: "test2",
                    Linkedin: "linkedin.com/test2",
                    GitHub: "github.com/test2",
                    Bio: "Teste de bio",
                    Email: "test2@test.com",
                    Descricao: "Teste de descrição",
                    Experiencia: ExperienceLevel.Entre1E3Anos
                )
            ),
        };
        _mockRepository
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(profiles);

        var result = await _businessLogic.GetByIds([Guid.NewGuid(), Guid.NewGuid()]);

        Assert.NotNull(result);
        Assert.Equal(profiles, result);

        _mockRepository.Verify(repo => repo.GetByIdsAsync(It.IsAny<List<Guid>>()), Times.Once);
    }

    [Fact]
    public async Task GetByIds_ShouldThrowNotFoundException()
    {
        _mockRepository
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<List<Guid>>()))
            .Throws(new KeyNotFoundException("Perfil não encontrado"));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _businessLogic.GetByIds([Guid.NewGuid(), Guid.NewGuid()])
        );

        Assert.Equal("Perfil não encontrado", exception.Message);
        _mockRepository.Verify(repo => repo.GetByIdsAsync(It.IsAny<List<Guid>>()), Times.Once);
    }

    [Fact]
    public async Task GetByUsername_ShouldReturnPerfil()
    {
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

        _mockRepository
            .Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(perfil);

        var result = await _businessLogic.GetByUsername("test");
        Assert.NotNull(result);
        Assert.Equal(perfil, result);

        _mockRepository.Verify(repo => repo.GetByUsernameAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetByUsername_ShouldReturnNotFoundException()
    {
        _mockRepository
            .Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>()))
            .ThrowsAsync(new KeyNotFoundException("Perfil não encontrado"));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _businessLogic.GetByUsername("random")
        );

        Assert.Equal("Perfil não encontrado", exception.Message);
        _mockRepository.Verify(repo => repo.GetByUsernameAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFotoPerfil_ShouldInvokeUpdateAsync()
    {
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

        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(perfil);

        _mockRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Perfil>()))
            .Returns(Task.CompletedTask);

        await _businessLogic.UpdateFotoPerfil(perfil.Id, "/fakepath");

        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Perfil>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFotoPerfil_ShouldThrowNotFoundException()
    {
        _mockRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .Throws(new KeyNotFoundException("Perfil não encontrado"));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _businessLogic.UpdateFotoPerfil(Guid.NewGuid(), "/fakepath")
        );

        Assert.Equal("Perfil não encontrado", exception.Message);
        _mockRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task TryCreateOrUpdate_ShouldReturnStatusPerfilCreated()
    {
        var request = new CreateOrUpdatePerfilRequest(
            Nome: "Test",
            Token: "12345qwerty",
            UserName: "test",
            Linkedin: "linkedin.com/test",
            GitHub: "github.com/test",
            Bio: "Teste de bio",
            Email: "test@test.com",
            Descricao: "Teste de descrição",
            Experiencia: ExperienceLevel.Entre1E3Anos
        );

        _mockRepository
            .Setup(repo => repo.GetByTokenAsync(request.Token))
            .ReturnsAsync((Perfil)null);

        _mockRepository
            .Setup(repo => repo.GetByUsernameAsync(request.UserName))
            .ReturnsAsync((Perfil)null);

        _mockRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Perfil>()))
            .Returns(Task.CompletedTask);

        var response = await _businessLogic.TryCreateOrUpdate(request);

        _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<Perfil>()), Times.Once);

        _mockMessagePublisher.Verify(
            publisher => publisher.PublishAsync("NotificationsQueue", It.IsAny<Notification>()),
            Times.Once
        );

        Assert.Equal(StatusCreateOrUpdatePerfil.PerfilCreated, response.status);
    }

    [Fact]
    public async Task TryCreateOrUpdate_ShouldReturnStatusPerfilUpdated()
    {
        var request = new CreateOrUpdatePerfilRequest(
            Nome: "Test",
            Token: "12345qwerty",
            UserName: "test",
            Linkedin: "linkedin.com/test",
            GitHub: "github.com/test",
            Bio: "Teste de bio",
            Email: "test@test.com",
            Descricao: "Teste de descrição",
            Experiencia: ExperienceLevel.Entre1E3Anos
        );

        var perfil = Perfil.Create(request);

        var updateRequest = new CreateOrUpdatePerfilRequest(
            Nome: "Novo nome",
            Token: perfil.Token,
            UserName: "Novo username",
            Linkedin: perfil.Linkedin,
            GitHub: perfil.GitHub,
            Bio: perfil.Bio,
            Email: perfil.Email,
            Descricao: perfil.Descricao,
            Experiencia: perfil.Experiencia
        );

        _mockRepository
            .Setup(repo => repo.GetByTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(perfil);

        var response = await _businessLogic.TryCreateOrUpdate(updateRequest);

        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Perfil>()), Times.Once);

        Assert.Equal(StatusCreateOrUpdatePerfil.PerfilUpdated, response.status);
        Assert.Equal(perfil.Id, response.Id);
    }

    [Fact]
    public async Task TryCreateOrUpdate_ShouldReturnStatusUsernameAlreadyInUse()
    {
        var request = new CreateOrUpdatePerfilRequest(
            Nome: "Test",
            Token: "12345qwerty",
            UserName: "test",
            Linkedin: "linkedin.com/test",
            GitHub: "github.com/test",
            Bio: "Teste de bio",
            Email: "test@test.com",
            Descricao: "Teste de descrição",
            Experiencia: ExperienceLevel.Entre1E3Anos
        );

        _mockRepository
            .Setup(repo => repo.GetByTokenAsync(request.Token))
            .ReturnsAsync((Perfil)null);

        var existingPerfil = Perfil.Create(request);
        _mockRepository
            .Setup(repo => repo.GetByUsernameAsync(request.UserName))
            .ReturnsAsync(existingPerfil);

        var response = await _businessLogic.TryCreateOrUpdate(request);

        _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<Perfil>()), Times.Never);
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Perfil>()), Times.Never);

        Assert.Equal(StatusCreateOrUpdatePerfil.UsernameAlreadyInUse, response.status);
        Assert.Equal(Guid.Empty, response.Id);
    }
}
