using Microsoft.EntityFrameworkCore;
using Todo.User.Domain.Entity;

namespace Todo.User.Infrastructure.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Domain.Entity.User> Users { get; set; } = null!;
    public virtual DbSet<LoginHistory> LoginHistories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entity.User>(builder =>
        {
            builder.HasIndex(x => x.Username)
                .IsUnique()
                .HasDatabaseName("idx_username");

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("idx_email");

            builder.HasIndex(x => x.Phone)
                .IsUnique()
                .HasDatabaseName("idx_phone");
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Database=TodoUser;User Id=sa;Password=yourStrong(!)Password;");
        }
    }
}