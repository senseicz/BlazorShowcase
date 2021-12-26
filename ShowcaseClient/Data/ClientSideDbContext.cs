using BlazorShowcase.Data;
using Microsoft.EntityFrameworkCore;

namespace ShowcaseClient.Data;

internal class ClientSideDbContext : DbContext
{
    public DbSet<Score> Scores { get; set; } = default!;

    public ClientSideDbContext(DbContextOptions<ClientSideDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Score>().HasIndex(nameof(Score.CreatedOn), nameof(Score.Id));
        modelBuilder.Entity<Score>().HasIndex(nameof(Score.UserName), nameof(Score.FullName));
        modelBuilder.Entity<Score>().HasIndex(x => x.RiskScore);
        modelBuilder.Entity<Score>().HasIndex(x => x.City);
        modelBuilder.Entity<Score>().HasIndex(x => x.IpAddress);
        modelBuilder.Entity<Score>().Property(x => x.UserName).UseCollation("nocase");
        modelBuilder.Entity<Score>().Property(x => x.FullName).UseCollation("nocase");
    }
}
