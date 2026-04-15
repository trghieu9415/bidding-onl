using L2.Application.Ports.Configs;

namespace L3.Infrastructure.Options;

public class EmailOptions : IOptionSection {
  public string Host { get; init; } = string.Empty;
  public int Port { get; init; }
  public string Username { get; init; } = string.Empty;
  public string Password { get; init; } = string.Empty;
  public string FromAddress { get; init; } = string.Empty;
  public string FromName { get; init; } = string.Empty;
  public bool EnableSsl { get; init; }
  public string FrontEndUrl { get; init; } = string.Empty;
  public static string SectionName => "EmailSettings";
}
