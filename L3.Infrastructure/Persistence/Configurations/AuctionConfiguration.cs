using L1.Core.Domain.Bidding.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L3.Infrastructure.Persistence.Configurations;

public class AuctionConfiguration : BaseConfiguration<Auction> {
  public override void Configure(EntityTypeBuilder<Auction> builder) {
    base.Configure(builder);

    builder.Property(x => x.CurrentPrice).HasPrecision(18, 2);

    builder.OwnsOne(x => x.Rules, nav => {
      nav.Property(r => r.StepPrice).HasPrecision(18, 2).HasColumnName("StepPrice");
      nav.Property(r => r.ReservePrice).HasPrecision(18, 2).HasColumnName("ReservePrice");
    });

    builder.HasMany(x => x.Bids)
      .WithOne(b => b.Auction)
      .HasForeignKey(b => b.AuctionId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.Navigation(x => x.Bids).AutoInclude(false);
  }
}