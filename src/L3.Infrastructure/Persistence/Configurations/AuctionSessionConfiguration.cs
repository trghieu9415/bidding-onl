using L1.Core.Domain.Bidding.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L3.Infrastructure.Persistence.Configurations;

public class AuctionSessionConfiguration : BaseConfiguration<AuctionSession> {
  public override void Configure(EntityTypeBuilder<AuctionSession> builder) {
    base.Configure(builder);

    builder.Property(x => x.Title).IsRequired().HasMaxLength(255);
    builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

    //
    builder.OwnsOne(x => x.TimeFrame, nav => {
      nav.Property(t => t.StartTime).HasColumnName("StartTime").IsRequired();
      nav.Property(t => t.EndTime).HasColumnName("EndTime").IsRequired();
    });

    builder.Property(x => x.AuctionIds).HasColumnType("jsonb");

    builder.HasIndex(x => x.AuctionIds).HasMethod("gin");
  }
}
