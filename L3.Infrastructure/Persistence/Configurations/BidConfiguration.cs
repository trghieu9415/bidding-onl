using L1.Core.Domain.Bidding.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L3.Infrastructure.Persistence.Configurations;

public class BidConfiguration : BaseConfiguration<Bid> {
  public override void Configure(EntityTypeBuilder<Bid> builder) {
    base.Configure(builder);

    builder.Property(x => x.Amount).HasPrecision(18, 2);
    builder.Property(x => x.TimePoint).IsRequired();

    builder.HasOne(x => x.Auction)
      .WithMany(a => a.Bids)
      .HasForeignKey(x => x.AuctionId);
  }
}