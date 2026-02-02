using L1.Core.Domain.Bidding.Entities;
using Sieve.Services;

namespace L3.Infrastructure.Sieve.Configurations;

public class AuctionSessionSieveConfiguration : BaseSieveConfiguration<AuctionSession> {
  public override void Configure(SievePropertyMapper mapper) {
    base.Configure(mapper);

    mapper.Property<AuctionSession>(x => x.Title)
      .CanFilter()
      .CanSort();

    mapper.Property<AuctionSession>(x => x.Status)
      .CanFilter()
      .CanSort();

    mapper.Property<AuctionSession>(x => x.TimeFrame!.StartTime)
      .HasName("StartTime")
      .CanFilter()
      .CanSort();

    mapper.Property<AuctionSession>(x => x.TimeFrame!.EndTime)
      .HasName("EndTime")
      .CanFilter()
      .CanSort();
  }
}
