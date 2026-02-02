using L1.Core.Base.Entity;
using Sieve.Services;

namespace L3.Infrastructure.Sieve;

public abstract class BaseSieveConfiguration<T> : ISieveConfiguration where T : BaseEntity {
  public virtual void Configure(SievePropertyMapper mapper) {
    mapper.Property<T>(x => x.Id)
      .CanFilter()
      .CanSort();

    mapper.Property<T>(x => x.CreatedAt)
      .CanFilter()
      .CanSort();
  }
}
