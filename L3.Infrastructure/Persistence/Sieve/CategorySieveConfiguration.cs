using L1.Core.Domain.Catalog.Entities;
using Sieve.Services;

namespace L3.Infrastructure.Persistence.Sieve;

public class CategorySieveConfiguration : BaseSieveConfiguration<Category> {
  public override void Configure(SievePropertyMapper mapper) {
    base.Configure(mapper);

    mapper.Property<Category>(x => x.Name)
      .CanFilter()
      .CanSort();

    mapper.Property<Category>(x => x.ParentId)
      .CanFilter();
  }
}