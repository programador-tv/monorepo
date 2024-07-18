using Domain.Entities;
using Domain.Models.ViewModels;

namespace Platform.Services;

public interface IPerfilService
{
    public Perfil? GetPerfilByUserName(string usr);
}
