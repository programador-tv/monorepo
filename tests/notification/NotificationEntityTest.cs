using Domain.Entities;
using Domain.Enumerables;

namespace tests;

public class NotificationEntityTest
{
    [Fact]
    public void Create_GenereteTypeOfNotification()
    {
        var request = new
        {
            DestinoPerfilId = Guid.NewGuid(),
            GeradorPerfilId = Guid.NewGuid(),
            TipoNotificacao = TipoNotificacao.FinalizouCadastro,
            Vizualizado = false,
            DataCriacao = DateTime.Now,
            Conteudo = "Obrigado por finalizar seu cadastro, agora você pode criar e participar de salas de estudo e compartilhar seus conhecimentos ao vivo. Saiba mais sobre o projeto clicando em ver",
            ActionLink = "https://programador.tv/Sobre",
            SecundaryLink = "_blank",
        };

        var notification = Notification.Create(
            request.DestinoPerfilId,
            request.GeradorPerfilId,
            request.TipoNotificacao,
            request.Conteudo,
            request.ActionLink,
            request.SecundaryLink
        );

        Assert.Multiple(() =>
        {
            Assert.NotNull(notification);
            Assert.Equal(request.DestinoPerfilId, notification.DestinoPerfilId);
            Assert.Equal(request.GeradorPerfilId, notification.GeradorPerfilId);
            Assert.Equal(request.TipoNotificacao, notification.TipoNotificacao);
            Assert.Equal(request.Vizualizado, notification.Vizualizado);
            Assert.Equal(
                request.DataCriacao.ToString("dd/MM/yy HH:mm"),
                notification.CreatedAt.ToString("dd/MM/yy HH:mm")
            );
            Assert.Equal(request.Conteudo, notification.Conteudo);
            Assert.Equal(request.ActionLink, notification.ActionLink);
            Assert.Equal(request.SecundaryLink, notification.SecundaryLink);
        });
    }

    [Fact]
    public void Visualizar_ShouldUpdateVisualizadoProperty()
    {
        var notification = Notification.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TipoNotificacao.FinalizouCadastro,
            "Obrigado por finalizar seu cadastro, agora você pode criar e participar de salas de estudo e compartilhar seus conhecimentos ao vivo. Saiba mais sobre o projeto clicando em ver",
            "https://programador.tv/Sobre",
            "_blank"
        );

        notification.Visualizar();

        Assert.True(notification.Vizualizado);
    }
}
