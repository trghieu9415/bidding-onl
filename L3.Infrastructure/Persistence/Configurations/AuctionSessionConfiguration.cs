using L1.Core.Domain.Bidding.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L3.Infrastructure.Persistence.Configurations;

public class AuctionSessionConfiguration : BaseConfiguration<AuctionSession> {
  public override void Configure(EntityTypeBuilder<AuctionSession> builder) {
    base.Configure(builder);

    builder.Property(x => x.Title).IsRequired().HasMaxLength(255);

    builder.OwnsOne(x => x.TimeFrame, nav => {
      nav.Property(t => t.StartTime).HasColumnName("StartTime");
      nav.Property(t => t.EndTime).HasColumnName("EndTime");
    });

    builder.Property(x => x.AuctionIds);
  }
}