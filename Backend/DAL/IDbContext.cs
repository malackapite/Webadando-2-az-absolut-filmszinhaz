using Backend.Models.Felhasznalo;
using Backend.Models.Macska;
using Backend.Models.Rendeles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backend.DAL
{
    public interface IDbContext
    {
        DbSet<Felhasznalo> Felhasznalok { get; }
        DbSet<Rendeles> Rendelesek { get; }
        DbSet<Macska> Macskak { get; }
        DbSet<RendeleshezTartozik> RendeleshezTartozikok { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}
