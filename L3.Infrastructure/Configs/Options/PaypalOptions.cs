namespace L3.Infrastructure.Configs.Options;

public class PayPalOptions : IOptionSection {
  public int ExchangeRate = 26220;
  public string ClientId { get; set; } = string.Empty;
  public string ClientSecret { get; set; } = string.Empty;
  public string Mode { get; set; } = "sandbox";
  public string Currency { get; set; } = "USD";

  public string SuccessUrl { get; set; } = string.Empty;
  public string CancelUrl { get; set; } = string.Empty;
  public static string SectionName => "PayPal";
}
