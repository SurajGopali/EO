using EO.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EO.WebContext
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Events> Events { get; set; }

        public DbSet<EventDetail> EventDetails { get; set; }
        public DbSet<EventSchedule> EventSchedules { get; set; }
        public DbSet<EventGuest> EventGuests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Events>()
                .HasOne(e => e.EventDetail)
                .WithOne(d => d.Event)
                .HasForeignKey<EventDetail>(d => d.EventId);

            modelBuilder.Entity<EventSchedule>()
                .HasOne(s => s.Event)
                .WithMany(e => e.Schedules)
                .HasForeignKey(s => s.EventId);

            modelBuilder.Entity<EventGuest>()
                .HasOne(g => g.Event)
                .WithMany(e => e.Guests)
                .HasForeignKey(g => g.EventId);
        }
    }
}