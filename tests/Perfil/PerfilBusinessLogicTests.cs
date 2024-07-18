using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Moq;

namespace tests;

public class PerfilBusinessLogicTests
{
    private readonly PerfilBusinessLogic _businessLogic;
    private readonly Mock<IPerfilRepository> _mockRepository;

    public PerfilBusinessLogicTests()
    {
        _mockRepository = new Mock<IPerfilRepository>();
        _businessLogic = new PerfilBusinessLogic(_mockRepository.Object);
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
            )
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
            )
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
}
