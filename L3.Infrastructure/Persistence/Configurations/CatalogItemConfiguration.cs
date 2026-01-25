using L1.Core.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L3.Infrastructure.Persistence.Configurations;

public class CatalogItemConfiguration : BaseConfiguration<CatalogItem> {
  public override void Configure(EntityTypeBuilder<CatalogItem> builder) {
    base.Configure(builder);

    builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
    builder.Property(x => x.Description).IsRequired();
    builder.Property(x => x.StartingPrice).HasPrecision(18, 2);

    builder.OwnsOne(x => x.Images, nav => {
      nav.Property(i => i.MainImageUrl).HasMaxLength(500);
      nav.Property(i => i.SubImageUrls)
        .HasColumnType("jsonb");
    });

    builder.Property(x => x.CategoryIds)
      .HasColumnType("jsonb");

    builder.HasIndex(x => x.CategoryIds)
      .HasMethod("gin");
  }
}
