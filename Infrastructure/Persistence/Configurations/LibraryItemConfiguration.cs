using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fcg.Domain.Library;

namespace Fcg.Infrastructure.Persistence.Configurations;

public class LibraryItemConfiguration : IEntityTypeConfiguration<LibraryItem>
{
    public void Configure(EntityTypeBuilder<LibraryItem> builder)
    {
        builder.ToTable("library_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.GameId).IsRequired();
        builder.Property(x => x.AcquiredAt).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.GameId }).IsUnique();
        builder.Ignore(x => x.DomainEvents);
    }
}
