using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GuestBook.Models;

namespace GuestBook.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<GuestBookMessage> GuestBookMessages { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure GuestBookMessage relationships
        modelBuilder.Entity<GuestBookMessage>()
            .HasOne(m => m.User)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<GuestBookMessage>()
            .HasOne(m => m.RepliedByUser)
            .WithMany()
            .HasForeignKey(m => m.RepliedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<GuestBookMessage>()
            .HasOne(m => m.Category)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure UserProfile relationship
        modelBuilder.Entity<UserProfile>()
            .HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed default categories with static dates
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "General", Description = "General guestbook messages", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 2, Name = "Feedback", Description = "Feedback and suggestions", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 3, Name = "Questions", Description = "Questions and inquiries", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
