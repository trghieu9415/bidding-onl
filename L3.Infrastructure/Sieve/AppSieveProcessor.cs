using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace L3.Infrastructure.Sieve;

public class AppSieveProcessor(
  IOptions<SieveOptions> options
) : SieveProcessor(options) {
  protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper) {
    return mapper.ApplyConfigurationsFromAssembly(typeof(AppSieveProcessor).Assembly);
  }
}
