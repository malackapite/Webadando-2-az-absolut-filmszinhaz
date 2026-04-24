using Backend.Models.Macska;
using Microsoft.EntityFrameworkCore;

namespace Backend.DAL
{
    public class AppDbContext(DbContextOptions<AppDbContext> contextOptions, IConfiguration config) : DbContext(contextOptions), IDbContext
    {
        readonly IConfiguration config = config;

        public DbSet<Macska> Macskak { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("DbConnection"));
        }
    }
}
