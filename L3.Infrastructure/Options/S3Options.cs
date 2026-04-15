using L2.Application.Ports.Configs;

namespace L3.Infrastructure.Options;

public class S3Options : IOptionSection {
  public string AccessKey { get; set; } = "minioadmin";
  public string SecretKey { get; set; } = "minioadmin";
  public string ServiceUrl { get; set; } = "http://localhost:9000";
  public string BucketName { get; set; } = "my-bucket";
  public int Retry { get; set; } = 5;
  public int TimeOut { get; set; } = 30;
  public bool ForcePathStyle { get; set; }
  public static string SectionName => "S3Config";
}
