using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace tests;

public class PerfilRepositoryTests
{
    private readonly PerfilRepository _repository;
    private readonly ApplicationDbContext _context;

    public PerfilRepositoryTests()
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

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new PerfilRepository(_context);

        if (!_context.Perfils.Any())
        {
            _context.Perfils.AddRange(new List<Perfil> { perfil });
            _context.SaveChanges();
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPerfils()
    {
        var result = await _repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPerfilById()
    {
        var expectedPerfil = _context.Perfils.First();

        var result = await _repository.GetByIdAsync(expectedPerfil.Id);

        Assert.NotNull(result);
        Assert.Equal(expectedPerfil.Id, result.Id);
    }

    [Fact]
    public async Task GetByTokenAsync_ShouldReturnPerfilByToken()
    {
        var expectedPerfil = _context.Perfils.First();

        var result = await _repository.GetByTokenAsync(expectedPerfil.Token);

        Assert.NotNull(result);
        Assert.Equal(expectedPerfil.Token, result.Token);
    }

    [Fact]
    public async Task GetWhenContainsAsync_ShouldReturnFilteredPerfils()
    {
        _context.Perfils.AddRange(
            Perfil.Create(
                new CreatePerfilRequest(
                    Nome: "parteDoNome Test",
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
                    Token: "12345qwerty",
                    UserName: "parteDoNome test",
                    Linkedin: "linkedin.com/test",
                    GitHub: "github.com/test",
                    Bio: "Teste de bio",
                    Email: "test@test.com",
                    Descricao: "Teste de descrição",
                    Experiencia: ExperienceLevel.Entre1E3Anos
                )
            )
        );
        _context.SaveChanges();

        // Arrange
        var keyword = "parteDoNome";

        // Act
        var result = await _repository.GetWhenContainsAsync(keyword);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, e => e.Nome.Contains(keyword) || e.UserName.Contains(keyword));
        Assert.True(result.Count == 2);
    }

    [Fact]
    public async Task AddAsync_ShouldAddPerfil()
    {
        // Arrange
        var perfil = Perfil.Create(
            new CreatePerfilRequest(
                Nome: "Test 2",
                Token: "12345qwerty",
                UserName: "test2",
                Linkedin: "linkedin.com/test2",
                GitHub: "github.com/test2",
                Bio: "Teste de bio",
                Email: "test2@test.com",
                Descricao: "Teste de descrição",
                Experiencia: ExperienceLevel.Entre1E3Anos
            )
        );

        // Act
        await _repository.AddAsync(perfil);
        var result = await _context.Perfils.FindAsync(perfil.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(perfil.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePerfil()
    {
        var perfil = _context.Perfils.First();
        var request = new UpdatePerfilRequest(
            perfil.Id,
            "Novo Nome",
            perfil.UserName,
            perfil.Linkedin ?? string.Empty,
            perfil.GitHub ?? string.Empty,
            perfil.Bio ?? string.Empty,
            perfil.Descricao ?? string.Empty,
            perfil.Experiencia
        );

        perfil.Update(request);

        await _repository.UpdateAsync(perfil);
        var result = await _context.Perfils.FindAsync(perfil.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Novo Nome", result.Nome);
    }
}
