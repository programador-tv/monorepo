using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace tests;

public class NotificationRepositoryTest
{
    private readonly NotificationRepository _repository;
    private readonly ApplicationDbContext _context;

    public NotificationRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options);
        _repository = new NotificationRepository(_context);
    }

    [Fact]
    public async Task GetNotificationsByPerfilId_shouldReturnAllUserNotifications()
    {
        var perfil = Perfil.Create(
            new CreatePerfilRequest(
                Nome: "Teste",
                Token: "12345qwerty",
                UserName: "teste",
                Linkedin: "linkedin.com/test",
                GitHub: "github.com/test",
                Bio: "Teste de bio",
                Email: "teste@test.com",
                Descricao: "Teste de descrição",
                Experiencia: ExperienceLevel.Entre1E3Anos
            )
        );

        if (!_context.Perfils.Any())
        {
            _context.Perfils.AddRange(new List<Perfil> { perfil });
            _context.SaveChangesAsync().Wait();
        }

        var notification1 = Notification.Create(
            perfil.Id,
            perfil.Id,
            TipoNotificacao.NovoInteressadoMentoria,
            "Obrigado por se interessar na mentoria, agora você pode criar e participar de salas de estudo e compartilhar seus conhecimentos ao vivo. Saiba mais sobre o projeto clicando em ver",
            "https://programador.tv/Sobre",
            "_blank"
        );

        var notification2 = Notification.Create(
            perfil.Id,
            perfil.Id,
            TipoNotificacao.FinalizouCadastro,
            "Obrigado por finalizar seu cadastro, agora você pode criar e participar de salas de estudo e compartilhar seus conhecimentos ao vivo. Saiba mais sobre o projeto clicando em ver",
            "https://programador.tv/Sobre",
            "_blank"
        );

        _context.Notifications.AddRange(new List<Notification> { notification1, notification2 });
        await _context.SaveChangesAsync();

        var notifications = await _repository.GetNotificationsByPerfilId(perfil.Id);

        Assert.Multiple(() =>
        {
            Assert.NotEmpty(notifications);
            Assert.True(notifications.Count > 0);
        });
    }

    [Fact]
    public async Task GetNotificationsByPerfilId_shouldReturnOrdenedByDate()
    {
        var perfil = Perfil.Create(
            new CreatePerfilRequest(
                Nome: "Test",
                Token: "12345qwertyX",
                UserName: "test",
                Linkedin: "linkedin.com/test",
                GitHub: "github.com/test",
                Bio: "Teste de bio",
                Email: "test@test.com",
                Descricao: "Teste de descrição",
                Experiencia: ExperienceLevel.Entre1E3Anos
            )
        );

        if (!_context.Perfils.Any())
        {
            _context.Perfils.Add(perfil);
            _context.SaveChangesAsync().Wait();
        }

        var notification1 = Notification.Create(
            perfil.Id,
            perfil.Id,
            TipoNotificacao.NovoInteressadoMentoria,
            "Obrigado por se interessar na mentoria, agora você pode criar e participar de salas de estudo e compartilhar seus conhecimentos ao vivo. Saiba mais sobre o projeto clicando em ver",
            "https://programador.tv/Sobre",
            "_blank"
        );

        var notification2 = Notification.Create(
            perfil.Id,
            perfil.Id,
            TipoNotificacao.FinalizouCadastro,
            "Obrigado por finalizar seu cadastro, agora você pode criar e participar de salas de estudo e compartilhar seus conhecimentos ao vivo. Saiba mais sobre o projeto clicando em ver",
            "https://programador.tv/Sobre",
            "_blank"
        );

        _context.Notifications.Add(notification1);
        await _context.SaveChangesAsync();

        await Task.Delay(10000);

        _context.Notifications.Add(notification2);
        await _context.SaveChangesAsync();

        var notifications = await _repository.GetNotificationsByPerfilId(perfil.Id);

        Assert.Multiple(() =>
        {
            Assert.NotEmpty(notifications);
            Assert.True(notifications.Last().TipoNotificacao == notification1.TipoNotificacao);
        });
    }

    [Fact]
    public async Task UpdateRangeAsync_shouldUpdateNotifications()
    {
        var perfil = Perfil.Create(
            new CreatePerfilRequest(
                Nome: "Teste",
                Token: "12345qwerty",
                UserName: "teste",
                Linkedin: "linkedin.com/test",
                GitHub: "github.com/test",
                Bio: "Teste de bio",
                Email: "teste@test.com",
                Descricao: "Teste de descrição",
                Experiencia: ExperienceLevel.Entre1E3Anos
            )
        );

        if (!_context.Perfils.Any())
        {
            _context.Perfils.Add(perfil);
            _context.SaveChangesAsync().Wait();
        }

        var notification = Notification.Create(
            perfil.Id,
            perfil.Id,
            TipoNotificacao.FinalizouCadastro,
            "Obrigado por finalizar seu cadastro, agora você pode criar e participar de salas de estudo e compartilhar seus conhecimentos ao vivo. Saiba mais sobre o projeto clicando em ver",
            "https://programador.tv/Sobre",
            "_blank"
        );

        if (!_context.Notifications.Any())
        {
            _context.Notifications.Add(notification);
            _context.SaveChangesAsync().Wait();
        }

        var notificationsToUpdate = new List<Notification> { notification };
        await _repository.UpdateRangeAsync(notificationsToUpdate);

        var updatedNotification = await _context.Notifications.FirstOrDefaultAsync(n =>
            n.Id == notification.Id
        );

        Assert.NotNull(updatedNotification);
        Assert.Equal(notification.Conteudo, updatedNotification.Conteudo);
        Assert.Equal(notification.ActionLink, updatedNotification.ActionLink);
        Assert.Equal(notification.SecundaryLink, updatedNotification.SecundaryLink);
    }
}
