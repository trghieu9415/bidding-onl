using System.ComponentModel.DataAnnotations;
using L2.Application.Ports.Configs;

namespace L3.Infrastructure.Options;

public class RedisSettings : IOptionSection {
  [Required(ErrorMessage = "Redis Connection String là bắt buộc!")]
  public string Configuration { get; set; } = "localhost:6379";

  public string InstanceName { get; set; } = "Bidding_";
  public string InstanceLockName { get; set; } = "Bidding_Lock_";
  public string InstanceRealtimeName { get; set; } = "Bidding_Realtime_";


  public static string SectionName => "Redis";
}
