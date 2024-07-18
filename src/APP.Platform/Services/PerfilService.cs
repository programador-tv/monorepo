using Domain.Entities;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace Platform.Services;

public class PerfilService : IPerfilService
{
    private readonly PerfilDbContext _perfilContext;

    public PerfilService(PerfilDbContext perfilDbContext) { }

    public Perfil? GetPerfilByUserName(string usr)
    {
        Perfil? perfilOwner = _perfilContext?.Perfils?.FirstOrDefault(x => usr == x.UserName);

        return perfilOwner;
    }
}
