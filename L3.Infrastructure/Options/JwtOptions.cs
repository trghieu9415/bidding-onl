using System.ComponentModel.DataAnnotations;

namespace L3.Infrastructure.Options;

public class JwtOptions : IAppOptions {
  [Required(ErrorMessage = "Secret Key là bắt buộc!")]
  [MinLength(32, ErrorMessage = "Secret Key quá ngắn, không an toàn!")]
  public string Secret { get; set; } = string.Empty;

  public string Issuer { get; set; } = "BiddingOnlineAPI";

  public string Audience { get; set; } = "BiddingOnlineClient";

  [Range(1, int.MaxValue, ErrorMessage = "Thời gian hết hạn phải lớn hơn 0")]
  public int AccessExpiration { get; set; } = 60;

  [Range(1, int.MaxValue)] public int RefreshExpiration { get; set; } = 1440;
  public static string SectionName => "Jwt";
}
