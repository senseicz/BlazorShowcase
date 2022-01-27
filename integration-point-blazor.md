# Blazor with SQLite and Entity Framework Core

## Native dependency reference + reference EF Core

* .csproj file

        <ItemGroup>
            <NativeFileReference Include="Data\e_sqlite3.o" />
            <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="6.0.1" />
        </ItemGroup>

## Client-side DB Context

* Data\ClientSideDbContext.cs

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

* Program.cs

        // Sets up EF Core with Sqlite
        builder.Services.AddShowcaseDataDbContext();

* Data\ShowcaseClientDataServices.cs

        public static void AddShowcaseDataDbContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContextFactory<ClientSideDbContext>(
                options => options.UseSqlite($"Filename={DataSynchronizer.SqliteDbFilename}"));
            serviceCollection.AddScoped<DataSynchronizer>();
        }        

## Grid component

* Pages\Dashboard\Scores.razor 

        <Grid Virtualize="true" Items="@GetFilteredScores()" ItemKey="@(x => x.Id)" ItemSize="35">

* Pages\Dashboard\Scores.razor.cs

        IQueryable<Score>? GetFilteredScores()
        {
            if (db is null)
            {
                return null;
            }

            var result = db.Scores.AsNoTracking().AsQueryable();
            
            if (!string.IsNullOrEmpty(searchUserName))
            {
                result = result.Where(x => EF.Functions.Like(x.UserName, searchUserName.Replace("%", "\\%") + "%", "\\"));
            }

            if (!string.IsNullOrEmpty(searchFullName))
            {
                result = result.Where(x => EF.Functions.Like(x.FullName, searchFullName.Replace("%", "\\%") + "%", "\\"));
            }

            if (minScore > 0)
            {
                result = result.Where(x => x.RiskScore >= minScore);
            }
            if (maxScore < 1000)
            {
                result = result.Where(x => x.RiskScore <= maxScore);
            }

            return result;
        }
