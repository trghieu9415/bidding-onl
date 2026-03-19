using L1.Core.Domain.Transaction.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L3.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : BaseConfiguration<Order> {
  public override void Configure(EntityTypeBuilder<Order> builder) {
    base.Configure(builder);
    builder.Property(o => o.BidderName)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(o => o.CatalogName)
      .IsRequired()
      .HasMaxLength(500);

    builder.Property(o => o.CatalogImage)
      .IsRequired()
      .HasMaxLength(2048);

    builder.Property(o => o.Price)
      .HasPrecision(18, 2)
      .IsRequired();

    builder.Property(o => o.Status)
      .HasConversion<string>()
      .HasMaxLength(50)
      .IsRequired();


    builder.HasIndex(o => o.AuctionId);
    builder.HasIndex(o => o.BidderId);
  }
}
