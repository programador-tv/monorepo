using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace tests;

public class JoinTimeRepositoryTest
{
    private readonly JoinTimeRepository _repository;
    private readonly ApplicationDbContext _context;

    public JoinTimeRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseJoinTime")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new JoinTimeRepository(_context);

        InitializeJoinTimes();
    }

    private void InitializeJoinTimes()
    {
        _context.JoinTimes.AddRange(
            new List<JoinTime>
            {
                JoinTime.Create(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    StatusJoinTime.Marcado,
                    false,
                    TipoAction.Aprender
                ),
                JoinTime.Create(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    StatusJoinTime.Marcado,
                    false,
                    TipoAction.Aprender
                ),
            }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetJoinTimePerfilIdsByTimeSelectionIds_ReturnsDictionary()
    {
        var tsIds = _context.TimeSelections.Select(ts => ts.Id).ToList();

        var result = await _repository.GetJoinTimePerfilIdsByTimeSelectionIds(tsIds);

        Assert.NotNull(result);
        Assert.IsType<Dictionary<Guid, Guid>>(result);
        Assert.Equal(tsIds.Count, result.Count);
    }

    [Fact]
    public async Task GetFreeTimeMarcadosAntigos_ReturnsDictionary()
    {
        var result = await _repository.GetFreeTimeMarcadosAntigos();

        Assert.NotNull(result);
        Assert.IsType<Dictionary<JoinTime, TimeSelection>>(result);
        Assert.Empty(result);

        var ts = TimeSelection.Create(
            Guid.NewGuid(),
            null,
            DateTime.Now.AddMinutes(-40),
            DateTime.Now.AddMinutes(-20),
            "Teste de descrição",
            EnumTipoTimeSelection.FreeTime,
            TipoAction.Ensinar,
            Variacao.OneToOne
        );
        _context.Add(ts);

        var jt = JoinTime.Create(
            Guid.NewGuid(),
            ts.Id,
            StatusJoinTime.Marcado,
            false,
            TipoAction.Aprender
        );
        _context.Add(jt);

        await _context.SaveChangesAsync();

        result = await _repository.GetFreeTimeMarcadosAntigos();

        Assert.NotNull(result);
        Assert.IsType<Dictionary<JoinTime, TimeSelection>>(result);
        Assert.NotEmpty(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task UpdateRange_UpdatesJoinTimes()
    {
        var joinTimesToUpdate = await _context.JoinTimes.ToListAsync();
        foreach (var joinTime in joinTimesToUpdate)
        {
            joinTime.ChangeStatus(StatusJoinTime.Concluído);
        }

        await _repository.UpdateRange(joinTimesToUpdate);

        var updatedJoinTimes = await _context.JoinTimes.ToListAsync();
        Assert.All(
            updatedJoinTimes,
            jt => Assert.Equal(StatusJoinTime.Concluído, jt.StatusJoinTime)
        );
    }
}
