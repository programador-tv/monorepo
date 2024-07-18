using Domain.Entities;

public class _PerfilAvatar
{
    public Perfil Perfil { get; set; } = new Perfil();
    public bool WithLink { get; set; }

    public string Size { get; set; } = "avatar-sm";
}
