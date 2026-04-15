using L3.Infrastructure.Persistence.Identity;
using Sieve.Services;

namespace L3.Infrastructure.Sieve.Configurations;

public class AppUserSieveConfiguration : ISieveConfiguration {
  public void Configure(SievePropertyMapper mapper) {
    mapper.Property<AppUser>(x => x.FullName)
      .CanFilter()
      .CanSort();

    mapper.Property<AppUser>(x => x.Email)
      .CanFilter()
      .CanSort();

    mapper.Property<AppUser>(x => x.Role)
      .CanFilter();

    mapper.Property<AppUser>(x => x.CreatedAt)
      .CanFilter()
      .CanSort();

    mapper.Property<AppUser>(x => x.IsDeleted)
      .CanFilter();
  }
}
