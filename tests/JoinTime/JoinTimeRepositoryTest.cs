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
    private readonly Guid specificGuid = Guid.Parse("1cc5615c-c82f-4e5c-b2de-df5421c72a82");

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
                    specificGuid,
                    StatusJoinTime.Marcado,
                    false,
                    TipoAction.Aprender
                ),
                JoinTime.Create(
                    Guid.NewGuid(),
                    specificGuid,
                    StatusJoinTime.Pendente,
                    false,
                    TipoAction.Aprender
                ),
                JoinTime.Create(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    StatusJoinTime.Confirmado,
                    false,
                    TipoAction.Aprender
                )
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

    [Fact]
    public async Task GetJoinTimesAtivos_ReturnsCorrectJoinTimes()
    {
        // Arrange: 
        InitializeJoinTimes();

         // Act:
        var result = await _repository.GetJoinTimesAtivos(specificGuid);

        // Assert:
        Assert.NotNull(result);
        Assert.NotEmpty(result); // Verifica se há pelo menos um JoinTime retornado
        Assert.All(result, jt => Assert.Contains(jt.StatusJoinTime, new[] { StatusJoinTime.Marcado, StatusJoinTime.Pendente }));
    }

   [Fact]
    public async Task GetJoinTimesAtivos_ReturnsEmptyList_WhenNoMatchingJoinTimes()
    {
        // Arrange: 
        InitializeJoinTimes();

        // Act: 
        var nonExistentTimeSelectionId = Guid.NewGuid();
        var result = await _repository.GetJoinTimesAtivos(nonExistentTimeSelectionId);

        // Assert: 
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
