using System.Data.Common;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;

namespace tests;

public class PerfilEntity
{
    [Fact]
    public void Create_GenereteTypeOfPerfil()
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

        var perfil = Perfil.Create(request);

        Assert.IsType<Perfil>(perfil);
        Assert.Equal(request.Nome, perfil.Nome);
        Assert.Equal(request.Token, perfil.Token);
        Assert.Equal(request.UserName, perfil.UserName);
        Assert.Equal(request.Linkedin, perfil.Linkedin);
        Assert.Equal(request.GitHub, perfil.GitHub);
        Assert.Equal(request.Bio, perfil.Bio);
        Assert.Equal(request.Email, perfil.Email);
        Assert.Equal(request.Descricao, perfil.Descricao);
        Assert.Equal(request.Experiencia, perfil.Experiencia);
    }

    [Fact]
    public void Create_GeneretePerfil_WithContractCreateOrUpdatePerfilRequest()
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

        Assert.IsType<Perfil>(perfil);
        Assert.Equal(request.Nome, perfil.Nome);
        Assert.Equal(request.Token, perfil.Token);
        Assert.Equal(request.UserName, perfil.UserName);
        Assert.Equal(request.Linkedin, perfil.Linkedin);
        Assert.Equal(request.GitHub, perfil.GitHub);
        Assert.Equal(request.Bio, perfil.Bio);
        Assert.Equal(request.Email, perfil.Email);
        Assert.Equal(request.Descricao, perfil.Descricao);
        Assert.Equal(request.Experiencia, perfil.Experiencia);
    }

    [Fact]
    public void Update_ShouldUpdatePerfilWithNewValues()
    {
        // Arrange
        var perfil = Perfil.Create(
            new CreatePerfilRequest(
                Nome: "",
                Token: "",
                UserName: "",
                Linkedin: "",
                GitHub: "",
                Bio: "",
                Email: "",
                Descricao: "",
                Experiencia: ExperienceLevel.Entre1E3Anos
            )
        );

        var updateRequest = new UpdatePerfilRequest(
            perfil.Id,
            "Novo nome",
            perfil.UserName,
            perfil.Linkedin ?? string.Empty,
            perfil.GitHub ?? string.Empty,
            perfil.Bio ?? string.Empty,
            perfil.Descricao ?? string.Empty,
            perfil.Experiencia
        );

        // Act
        perfil.Update(updateRequest);

        // Assert
        Assert.Equal(updateRequest.Nome, perfil.Nome);
        Assert.Equal(updateRequest.UserName, perfil.UserName);
        Assert.Equal(updateRequest.Linkedin, perfil.Linkedin);
        Assert.Equal(updateRequest.GitHub, perfil.GitHub);
        Assert.Equal(updateRequest.Bio, perfil.Bio);
        Assert.Equal(updateRequest.Descricao, perfil.Descricao);
        Assert.Equal(updateRequest.Experiencia, perfil.Experiencia);
    }

    [Fact]
    public void UpdateFoto_ShouldUpdateFoto()
    {
        // Arrange
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

        var perfil = Perfil.Create(request);
        var novaFoto = "path/para/nova/foto.jpg";

        // Act
        perfil.UpdateFoto(novaFoto);

        // Assert
        Assert.Equal(novaFoto, perfil.Foto);
    }

    [Fact]
    public void Update_WithContractCreateOrUpdatePerfilRequest()
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
            Token: "NovoToken12345",
            UserName: "Novo username",
            Linkedin: perfil.Linkedin,
            GitHub: perfil.GitHub,
            Bio: perfil.Bio,
            Email: perfil.Email,
            Descricao: perfil.Descricao,
            Experiencia: perfil.Experiencia
        );

        perfil.Update(updateRequest);

        Assert.Equal(updateRequest.Nome, perfil.Nome);
        Assert.Equal(updateRequest.UserName, perfil.UserName);
        Assert.Equal(updateRequest.Linkedin, perfil.Linkedin);
        Assert.Equal(updateRequest.GitHub, perfil.GitHub);
        Assert.Equal(updateRequest.Bio, perfil.Bio);
        Assert.Equal(updateRequest.Descricao, perfil.Descricao);
        Assert.Equal(updateRequest.Experiencia, perfil.Experiencia);
    }
}
