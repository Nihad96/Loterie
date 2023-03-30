using DataLayer.Model;
using Microsoft.EntityFrameworkCore;


namespace DataLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Participation> Participations { get; set; }

        public DbSet<Tirage> Tirages { get; set; }

    }
}