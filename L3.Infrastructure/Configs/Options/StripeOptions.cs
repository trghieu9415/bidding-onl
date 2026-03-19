namespace L3.Infrastructure.Configs.Options;

public class StripeOptions : IOptionSection {
  public string SecretKey { get; set; } = string.Empty;
  public string PublishableKey { get; set; } = string.Empty;
  public int Retry { get; set; } = 3;

  public string Currency { get; set; } = "vnd";
  public string SuccessUrl { get; set; } = string.Empty;
  public string CancelUrl { get; set; } = string.Empty;
  public static string SectionName => "Stripe";
}
