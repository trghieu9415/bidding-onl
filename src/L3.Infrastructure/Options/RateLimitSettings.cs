using L2.Application.Ports.Configs;

namespace L3.Infrastructure.Options;

public class RateLimitSettings : IOptionSection {
  public bool IsEnabled { get; set; }
  public Dictionary<string, RateLimitPolicy> Policies { get; set; } = new();
  public static string SectionName => "RateLimit";
}

public class RateLimitPolicy {
  public int TokenLimit { get; set; }
  public int TokensPerPeriod { get; set; }
  public int ReplenishmentPeriodInSeconds { get; set; }
}
