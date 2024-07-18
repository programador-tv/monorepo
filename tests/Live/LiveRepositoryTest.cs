using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using static MassTransit.Util.ChartTable;

namespace tests;

public class LiveRepositoryTests
{
    private readonly LiveRepository _repository;
    private readonly ApplicationDbContext _context;

    public LiveRepositoryTests()
    {
        var live = Live.Create(
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

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new LiveRepository(_context);

        if (!_context.Lives.Any())
        {
            _context.Lives.AddRange(new List<Live> { live });
            _context.SaveChanges();
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnLiveById()
    {
        var expectedLive = _context.Lives.First();

        var result = await _repository.GetByIdAsync(expectedLive.Id);

        Assert.NotNull(result);
        Assert.Equal(expectedLive.Id, result.Id);
    }

    [Fact]
    public async Task GetLiveByUrl_ShouldReturnALive()
    {
        var expectedLive = _context.Lives.First();

        var result = await _repository.GetByUrlAsync(expectedLive.UrlAlias ?? string.Empty);

        Assert.NotNull(result);
        Assert.Equal(expectedLive.UrlAlias, result.UrlAlias);
    }

    [Fact]
    public async Task AddAsync_ShouldAddALive()
    {
        var live = Live.Create(
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

        await _repository.AddAsync(live);
        var result = await _repository.GetByIdAsync(live.Id);

        Assert.NotNull(result);
        Assert.Equal(live.Id, result.Id);
    }

    [Fact]
    public async Task UpdateRangeNotifyUserLiveEarly_ValidList_UpdatesData()
    {
        var live = Live.Create(
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

        var notifyUsers = new List<NotifyUserLiveEarly>
        {
            NotifyUserLiveEarly.Create(live.Id, Guid.NewGuid(), true, false),
            NotifyUserLiveEarly.Create(live.Id, Guid.NewGuid(), true, false),
            NotifyUserLiveEarly.Create(live.Id, Guid.NewGuid(), true, false)
        };

        await _context.Lives.AddAsync(live);
        await _context.NotifyUserLiveEarlies.AddRangeAsync(notifyUsers);
        await _context.SaveChangesAsync();

        notifyUsers[0].MarkAsNotificated();

        await _repository.UpdateRangeNotifyUserLiveEarly(notifyUsers);

        var notifyUsersFromDb = await _context
            .NotifyUserLiveEarlies.Where(n => n.LiveId == live.Id)
            .ToListAsync();

        Assert.True(notifyUsersFromDb[0].HasNotificated);
        Assert.False(notifyUsersFromDb[1].HasNotificated);
        Assert.False(notifyUsersFromDb[2].HasNotificated);
    }

    [Fact]
    public async Task UpdateRangeNotifyUserLiveEarly_EmptyList_NoExceptionThrown()
    {
        var emptyList = new List<NotifyUserLiveEarly>();

        var exception = await Record.ExceptionAsync(
            async () => await _repository.UpdateRangeNotifyUserLiveEarly(emptyList)
        );

        Assert.Null(exception);
    }

    [Fact]
    public async Task GetUpcomingLives_WorksFine()
    {
        _context.TimeSelections.RemoveRange(_context.TimeSelections);
        _context.LiveBackstages.RemoveRange(_context.LiveBackstages);
        _context.NotifyUserLiveEarlies.RemoveRange(_context.NotifyUserLiveEarlies);
        await _context.SaveChangesAsync();

        var liveTimeSelection = TimeSelection.Create(
            perfilId: Guid.NewGuid(),
            roomId: null,
            startTime: DateTime.Now.AddMinutes(20),
            endTime: DateTime.Now.AddMinutes(40),
            tituloTemporario: "Teste de Titulo",
            tipo: EnumTipoTimeSelection.Live,
            tipoAction: TipoAction.Ensinar,
            variacao: Variacao.OneToOne
        );

        var nonLiveTimeSelection = TimeSelection.Create(
            perfilId: Guid.NewGuid(),
            roomId: null,
            startTime: DateTime.Now.AddMinutes(20),
            endTime: DateTime.Now.AddMinutes(40),
            tituloTemporario: "Teste de Titulo",
            tipo: EnumTipoTimeSelection.FreeTime,
            tipoAction: TipoAction.Ensinar,
            variacao: Variacao.OneToOne
        );

        _context.TimeSelections.AddRange(nonLiveTimeSelection, liveTimeSelection);

        var live = Live.Create(
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
        _context.Lives.Add(live);

        var liveBackstage = LiveBackstage.Create(
            liveTimeSelection.Id,
            live.Id,
            live.Titulo,
            live.Descricao
        );
        _context.LiveBackstages.Add(liveBackstage);

        var notifyUser1 = NotifyUserLiveEarly.Create(live.Id, Guid.NewGuid(), true, false);
        var notifyUser2 = NotifyUserLiveEarly.Create(live.Id, Guid.NewGuid(), true, false);
        _context.NotifyUserLiveEarlies.AddRange(notifyUser1, notifyUser2);

        await _context.SaveChangesAsync();

        var result = await _repository.GetUpcomingLives();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Single(result);
        Assert.Equal(live.Id, result.Keys.First().Id);
        Assert.Equal(2, result.Values.First().Count);
        var notifyUsers = result.Values.First();
        var n1Exists = notifyUsers.Any(n => n.Id == notifyUser1.Id);
        var n2Exists = notifyUsers.Any(n => n.Id == notifyUser2.Id);
        Assert.True(n1Exists);
        Assert.True(n2Exists);
    }

    [Fact]
    public async Task GetUpcomingLives_ReturnsEmptyWhenHaveNotNotifyUser()
    {
        _context.TimeSelections.RemoveRange(_context.TimeSelections);
        _context.LiveBackstages.RemoveRange(_context.LiveBackstages);
        _context.NotifyUserLiveEarlies.RemoveRange(_context.NotifyUserLiveEarlies);
        await _context.SaveChangesAsync();

        var liveTimeSelection = TimeSelection.Create(
            perfilId: Guid.NewGuid(),
            roomId: null,
            startTime: DateTime.Now.AddMinutes(20),
            endTime: DateTime.Now.AddMinutes(40),
            tituloTemporario: "Teste de Titulo",
            tipo: EnumTipoTimeSelection.Live,
            tipoAction: TipoAction.Ensinar,
            variacao: Variacao.OneToOne
        );

        var nonLiveTimeSelection = TimeSelection.Create(
            perfilId: Guid.NewGuid(),
            roomId: null,
            startTime: DateTime.Now.AddMinutes(20),
            endTime: DateTime.Now.AddMinutes(40),
            tituloTemporario: "Teste de Titulo",
            tipo: EnumTipoTimeSelection.FreeTime,
            tipoAction: TipoAction.Ensinar,
            variacao: Variacao.OneToOne
        );

        _context.TimeSelections.AddRange(nonLiveTimeSelection, liveTimeSelection);

        var live = Live.Create(
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
        _context.Lives.Add(live);

        var liveBackstage = LiveBackstage.Create(
            liveTimeSelection.Id,
            live.Id,
            live.Titulo,
            live.Descricao
        );
        _context.LiveBackstages.Add(liveBackstage);

        await _context.SaveChangesAsync();

        var result = await _repository.GetUpcomingLives();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateLive()
    {
        var live = _context.Lives.First();
        var newThumbnail = "update thumbnail";

        live.AtualizaThumbnail(newThumbnail);
        await _repository.UpdateAsync(live);

        var updatedLive = await _context.Lives.FindAsync(live.Id);
        Assert.NotNull(updatedLive);
        Assert.Equal(newThumbnail, updatedLive.Thumbnail);
    }

    // [Fact]
    // public async Task GetOpenLives_ReturnsOpenLives()
    // {
    //     var mockLive = new CreateLiveRequest(
    //         PerfilId: Guid.NewGuid(),
    //         Titulo: "Teste 1",
    //         Descricao: "Teste de descrição 1",
    //         Thumbnail: "thumbnail teste",
    //         IsUsingObs: false,
    //         StreamId: Guid.NewGuid().ToString(),
    //         UrlAlias: "Test-efb58h-1"
    //     );

    //     var live1 = Live.Create(mockLive);
    //     live1.Inicia();

    //     var live2 = Live.Create(mockLive);
    //     live2.Inicia();

    //     var live3 = Live.Create(mockLive);

    //     _context.Lives.AddRange(live1, live2, live3);

    //     await _context.SaveChangesAsync();

    //     var result = await _repository.GetOpenLives();

    //     Assert.NotNull(result);
    //     Assert.Equal(2, result.Count);
    //     Assert.Contains(result, l => l.Id == live1.Id);
    //     Assert.Contains(result, l => l.Id == live2.Id);
    // }

    [Fact]
    public async Task UpdateRangeAsync_UpdatesMultipleLives()
    {
        var live1 = Live.Create(
            new CreateLiveRequest(
                PerfilId: Guid.NewGuid(),
                Titulo: "Teste 2",
                Descricao: "Teste de descrição 2",
                Thumbnail: "thumbnail teste",
                IsUsingObs: false,
                StreamId: Guid.NewGuid().ToString(),
                UrlAlias: "Test-efb58h-2"
            )
        );

        var live2 = Live.Create(
            new CreateLiveRequest(
                PerfilId: Guid.NewGuid(),
                Titulo: "Teste 3",
                Descricao: "Teste de descrição 3",
                Thumbnail: "thumbnail teste",
                IsUsingObs: false,
                StreamId: Guid.NewGuid().ToString(),
                UrlAlias: "Test-efb58h-3"
            )
        );

        var lives = new List<Live> { live1, live2 };

        _context.Lives.AddRange(lives);

        await _context.SaveChangesAsync();

        var updatedLives = lives.Where(l => l.Descricao == "updated description").ToList();

        await _repository.UpdateRangeAsync(updatedLives);

        var updatedLive1 = await _context.Lives.FindAsync(live1.Id);
        var updatedLive2 = await _context.Lives.FindAsync(live2.Id);

        Assert.NotNull(updatedLive1);
        Assert.NotNull(updatedLive2);

        Assert.Equal(live1.Id, updatedLive1.Id);
        Assert.Equal(live2.Id, updatedLive2.Id);
    }

    [Fact]
    public async Task GetKeyByStreamId_WhenStreamIdExists_ReturnsCorrespondingId()
    {
        var streamId = Guid.NewGuid().ToString();
        var expectedId = Guid.NewGuid();
        var live = new Live(
            id: expectedId,
            perfilId: Guid.NewGuid(),
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: streamId,
            liveEstaAberta: true,
            titulo: "Test",
            descricao: "Teste de descrição",
            thumbnail: "thumbnail teste",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );
        _context.Lives.Add(live);
        await _context.SaveChangesAsync();

        var result = await _repository.GetKeyByStreamId(streamId);

        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task SearchBySpecificTitle_ReturnsCorrespondingLives()
    {
        var keyword = "Teste";

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
            titulo: "Teste",
            descricao: "Teste de descrição",
            thumbnail: "Thumbnail teste",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );
        _context.Lives.Add(live);
        await _context.SaveChangesAsync();

        var result = await _repository.SearchBySpecificTitle(keyword);

        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        foreach (var item in result)
        {
            Assert.Equal(keyword, item.Titulo);
        }
    }

    [Fact]
    public async Task SearchBySpecificTitle_ReturnsEmptyList()
    {
        var keyword = "TesteListaVazia";
        var result = await _repository.SearchBySpecificTitle(keyword);

        Assert.Equal(0, result.Count);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchByTitleContaining_ReturnsCorrespondingLives()
    {
        var keyword = "Carol";

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
            titulo: "Carolina",
            descricao: "Teste de descrição",
            thumbnail: "Thumbnail teste",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );
        _context.Lives.Add(live);
        await _context.SaveChangesAsync();

        var result = await _repository.SearchByTitleContaining(keyword);

        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        foreach (var item in result)
        {
            Assert.Contains(keyword, item.Titulo, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task SearchByTitleContaining_ReturnsEmptyList()
    {
        var keyword = "TesteListaVazia";
        var result = await _repository.SearchByTitleContaining(keyword);

        Assert.Equal(0, result.Count);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchByDescriptionContaining_ReturnsCorrespondingLives()
    {
        var keyword = "ProgramadorTv";

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
            titulo: "Carolina",
            descricao: "Essa é uma live do ProgramadorTv",
            thumbnail: "Thumbnail teste",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );
        _context.Lives.Add(live);
        await _context.SaveChangesAsync();

        var result = await _repository.SearchByDescriptionContaining(keyword);

        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        foreach (var item in result)
        {
            Assert.Contains(keyword, item.Descricao, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task SearchByDescriptionContaining_ReturnsEmptyList()
    {
        var keyword = "TesteListaVazia";
        var result = await _repository.SearchByDescriptionContaining(keyword);

        Assert.Equal(0, result.Count);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchByTagContaining_ReturnsCorrespondingLives()
    {
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
            titulo: "ProgramadorTv",
            descricao: "Essa é uma live do ProgramadorTv",
            thumbnail: "Thumbnail teste",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        _context.Lives.Add(live);
        await _context.SaveChangesAsync();

        var tag = Tag.AddForLive("TituloDaTag", live.Id.ToString());

        var tags = new List<Tag> { tag };

        var result = await _repository.SearchByTagContaining(tags);

        Assert.NotNull(result);
        Assert.Equal(1, result.Count);

        foreach (var item in result)
        {
            Assert.Equal(item.Id.ToString(), tag.LiveRelacao);
        }
    }

    [Fact]
    public async Task SearchByTagContaining_ReturnsEmptyList()
    {
        var guidLiveId = Guid.NewGuid().ToString();
        var tag = Tag.AddForLive("TituloDaTag", guidLiveId);
        var tags = new List<Tag> { tag };

        var result = await _repository.SearchByTagContaining(tags);

        Assert.Equal(0, result.Count);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchByListPerfilId_ReturnsCorrespondingLives()
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

        var live = new Live(
            id: Guid.NewGuid(),
            perfilId: perfil.Id,
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "ProgramadorTv",
            descricao: "Essa é uma live do ProgramadorTv",
            thumbnail: "Thumbnail teste",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        _context.Lives.Add(live);
        await _context.SaveChangesAsync();

        var perfis = new List<Perfil> { perfil };

        var result = await _repository.SearchByListPerfilId(perfis);

        Assert.NotNull(result);
        Assert.Equal(1, result.Count);

        foreach (var item in result)
        {
            Assert.Equal(item.PerfilId, perfil.Id);
        }
    }

    [Fact]
    public async Task SearchByListPerfilId_ReturnsEmptyList()
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
        var perfis = new List<Perfil> { perfil };

        var result = await _repository.SearchByListPerfilId(perfis);

        Assert.Equal(0, result.Count);
        Assert.Empty(result);
    }
}
