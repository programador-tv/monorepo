using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Enums;

namespace tests;

public class JoinTimeEntityTest
{
    [Fact]
    public void CreateJoinTime_ShouldInitializeProperties()
    {
        // Arrange
        Guid perfilId = Guid.NewGuid();
        Guid timeSelectionId = Guid.NewGuid();

        // Act
        var joinTime = JoinTime.Create(
            perfilId,
            timeSelectionId,
            StatusJoinTime.Marcado,
            false,
            TipoAction.Aprender
        );

        // Assert
        Assert.Equal(perfilId, joinTime.PerfilId);
        Assert.Equal(timeSelectionId, joinTime.TimeSelectionId);
        Assert.Equal(StatusJoinTime.Marcado, joinTime.StatusJoinTime);
        Assert.False(joinTime.NotifiedMentoriaProxima);
        Assert.Equal(TipoAction.Aprender, joinTime.TipoAction);
    }

    [Fact]
    public void ChangeStatus_ShouldUpdateStatus()
    {
        // Arrange
        var joinTime = JoinTime.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            StatusJoinTime.Marcado,
            false,
            TipoAction.Aprender
        );

        // Act
        joinTime.ChangeStatus(StatusJoinTime.Concluído);

        // Assert
        Assert.Equal(StatusJoinTime.Concluído, joinTime.StatusJoinTime);
    }

    [Fact]
    public void MarkAsNotified_ShouldSetNotifiedMentoriaProximaToTrue()
    {
        // Arrange
        var joinTime = JoinTime.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            StatusJoinTime.Marcado,
            false,
            TipoAction.Aprender
        );

        // Act
        joinTime.MarkAsNotified();

        // Assert
        Assert.True(joinTime.NotifiedMentoriaProxima);
    }

    [Fact]
    public void BuildUpcomingNotification_ShouldCreateUpcomingNotification()
    {
        // Arrange
        var joinTime = JoinTime.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            StatusJoinTime.Marcado,
            false,
            TipoAction.Aprender
        );
        Guid timeSelectionPerfilId = Guid.NewGuid();

        // Act
        var notification = joinTime.BuildUpcomingNotification(timeSelectionPerfilId);

        // Assert
        Assert.Equal(joinTime.PerfilId, notification.DestinoPerfilId);
        Assert.Equal(timeSelectionPerfilId, notification.GeradorPerfilId);
        Assert.Equal(TipoNotificacao.AlunoMentoriaProxima, notification.TipoNotificacao);
        // Add more assertions as needed
    }

    [Fact]
    public void BuildPendingNotification_ShouldCreatePendingNotification()
    {
        // Arrange
        var joinTime = JoinTime.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            StatusJoinTime.Marcado,
            false,
            TipoAction.Aprender
        );
        Guid timeSelectionPerfilId = Guid.NewGuid();
        string title = "Test Title";

        // Act
        var notification = joinTime.BuildPendingNotification(timeSelectionPerfilId, title);

        // Assert
        Assert.Equal(joinTime.PerfilId, notification.DestinoPerfilId);
        Assert.Equal(timeSelectionPerfilId, notification.GeradorPerfilId);
        Assert.Equal(TipoNotificacao.AlunoConsideracaoFinalPendente, notification.TipoNotificacao);
        // Add more assertions as needed
    }
}
