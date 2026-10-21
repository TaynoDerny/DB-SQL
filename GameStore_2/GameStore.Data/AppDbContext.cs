using Microsoft.EntityFrameworkCore;
using GameStore.Models;

namespace GameStore.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=GameStoreDb;Trusted_Connection=True;");
            }
        }
    }
}