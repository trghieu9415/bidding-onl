using System.ComponentModel.DataAnnotations;
using L2.Application.Ports.Configs;

namespace L3.Infrastructure.Options;

public class RedisSettings : IOptionSection {
  [Required(ErrorMessage = "Cache Connection String là bắt buộc!")]
  public string CacheConnection { get; set; } = "localhost:6379";

  [Required(ErrorMessage = "Critical Connection String là bắt buộc!")]
  public string CriticalConnection { get; set; } = "localhost:6380";

  [Required(ErrorMessage = "Backplane Connection String là bắt buộc!")]
  public string BackplaneConnection { get; set; } = "localhost:6381";

  public RedisKeys Keys { get; set; } = new();
  public static string SectionName => "Redis";

  public static class MutexKeys {
    public const string Cache = "CacheRedis";
    public const string Critical = "CriticalRedis";
    public const string Backplane = "BackplaneRedis";
  }
}

public class RedisKeys {
  public string Cache { get; set; } = "bidding:cache:";
  public string Lock { get; set; } = "bidding:lock:";
  public string Security { get; set; } = "bidding:security:";
  public string Backplane { get; set; } = "bidding:backplane:";
}
