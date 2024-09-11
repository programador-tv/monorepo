using Domain.Entities;

public class _PerfilAvatar
{
    public string Nome { get; set; } = string.Empty;
    public string Foto { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool WithLink { get; set; }

    public string Size { get; set; } = "avatar-sm";
}
