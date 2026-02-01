using FlightDocSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightDocSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Flights> Flights { get; set; }
        public DbSet<DocumentCategory> DocumentCategories { get; set; }
        public DbSet<FlightDocuments> FlightDocuments { get; set; }
        public DbSet<DocumentFile> DocumentFiles { get; set; }
        public DbSet<FlightAssigment> FlightAssigments { get; set; }
        public DbSet<RevokedToken> RevokedTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Code)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<FlightAssigment>()
                .HasKey(fa => new { fa.UserId, fa.FlightId });

            modelBuilder.Entity<FlightAssigment>()
                .HasOne(fa => fa.User)
                .WithMany(u => u.FlightAssigments)
                .HasForeignKey(fa => fa.UserId);

            modelBuilder.Entity<FlightAssigment>()
                .HasOne(fa => fa.Flight)
                .WithMany(f => f.FlightAssigments)
                .HasForeignKey(fa => fa.FlightId);
        }
    }
}
