using L1.Core.Domain.Bidding.Entities;
using Sieve.Services;

namespace L3.Infrastructure.Sieve.Configurations;

public class AuctionSieveConfiguration : BaseSieveConfiguration<Auction> {
  public override void Configure(SievePropertyMapper mapper) {
    base.Configure(mapper);

    mapper.Property<Auction>(x => x.Status)
      .CanFilter()
      .CanSort();

    mapper.Property<Auction>(x => x.CurrentPrice)
      .CanFilter()
      .CanSort();

    mapper.Property<Auction>(x => x.CatalogItemId)
      .CanFilter();
  }
}
