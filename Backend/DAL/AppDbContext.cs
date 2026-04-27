using Backend.Models.Felhasznalo;
using Backend.Models.Macska;
using Backend.Models.Rendeles;
using Microsoft.EntityFrameworkCore;

namespace Backend.DAL
{
    public class AppDbContext(IConfiguration config) : DbContext()
    {
        readonly IConfiguration config = config;
        
        public DbSet<Felhasznalo> Felhasznalok { get; set; }
        public DbSet<Rendeles> Rendelesek { get; set; }
        public DbSet<Macska> Macskak { get; set; }
        public DbSet<RendeleshezTartozik> RendeleshezTartozikok { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("DbConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Felhasznalo>()
                .HasMany(static (Felhasznalo felhasznalo) => felhasznalo._Rendelesek)
                .WithOne(static (Rendeles rendeles) => rendeles._Felhasznalo)
                .HasForeignKey(static (Rendeles rendeles) => rendeles.Felhasznalo)
                .OnDelete(DeleteBehavior.Cascade)
            ;
            modelBuilder.Entity<Rendeles>()
                .HasMany(static (Rendeles rendeles) => rendeles._RendeleshezTartozikok)
                .WithOne(static (RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik._Rendeles)
                .HasForeignKey(static (RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik.Rendeles)
                .OnDelete(DeleteBehavior.Cascade)
            ;
            modelBuilder.Entity<Macska>()
                .HasMany(static (Macska macska) => macska._RendeleshezTartozikok)
                .WithOne(static (RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik._Macska)
                .HasForeignKey(static (RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik.Macska)
                .OnDelete(DeleteBehavior.Cascade)
            ;
        }
    }
}
