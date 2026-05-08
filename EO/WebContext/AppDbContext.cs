using EO.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EO.WebContext
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }


        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<CompanyDetails> CompanyDetails { get; set; }
        public DbSet<UserSocialLinks> UserSocialLinks { get; set; }
        public DbSet<Spouse> Spouses { get; set; }
        public DbSet<SpouseSocialLinks> SpouseSocialLinks { get; set; }
        public DbSet<SpouseProfessionalDetails> SpouseProfessionalDetails { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Events> Events { get; set; }

        public DbSet<EventDetail> EventDetails { get; set; }
        public DbSet<EventSchedule> EventSchedules { get; set; }
        public DbSet<EventGuest> EventGuests { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Alliance> Alliances { get; set; }
        public DbSet<AlliancePerk> AlliancePerks { get; set; }
        public DbSet<AllianceType> AllianceTypes { get; set; }
        public DbSet<Perk> Perks { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupTag> GroupTag { get; set; }
        public DbSet<GroupCategory> GroupCategory { get; set; }
        public DbSet<GroupTagMap> GroupTagMaps { get; set; }
        public DbSet<GroupCategoryMap> GroupCategoryMaps { get; set; }
        public DbSet<GroupChampionsMap> GroupChampionsMaps { get; set; }
        public DbSet<GroupCoordinatorsMap> GroupCoordinatorsMaps { get; set; }
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

            modelBuilder.Entity<AlliancePerk>()
                .HasKey(x => new { x.AllianceId, x.PerkId });

            modelBuilder.Entity<AlliancePerk>()
                .HasOne(x => x.Alliance)
                .WithMany(x => x.AlliancePerks)
                .HasForeignKey(x => x.AllianceId);

            modelBuilder.Entity<AlliancePerk>()
                .HasOne(x => x.Perk)
                .WithMany(x => x.AlliancePerks)
                .HasForeignKey(x => x.PerkId);

            modelBuilder.Entity<CompanyDetails>()
                .HasOne(c => c.User)
                .WithOne(u => u.CompanyDetails)
                .HasForeignKey<CompanyDetails>(c => c.UserId);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Group>().ToTable("Groups").HasKey(x => x.Id);

            modelBuilder.Entity<GroupTag>()
                .ToTable("GroupTag")
                .HasKey(x => x.Id);

            modelBuilder.Entity<GroupCategory>()
                .ToTable("GroupCategory")
                .HasKey(x => x.Id);

            modelBuilder.Entity<GroupTagMap>(entity =>
            {
                entity.ToTable("GroupTagMap");
                entity.HasKey(x => new { x.GroupId, x.TagId });
            });

            modelBuilder.Entity<GroupCategoryMap>(entity =>
            {
                entity.ToTable("GroupCategoryMap");
                entity.HasKey(x => new { x.GroupId, x.CategoryId });
            });

            modelBuilder.Entity<GroupChampionsMap>(entity =>
            {
                entity.ToTable("GroupChampionsMap");
                entity.HasKey(x => new { x.GroupId, x.UserId });
            });

            modelBuilder.Entity<GroupCoordinatorsMap>(entity =>
            {
                entity.ToTable("GroupCoordinatorsMap");
                entity.HasKey(x => new { x.GroupId, x.UserId });
            });
        }
    }
}