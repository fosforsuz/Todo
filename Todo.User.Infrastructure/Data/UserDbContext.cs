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
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

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

        modelBuilder.Entity<LoginHistory>(builder =>
        {
            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("idx_user_id");

            builder.HasIndex(x => x.LoginAt)
                .HasDatabaseName("idx_login_time");

            builder.HasOne(x => x.User)
                .WithMany(x => x.LoginHistories)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_user_login_history");
        });

        modelBuilder.Entity<RefreshToken>(builder =>
        {
            builder.HasIndex(x => x.Token)
                .IsUnique()
                .HasDatabaseName("idx_token");

            builder.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_user_refresh_token");
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql("Server=localhost;Database=TodoUser;User Id=sa;Password=yourStrong(!)Password;");
    }
}