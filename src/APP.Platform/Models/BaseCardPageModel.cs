namespace Models;

public abstract class BaseCardPageModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Titulo { get; set; } = string.Empty;
}
