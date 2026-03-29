using EO.Models;
using Microsoft.EntityFrameworkCore;

namespace EO.WebContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Events> Events { get; set; }

    }
}
