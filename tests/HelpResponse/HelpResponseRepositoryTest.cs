using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;

namespace tests;

public class HelpResponseRepositoryTest
{
    private readonly HelpResponseRepository _repository;
    private readonly ApplicationDbContext _context;

    public HelpResponseRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new HelpResponseRepository(_context);
    }

    [Fact]
    public async Task Create_ShouldReturnHelpResponse()
    {
        var request = new CreateHelpResponse(
            timeSelectionId: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            Conteudo: "Conteudo da mensagem de teste"
        );

        var helpResponse = HelpResponse.Create(
            request.timeSelectionId,
            request.perfilId,
            request.Conteudo
        );

        await _repository.AddAsync(helpResponse);

        var result = await _context.HelpResponses.FirstOrDefaultAsync();

        Assert.NotNull(result);
        Assert.Equal(helpResponse.Id, result.Id);
        Assert.Equal(helpResponse.TimeSelectionId, result.TimeSelectionId);
        Assert.Equal(helpResponse.PerfilId, result.PerfilId);
        Assert.Equal(helpResponse.Conteudo, result.Conteudo);
        Assert.Equal(helpResponse.CreatedAt, result.CreatedAt);
        Assert.Equal(helpResponse.ResponseStatus, result.ResponseStatus);
    }

    [Fact]
    public async Task Update_ShouldUpdateTheStatusToDeleted()
    {
        var request = new CreateHelpResponse(
            timeSelectionId: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            Conteudo: "Teste de deletar comentario."
        );

        var helpResponse = HelpResponse.Create(
            request.timeSelectionId,
            request.perfilId,
            request.Conteudo
        );

        await _repository.AddAsync(helpResponse);

        var result =
            await _context.HelpResponses.FirstOrDefaultAsync() ?? throw new ArgumentNullException();
        result.DeleteResponse();

        await _repository.UpdateAsync(result);

        var helpResponseDeleted = _context.HelpResponses.FirstOrDefault();

        Assert.NotNull(helpResponseDeleted);
        Assert.Equal(nameof(ResponseStatus.Deleted), helpResponseDeleted.ResponseStatus.ToString());
    }

    [Fact]
    public async Task GetAllById_ShouldReturnHelpReponsesById()
    {
        var timeSelectionId = Guid.NewGuid();
        var request = new CreateHelpResponse(
            timeSelectionId: timeSelectionId,
            perfilId: Guid.NewGuid(),
            Conteudo: "Teste de deletar comentario."
        );

        var newRequest = new CreateHelpResponse(
            timeSelectionId: timeSelectionId,
            perfilId: Guid.NewGuid(),
            Conteudo: "Novo comentario do mesmo usuario"
        );

        _context.HelpResponses.AddRange(
            HelpResponse.Create(request.timeSelectionId, request.perfilId, request.Conteudo),
            HelpResponse.Create(
                newRequest.timeSelectionId,
                newRequest.perfilId,
                newRequest.Conteudo
            )
        );
        _context.SaveChanges();

        var result = await _repository.GetAllAsync(timeSelectionId);
        bool isOrdered = result
            .Zip(result.Skip(1), (current, next) => current.CreatedAt >= next.CreatedAt)
            .All(x => x);

        Assert.NotNull(result);
        Assert.True(result.Count == 2);
        Assert.True(isOrdered);
        Assert.True(result.Select(hr => hr.TimeSelectionId == timeSelectionId).Count() == 2);
    }

    [Fact]
    public async Task GetById_OnlyUndeletedCommentsShouldBeReturned()
    {
        var timeSelectionId = Guid.NewGuid();
        var request = new CreateHelpResponse(
            timeSelectionId: timeSelectionId,
            perfilId: Guid.NewGuid(),
            Conteudo: "Teste de deletar comentario."
        );

        var newRequest = new CreateHelpResponse(
            timeSelectionId: timeSelectionId,
            perfilId: Guid.NewGuid(),
            Conteudo: "Novo comentario do mesmo usuario"
        );

        var helpResponseToBeDeleted = HelpResponse.Create(
            newRequest.timeSelectionId,
            newRequest.perfilId,
            newRequest.Conteudo
        );
        helpResponseToBeDeleted.DeleteResponse();

        _context.HelpResponses.AddRange(
            HelpResponse.Create(request.timeSelectionId, request.perfilId, request.Conteudo),
            helpResponseToBeDeleted
        );
        _context.SaveChanges();

        var result = await _repository.GetAllAsync(timeSelectionId);

        Assert.NotNull(result);
        Assert.True(result.Count == 1);
        Assert.NotEqual(
            result.First().ResponseStatus.ToString(),
            ResponseStatus.Deleted.ToString()
        );
    }

    [Fact]
    public async Task GetById_MustReturnByProfileId()
    {
        var request = new CreateHelpResponse(
            timeSelectionId: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            Conteudo: "Teste de deletar comentario."
        );

        var newRequest = new CreateHelpResponse(
            timeSelectionId: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            Conteudo: "Novo comentario do mesmo usuario"
        );

        var helpResponse = HelpResponse.Create(
            newRequest.timeSelectionId,
            newRequest.perfilId,
            newRequest.Conteudo
        );

        _context.HelpResponses.AddRange(
            HelpResponse.Create(request.timeSelectionId, request.perfilId, request.Conteudo),
            helpResponse
        );
        _context.SaveChanges();

        var result = await _repository.GetById(helpResponse.Id);

        Assert.NotNull(result);
        Assert.Equal(result.Id, helpResponse.Id);
    }
}
