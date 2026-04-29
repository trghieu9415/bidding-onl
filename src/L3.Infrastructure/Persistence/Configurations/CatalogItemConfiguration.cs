using L1.Core.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L3.Infrastructure.Persistence.Configurations;

public class CatalogItemConfiguration : BaseConfiguration<CatalogItem> {
  public override void Configure(EntityTypeBuilder<CatalogItem> builder) {
    base.Configure(builder);

    builder.Property(x => x.OwnerId).IsRequired();
    builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
    builder.Property(x => x.Description).IsRequired();
    builder.Property(x => x.StartingPrice).HasPrecision(18, 2);

    builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
    builder.Property(x => x.Condition).HasConversion<string>().HasMaxLength(50).IsRequired(false);

    builder.Property(x => x.CategoryIds).HasColumnType("jsonb");

    builder.OwnsOne(x => x.Images, nav => {
      nav.Property(i => i.MainImageUrl).HasMaxLength(500).IsRequired(false);
      nav.Property(i => i.SubImageUrls).HasColumnType("jsonb").IsRequired(false);
    });

    builder.HasIndex(x => x.CategoryIds).HasMethod("gin");
  }
}
