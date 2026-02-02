using L1.Core.Domain.Catalog.Entities;
using Sieve.Services;

namespace L3.Infrastructure.Sieve.Configurations;

public class CatalogItemSieveConfiguration : BaseSieveConfiguration<CatalogItem> {
  public override void Configure(SievePropertyMapper mapper) {
    base.Configure(mapper);

    mapper.Property<CatalogItem>(x => x.Name)
      .CanFilter()
      .CanSort();

    mapper.Property<CatalogItem>(x => x.StartingPrice)
      .CanFilter()
      .CanSort();

    mapper.Property<CatalogItem>(x => x.Status)
      .CanFilter()
      .CanSort();

    mapper.Property<CatalogItem>(x => x.OwnerId)
      .CanFilter();

    mapper.Property<CatalogItem>(x => x.Condition)
      .CanFilter();
  }
}
