using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Enums;
using Infrastructure.FileHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.EndPoints;

namespace tests;

public class PerfilsEndPointsTests
{
    private readonly Mock<IPerfilBusinessLogic> mockLogic;

    public PerfilsEndPointsTests()
    {
        mockLogic = new Mock<IPerfilBusinessLogic>();
    }

    [Fact]
    public async Task Add_ShouldReturnOk_WhenPerfilIsAdded()
    {
        var request = new CreatePerfilRequest(
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

        mockLogic.Setup(logic => logic.AddPerfil(request)).Returns(Task.CompletedTask);

        var result = await PerfilsEndPoints.Add(mockLogic.Object, request);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task Add_ShouldReturnBadRequest_OnException()
    {
        var request = new CreatePerfilRequest(
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
        mockLogic
            .Setup(logic => logic.AddPerfil(request))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await PerfilsEndPoints.Add(mockLogic.Object, request);

        Assert.IsType<BadRequest<string>>(result);
    }

    // Exemplo para GetById
    [Fact]
    public async Task GetById_ShouldReturnOk_WithPerfil()
    {
        var perfilId = Guid.NewGuid();
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
        mockLogic.Setup(logic => logic.GetById(perfilId)).ReturnsAsync(perfil);

        var result = await PerfilsEndPoints.GetById(mockLogic.Object, perfilId);

        Assert.IsType<Ok<Perfil>>(result);
        var okResult = result as Ok<Perfil>;
        Assert.Equal(perfil, okResult?.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnBadRequest_OnException()
    {
        var perfilId = Guid.NewGuid();

        mockLogic
            .Setup(logic => logic.GetById(perfilId))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await PerfilsEndPoints.GetById(mockLogic.Object, perfilId);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenPerfilIsUpdated()
    {
        var request = new UpdatePerfilRequest(
            Guid.NewGuid(),
            "Novo nome",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            ExperienceLevel.Entre1E3Anos
        );
        mockLogic.Setup(logic => logic.UpdatePerfil(request)).Returns(Task.CompletedTask);

        var result = await PerfilsEndPoints.Update(mockLogic.Object, request);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_OnException()
    {
        var request = new UpdatePerfilRequest(
            Guid.NewGuid(),
            "Novo nome",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            ExperienceLevel.Entre1E3Anos
        );
        mockLogic
            .Setup(logic => logic.UpdatePerfil(request))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await PerfilsEndPoints.Update(mockLogic.Object, request);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task UpdateFoto_ShouldReturnOk_WhenFotoIsUpdated()
    {
        var id = Guid.NewGuid();
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.ContentType).Returns("image/jpeg");
        var file = fileMock.Object;

        var mockSaveFile = new Mock<ISaveFile>();

        mockLogic
            .Setup(logic => logic.UpdateFotoPerfil(id, It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await PerfilsEndPoints.UpdateFoto(
            mockLogic.Object,
            id,
            file,
            mockSaveFile.Object
        );

        Assert.IsType<Ok>(result);
    }

    // Teste para GetByToken
    [Fact]
    public async Task GetByToken_ShouldReturnOk_WithPerfil()
    {
        var token = "testToken";
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
        mockLogic.Setup(logic => logic.GetByToken(token)).ReturnsAsync(perfil);

        var result = await PerfilsEndPoints.GetByToken(mockLogic.Object, token);

        Assert.IsType<Ok<Perfil>>(result);
        var okResult = result as Ok<Perfil>;
        Assert.Equal(perfil, okResult?.Value);
    }

    // Teste para GetAll
    [Fact]
    public async Task GetAll_ShouldReturnOk_WithPerfils()
    {
        var perfils = new List<Perfil>
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
                    Nome: "Test",
                    Token: "12345qwerty2",
                    UserName: "test2",
                    Linkedin: "linkedin.com/test",
                    GitHub: "github.com/test",
                    Bio: "Teste de bio",
                    Email: "test@test.com",
                    Descricao: "Teste de descrição",
                    Experiencia: ExperienceLevel.Entre1E3Anos
                )
            )
        };
        mockLogic.Setup(logic => logic.GetAll()).ReturnsAsync(perfils);

        var result = await PerfilsEndPoints.GetAll(mockLogic.Object);

        Assert.IsType<Ok<List<Perfil>>>(result);
        var okResult = result as Ok<List<Perfil>>;

        Assert.Equal(perfils, okResult?.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_OnException()
    {
        mockLogic.Setup(logic => logic.GetAll()).ThrowsAsync(new Exception("Erro de teste"));

        var result = await PerfilsEndPoints.GetAll(mockLogic.Object);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task UpdateFoto_ReturnsBadRequest_IfFileTypeIsNotImage()
    {
        // Arrange
        var mockLogic = new Mock<IPerfilBusinessLogic>();
        var file = new Mock<IFormFile>();
        file.Setup(f => f.ContentType).Returns("application/pdf"); // Set a non-image content type
        var id = Guid.NewGuid();

        var mockSaveFile = new Mock<ISaveFile>();
        // Act
        var result = await PerfilsEndPoints.UpdateFoto(
            mockLogic.Object,
            id,
            file.Object,
            mockSaveFile.Object
        );

        // Assert
        Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("O arquivo enviado não é uma imagem.", (result as BadRequest<string>)?.Value);
    }

    [Fact]
    public async Task UpdateFoto_ReturnsBadRequest_OnException()
    {
        // Arrange
        var mockLogic = new Mock<IPerfilBusinessLogic>();
        var file = new Mock<IFormFile>();
        file.Setup(f => f.ContentType).Returns("image/png"); // Set an image content type
        var id = Guid.NewGuid();

        var mockSaveFile = new Mock<ISaveFile>();

        mockLogic
            .Setup(logic => logic.UpdateFotoPerfil(id, It.IsAny<string>()))
            .ThrowsAsync(new Exception("Erro durante a atualização da foto"));

        // Act
        var result = await PerfilsEndPoints.UpdateFoto(
            mockLogic.Object,
            id,
            file.Object,
            mockSaveFile.Object
        );

        // Assert
        Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Erro durante a atualização da foto", (result as BadRequest<string>)?.Value);
    }

    [Fact]
    public async Task GetAllByIds_ReturnsOkResult_WithValidIds()
    {
        // Arrange
        var mockLogic = new Mock<IPerfilBusinessLogic>();
        var validIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        mockLogic.Setup(logic => logic.GetByIds(validIds)).ReturnsAsync(new List<Perfil>());

        // Act
        var result = await PerfilsEndPoints.GetAllByIds(mockLogic.Object, validIds);

        // Assert
        Assert.IsType<Ok<List<Perfil>>>(result);
        Assert.Empty((result as Ok<List<Perfil>>)?.Value!);
    }

    [Fact]
    public async Task GetAllByIds_ReturnsBadRequest_WithInvalidIds()
    {
        // Arrange
        var mockLogic = new Mock<IPerfilBusinessLogic>();
        var invalidIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        mockLogic
            .Setup(logic => logic.GetByIds(invalidIds))
            .ThrowsAsync(new Exception("Invalid Ids"));

        // Act
        var result = await PerfilsEndPoints.GetAllByIds(mockLogic.Object, invalidIds);

        // Assert
        Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Invalid Ids", (result as BadRequest<string>)?.Value);
    }

    [Fact]
    public async Task GetByUsername_ReturnsOkResult_WithValidUsername()
    {
        // Arrange
        var mockLogic = new Mock<IPerfilBusinessLogic>();
        var validUsername = "test";

        mockLogic
            .Setup(logic => logic.GetByUsername(validUsername))
            .ReturnsAsync(
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
                )
            );

        // Act
        var result = await PerfilsEndPoints.GetByUsername(mockLogic.Object, validUsername);

        // Assert
        Assert.IsType<Ok<Perfil>>(result);
        Assert.NotNull((result as Ok<Perfil>)?.Value);
    }

    [Fact]
    public async Task GetByUsername_ReturnsBadRequest_WithInvalidUsername()
    {
        // Arrange
        var mockLogic = new Mock<IPerfilBusinessLogic>();
        var invalidUsername = "invalid";

        mockLogic
            .Setup(logic => logic.GetByUsername(invalidUsername))
            .ThrowsAsync(new Exception("Invalid Username"));

        // Act
        var result = await PerfilsEndPoints.GetByUsername(mockLogic.Object, invalidUsername);

        // Assert
        Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Invalid Username", (result as BadRequest<string>)?.Value);
    }

    [Fact]
    public async Task GetByToken_ReturnsOkResult_WithValidToken()
    {
        // Arrange
        var mockLogic = new Mock<IPerfilBusinessLogic>();
        var validToken = "token123";

        mockLogic
            .Setup(logic => logic.GetByToken(validToken))
            .ReturnsAsync(
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
                )
            );

        // Act
        var result = await PerfilsEndPoints.GetByToken(mockLogic.Object, validToken);

        // Assert
        Assert.IsType<Ok<Perfil>>(result);
        Assert.NotNull((result as Ok<Perfil>)?.Value);
    }

    [Fact]
    public async Task GetByToken_ReturnsBadRequest_WithInvalidToken()
    {
        // Arrange
        var mockLogic = new Mock<IPerfilBusinessLogic>();
        var invalidToken = "invalidToken";

        mockLogic
            .Setup(logic => logic.GetByToken(invalidToken))
            .ThrowsAsync(new Exception("Invalid Token"));

        // Act
        var result = await PerfilsEndPoints.GetByToken(mockLogic.Object, invalidToken);

        // Assert
        Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Invalid Token", (result as BadRequest<string>)?.Value);
    }

    [Fact]
    public async Task GetWhenContains_ReturnsOkResult_WithValidKeyword()
    {
        // Arrange
        var mockLogic = new Mock<IPerfilBusinessLogic>();
        var validKeyword = "keyword";

        mockLogic
            .Setup(logic => logic.GetWhenContainsAsync(validKeyword))
            .ReturnsAsync(new List<Perfil>());

        // Act
        var result = await PerfilsEndPoints.GetWhenContains(mockLogic.Object, validKeyword);

        // Assert
        Assert.IsType<Ok<List<Perfil>>>(result);
        Assert.Empty((result as Ok<List<Perfil>>)?.Value!);
    }

    [Fact]
    public async Task GetWhenContains_ReturnsBadRequest_WithInvalidKeyword()
    {
        // Arrange
        var mockLogic = new Mock<IPerfilBusinessLogic>();
        var invalidKeyword = "invalidKeyword";

        mockLogic
            .Setup(logic => logic.GetWhenContainsAsync(invalidKeyword))
            .ThrowsAsync(new Exception("Invalid Keyword"));

        // Act
        var result = await PerfilsEndPoints.GetWhenContains(mockLogic.Object, invalidKeyword);

        // Assert
        Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Invalid Keyword", (result as BadRequest<string>)?.Value);
    }
}
