using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Model;

public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<PasswordHistory> PasswordHistory { get; set; }

    private readonly IConfiguration _configuration;

    public AuthDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = _configuration.GetConnectionString("AuthConnectionString");
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the relationship between ApplicationUser and PasswordHistory
        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.PasswordHistories)
            .WithOne(ph => ph.User)
            .HasForeignKey(ph => ph.UserId);
    }
}
