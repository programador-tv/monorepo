using System.Globalization;
using Domain.Contracts;
using Domain.Enumerables;
using Domain.Primitives;

namespace Domain.Entities;

public sealed class Perfil(
    Guid id,
    string nome,
    string? foto,
    string token,
    string userName,
    string? linkedin,
    string? gitHub,
    string? bio,
    string email,
    string? descricao,
    ExperienceLevel experiencia
) : Entity(id)
{
    public string Nome { get; private set; } = nome;
    public string? Foto { get; private set; } = foto;
    public string Token { get; private set; } = token;
    public string UserName { get; private set; } = userName;
    public string? Linkedin { get; private set; } = linkedin;
    public string? GitHub { get; private set; } = gitHub;
    public string? Bio { get; private set; } = bio;
    public string Email { get; private set; } = email;
    public string? Descricao { get; private set; } = descricao;
    public ExperienceLevel Experiencia { get; private set; } = experiencia;

    public static Perfil Create(CreatePerfilRequest createPerfilRequest)
    {
        return new Perfil(
            id: Guid.NewGuid(),
            foto: string.Empty,
            nome: createPerfilRequest.Nome,
            token: createPerfilRequest.Token,
            userName: createPerfilRequest.UserName,
            linkedin: createPerfilRequest.Linkedin,
            gitHub: createPerfilRequest.GitHub,
            bio: createPerfilRequest.Bio,
            email: createPerfilRequest.Email,
            descricao: createPerfilRequest.Descricao,
            experiencia: createPerfilRequest.Experiencia
        );
    }

    public static Perfil Create(CreateOrUpdatePerfilRequest createOrUpdatePerfilRequest)
    {
        return new Perfil(
            id: Guid.NewGuid(),
            foto: string.Empty,
            nome: createOrUpdatePerfilRequest.Nome,
            token: createOrUpdatePerfilRequest.Token,
            userName: createOrUpdatePerfilRequest.UserName,
            linkedin: createOrUpdatePerfilRequest.Linkedin,
            gitHub: createOrUpdatePerfilRequest.GitHub,
            bio: createOrUpdatePerfilRequest.Bio,
            email: createOrUpdatePerfilRequest.Email,
            descricao: createOrUpdatePerfilRequest.Descricao,
            experiencia: createOrUpdatePerfilRequest.Experiencia
        );
    }

    public void Update(UpdatePerfilRequest updatePerfilRequest)
    {
        Nome = updatePerfilRequest.Nome;
        UserName = updatePerfilRequest.UserName;
        Linkedin = updatePerfilRequest.Linkedin;
        GitHub = updatePerfilRequest.GitHub;
        Bio = updatePerfilRequest.Bio;
        Descricao = updatePerfilRequest.Descricao;
        Experiencia = updatePerfilRequest.Experiencia;
    }

    public void Update(CreateOrUpdatePerfilRequest createOrUpdatePerfilRequest)
    {
        Nome = createOrUpdatePerfilRequest.Nome;
        UserName = createOrUpdatePerfilRequest.UserName;
        Linkedin = createOrUpdatePerfilRequest.Linkedin;
        GitHub = createOrUpdatePerfilRequest.GitHub;
        Bio = createOrUpdatePerfilRequest.Bio;
        Descricao = createOrUpdatePerfilRequest.Descricao;
        Experiencia = createOrUpdatePerfilRequest.Experiencia;
    }

    public void UpdateFoto(string foto)
    {
        Foto = foto;
    }

    public Notification BuildNotificationProfileCreatedSuccessfuly()
    {
        var conteudo =
            $@"
                    Obrigado por finalizar seu cadastro,
                    agora você pode criar e participar de salas de estudo e
                    compartilhar seus conhecimentos ao vivo. Saiba mais sobre o projeto clicando em ver
                    ";
        var notification = Notification.Create(
            Id,
            Id,
            TipoNotificacao.FinalizouCadastro,
            conteudo,
            "/Sobre",
            string.Empty
        );

        return notification;
    }
}
