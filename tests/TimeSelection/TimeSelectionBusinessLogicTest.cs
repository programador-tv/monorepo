using Application.Logic;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Moq;
using Queue;

namespace tests;

public class TimeSelectionBusinessLogicTests
{
    private readonly Mock<ITimeSelectionRepository> mockTimeSelectionRepo;
    private readonly Mock<IJoinTimeRepository> mockJoinTimeRepo;
    private readonly Mock<IPerfilRepository> mockPerfilRepo;
    private readonly Mock<ITagRepository> mockTagRepo;
    private readonly Mock<IMessagePublisher> mockMessagePublisher;
    private readonly TimeSelectionBusinessLogic businessLogic;

    public TimeSelectionBusinessLogicTests()
    {
        mockTimeSelectionRepo = new Mock<ITimeSelectionRepository>();
        mockJoinTimeRepo = new Mock<IJoinTimeRepository>();
        mockMessagePublisher = new Mock<IMessagePublisher>();
        mockPerfilRepo = new Mock<IPerfilRepository>();
        mockTagRepo = new Mock<ITagRepository>();
        businessLogic = new TimeSelectionBusinessLogic(
            mockTimeSelectionRepo.Object,
            mockJoinTimeRepo.Object,
            mockPerfilRepo.Object,
            mockTagRepo.Object,
            mockMessagePublisher.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsTimeSelection()
    {
        var testId = Guid.NewGuid();
        var testTimeSelection = TimeSelection.Create(
            Guid.NewGuid(),
            null,
            DateTime.Now,
            DateTime.Now.AddHours(1),
            "Guid.NewGuid()",
            EnumTipoTimeSelection.FreeTime,
            TipoAction.Ensinar,
            Variacao.OneToOne
        );
        mockTimeSelectionRepo.Setup(repo => repo.GetById(testId)).ReturnsAsync(testTimeSelection);

        var result = await businessLogic.GetById(testId);

        Assert.Equal(testTimeSelection, result);
    }

    [Fact]
    public async Task UpdateOldTimeSelections_UpdatesStatusAndPublishesNotifications()
    {
        var perfilId = Guid.NewGuid();

        var testTimeSelections = new List<TimeSelection>
        {
            TimeSelection.Create(
                perfilId,
                null,
                DateTime.Now.AddHours(-1),
                DateTime.Now,
                "Guid.NewGuid()",
                EnumTipoTimeSelection.FreeTime,
                TipoAction.Ensinar,
                Variacao.OneToOne
            ),
            TimeSelection.Create(
                perfilId,
                null,
                DateTime.Now.AddHours(-1),
                DateTime.Now,
                "Guid.NewGuid()",
                EnumTipoTimeSelection.FreeTime,
                TipoAction.Ensinar,
                Variacao.CursoOuEvento
            )
        };
        mockTimeSelectionRepo
            .Setup(repo => repo.GetFreeTimeMarcadosAntigos())
            .ReturnsAsync(testTimeSelections);

        var perfilGeradorHash = new Dictionary<Guid, Guid>()
        {
            { testTimeSelections[0].Id, perfilId },
            { testTimeSelections[1].Id, perfilId }
        };
        mockJoinTimeRepo
            .Setup(repo => repo.GetJoinTimePerfilIdsByTimeSelectionIds(It.IsAny<List<Guid>>()))
            .ReturnsAsync(perfilGeradorHash);

        await businessLogic.UpdateOldTimeSelections();

        // Verifica se UpdateRange foi chamado
        mockTimeSelectionRepo.Verify(repo => repo.UpdateRange(testTimeSelections), Times.Once);

        // Verifica se PublishAsync foi chamado para cada TimeSelection
        foreach (var ts in testTimeSelections)
        {
            Assert.True(mockMessagePublisher.Invocations.Count == 1);
        }
    }

    [Fact]
    public async Task NotifyUpcomingTimeSelectionAndJoinTime_UpdatesStatusAndPublishesNotifications()
    {
        var timeSelection = TimeSelection.Create(
            Guid.NewGuid(),
            null,
            DateTime.Now,
            DateTime.Now.AddHours(1),
            "Guid.NewGuid()",
            EnumTipoTimeSelection.FreeTime,
            TipoAction.Ensinar,
            Variacao.OneToOne
        );

        var tsAndJts = new Dictionary<TimeSelection, List<JoinTime>>()
        {
            {
                timeSelection,
                new List<JoinTime>()
                {
                    JoinTime.Create(
                        Guid.NewGuid(),
                        timeSelection.Id,
                        StatusJoinTime.Marcado,
                        false,
                        TipoAction.Aprender
                    )
                }
            }
        };
        mockTimeSelectionRepo
            .Setup(repo => repo.GetUpcomingTimeSelectionAndJoinTime())
            .ReturnsAsync(tsAndJts);

        await businessLogic.NotifyUpcomingTimeSelectionAndJoinTime();

        mockTimeSelectionRepo.Verify(repo => repo.UpdateRange(tsAndJts.Keys.ToList()), Times.Once);
        mockJoinTimeRepo.Verify(repo => repo.UpdateRange(It.IsAny<List<JoinTime>>()), Times.Once);

        foreach (var kvp in tsAndJts)
        {
            var ts = kvp.Key;
            var jts = kvp.Value;
            Assert.True(mockMessagePublisher.Invocations.Count == 2);
        }
    }
}
