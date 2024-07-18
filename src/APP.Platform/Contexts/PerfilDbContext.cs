using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Contexts
{
    public class PerfilDbContext : DbContext
    {
        public DbSet<Perfil> Perfils { get; set; }

        public PerfilDbContext(DbContextOptions<PerfilDbContext> options)
            : base(options) { }
    }
}
