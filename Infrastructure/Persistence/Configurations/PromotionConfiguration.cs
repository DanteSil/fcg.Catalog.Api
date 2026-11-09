using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fcg.Domain.Promotions;

namespace Fcg.Infrastructure.Persistence.Configurations;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.ToTable("promotions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.GameId).IsRequired();
        builder.Property(x => x.DiscountPercent).HasPrecision(5, 2).IsRequired();
        builder.Property(x => x.StartsAt).IsRequired();
        builder.Property(x => x.EndsAt).IsRequired();
        builder.Ignore(x => x.DomainEvents);
    }
}
