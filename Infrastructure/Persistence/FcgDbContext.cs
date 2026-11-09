using Microsoft.EntityFrameworkCore;
using Fcg.Domain.Games;
using Fcg.Domain.Interfaces;
using Fcg.Domain.Library;
using Fcg.Domain.Promotions;

namespace Fcg.Infrastructure.Persistence;

public class FcgDbContext : DbContext, IUnitOfWork
{
    public FcgDbContext(DbContextOptions<FcgDbContext> options) : base(options)
    {
    }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<LibraryItem> LibraryItems => Set<LibraryItem>();
    public DbSet<Promotion> Promotions => Set<Promotion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FcgDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
