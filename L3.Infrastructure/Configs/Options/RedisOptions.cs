using System.ComponentModel.DataAnnotations;

namespace L3.Infrastructure.Configs.Options;

public class RedisOptions : IOptionSection {
  [Required(ErrorMessage = "Redis Connection String là bắt buộc!")]
  public string Configuration { get; set; } = "localhost:6379";

  public string InstanceName { get; set; } = "Bidding_";
  public static string SectionName => "Redis";
}
